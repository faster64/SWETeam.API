using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SWETeam.Common.Entities;
using SWETeam.Common.Libraries;
using SWETeam.Common.Mail;
using SWETeam.Common.MySQL;
using SWETeam.Common.Validations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static SWETeam.Common.Auth.AuthEnumeration;
using static SWETeam.Common.Entities.Enumeration;

namespace SWETeam.Common.Auth
{
    public  class AuthService : IAuthService
    {
        #region Declare
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public AuthService(IServiceProvider provider)
        {
            _provider = provider;
            _config = provider.GetRequiredService<IConfiguration>();
            _unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Đăng ký tài khoản
        /// </summary>
        public AuthResult Register(User userInfo)
        {

            AuthResult authResult = new AuthResult();
            ValidateResult validateResult = new Validator(_provider).Execute(userInfo, "user");

            // Nếu thông tin không hợp lệ thì return luôn
            if (!validateResult.IsValid)
            {
                authResult.Code = System.Net.HttpStatusCode.BadRequest;
                authResult.ValidateInfo = validateResult.ValidateFields.CloneJson();
                return authResult;
            }

            // Nếu thông tin hợp lệ thì thêm vào bảng User
            Guid userId = Guid.NewGuid();
            Dictionary<string, object> param = new Dictionary<string, object>();
            var properties = userInfo.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0);
            string otp = Libraries.Utility.RandomNumber(AuthConstant.OTP_LENGTH);

            foreach (var prop in properties)
            {
                param.Add($"v_{prop.Name}", prop.GetValue(userInfo));
            }

            // Cập nhật lại field PasswordHash, thêm field Id
            param.AddOrUpdate("v_PasswordHash", userInfo.Password.ToMD5());
            param.AddOrUpdate("v_Id", userId.ToString());

            int result = _unitOfWork.Execute(AuthConstant.STORE_REGISTER, param, commandType: System.Data.CommandType.StoredProcedure);

            if (result == 0)
            {
                authResult.Code = System.Net.HttpStatusCode.InternalServerError;
                authResult.ErrorMessage = Constant.HAS_ERROR_MESSAGE;
            }
            else
            {
                _unitOfWork.Commit();

                // Gửi OTP verify về Mail
                userInfo.Id = userId;
                ProvideOtpToMail(new SendOtpMailConfig() { User = userInfo, OtpType = OtpType.Verify }, out string newOtp);
            }

            return authResult;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        public AuthResult Login(LoginRequest loginRequest)
        {
            // Validate trước khi login
            var validate = new Validator(_provider).Execute(loginRequest);
            if (!validate.IsValid)
            {
                return new AuthResult() { Code = System.Net.HttpStatusCode.BadRequest, ErrorMessage = AuthConstant.ACCOUNT_OR_PASSWORD_NOT_ALLOW_EMPTY };
            }

            string usernameDecode = loginRequest.UserName.ToBase64Decode();
            string passwordDecode = loginRequest.Password.ToBase64Decode();

            // Execute login
            string storeName = AuthConstant.STORE_LOGIN_RETURN_USER_AND_ROLE;

            LoginResult loginResult = _unitOfWork.QuerySingleOrDefault<LoginResult>
                                (
                                    storeName,
                                    new Dictionary<string, object>() {
                                        { "v_UserName", usernameDecode },
                                        { "v_PasswordHash", passwordDecode.ToMD5() },
                                    },
                                    commandType: System.Data.CommandType.StoredProcedure
                                );

            // Nếu không tìm thấy user thì return luôn
            if (loginResult == null)
            {
                return new AuthResult() { Code = System.Net.HttpStatusCode.BadRequest, ErrorMessage = AuthConstant.LOGIN_INFO_INCORRECT };
            }

            // Nếu tài khoản chưa đc xác minh
            if (!loginResult.IsVerified)
            {
                return new AuthResult() { Code = (System.Net.HttpStatusCode)HttpStatusCodeExtension.NotVerified, ErrorMessage = AuthConstant.USER_NOT_VERIFIED };
            }

            // Kiểm tra xem có 2FA không, nếu có thì gửi OTP
            string tfaCommand = "SELECT * FROM user_two_factor_authentication WHERE UserId = @UserId";
            UserTwoFactor userTwoFactor = _unitOfWork.QuerySingleOrDefault<UserTwoFactor>(tfaCommand, new { UserId = loginResult.Id });
            if (userTwoFactor != null && userTwoFactor.TwoFactorEnabled)
            {
                // Nếu là xác thực qua mail, thì gửi OTP về mail
                if (userTwoFactor.TwoFactorType == TwoFactorType.Email)
                {
                    ProvideOtpResult otpResult = ProvideOtpToMail(new SendOtpMailConfig() { User = loginResult, OtpType = OtpType.TFA, DeleteOldOtp = true, WaitRequired = false }, out string otp);
                    if (!otpResult.Success)
                    {
                        return new AuthResult() { Code = System.Net.HttpStatusCode.InternalServerError, ErrorMessage = otpResult.ErrorMessage };
                    }

                    return new AuthResult() { Message = "Người dùng này sử dụng two-factor authentication. OTP đã được gửi về mail. Vui lòng kiểm tra mail. nếu không thấy, tìm trong thư rác/spam. Xin cảm ơn!" };
                }

                return new AuthResult() { Code = System.Net.HttpStatusCode.BadRequest, ErrorMessage = "Chưa hỗ trợ phương thức xác thực này." };
            }

            // Lưu refresh token
            SaveRefreshToken(loginResult.Id.ToString(), out string refreshToken);

            return new AuthResult() { AccessToken = GenerateToken(loginResult), RefreshToken = refreshToken };
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        public AuthResult Logout(string uid)
        {
            if (!string.IsNullOrEmpty(uid))
            {
                string sqlCommand = "DELETE FROM refresh_token WHERE UserId = @UserId and ExpriedTime > NOW()";
                _unitOfWork.ExecuteAutoCommit(sqlCommand, new { UserId = uid });

                // Revoke token
                // ...
            }
            return new AuthResult() { Message = "Logout successfully" };
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        public AuthResult RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            AuthResult result = new AuthResult();
            var validate = new Validator(_provider).Execute(refreshTokenRequest);
            if (!validate.IsValid)
            {
                result.Code = System.Net.HttpStatusCode.BadRequest;
                result.ValidateInfo = validate.ValidateFields.CloneJson();
                return result;
            }
            string queryCommand = "SELECT Id FROM refresh_token WHERE UserId = @UserId AND RefreshToken = @RefreshToken AND ExpriedTime > NOW()";
            bool isValid = _unitOfWork.QuerySingleOrDefault<RefreshTokenRequest>(queryCommand, new { UserId = refreshTokenRequest.UserId, RefreshToken = refreshTokenRequest.RefreshToken }) != null;
            if (isValid)
            {
                string sqlCommand = "SELECT u.*, ru.RoleId, r.RoleName FROM user u INNER JOIN role_user ru INNER JOIN `role` r ON r.Id = ru.RoleId WHERE u.Id = ru.UserId AND u.Id = @UserId;";
                LoginResult login = _unitOfWork.QuerySingleOrDefault<LoginResult>(sqlCommand, new { UserId = refreshTokenRequest.UserId });

                // Lưu refresh token
                SaveRefreshToken(refreshTokenRequest.UserId.ToString(), out string refreshToken);

                result.AccessToken = GenerateToken(login);
                result.RefreshToken = refreshToken;
            }
            else
            {
                result.Code = System.Net.HttpStatusCode.BadRequest;
                result.ErrorMessage = "RefreshToken không tồn tại hoặc đã hết hạn";
            }
            return result;
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public AuthResult ChangePassword(ChangePasswordRequest model)
        {
            model.IsUseOTP = false;
            AuthResult authResult = new AuthResult();

            // Validate
            ValidateResult validateResult = ValidateBeforeChangePassword(model, out User user);

            // Nếu model không hợp lệ thì return luôn
            if (!validateResult.IsValid)
            {
                authResult.Code = System.Net.HttpStatusCode.BadRequest;
                authResult.ValidateInfo = validateResult.ValidateFields.CloneJson();
                return authResult;
            }

            // Decode
            model.UserName = model.UserName.ToBase64Decode();
            model.OldPassword = model.OldPassword.ToBase64Decode();
            model.NewPassword = model.NewPassword.ToBase64Decode();
            model.ConfirmNewPassword = model.ConfirmNewPassword.ToBase64Decode();

            string updateQuery = "UPDATE user u SET u.PasswordHash = @PasswordHash WHERE u.Email = @UserName or u.PhoneNumber = @UserName";
            int updateResult = _unitOfWork.ExecuteAutoCommit(
                updateQuery,
                new
                {
                    PasswordHash = model.NewPassword.ToMD5(),
                    Username = model.UserName
                });

            if (updateResult == 0)
            {
                authResult.Success = false;
                authResult.ErrorMessage = Constant.HAS_ERROR_MESSAGE;
            }

            // Gửi mail cảnh báo đã đổi mật khẩu
            MailParameter config = new MailParameter(_config);
            config.To = model.UserName;
            config.Subject = AuthConstant.TITLE_CHANGE_PASSWORD;
            config.Body = string.Format(AuthConstant.CONTENT_CHANGE_PASSWORD, user.FullName);
            config.Priority = System.Net.Mail.MailPriority.High;

            EmailHelper.SendMailAsync(config);

            authResult.Message = "Success";
            return authResult;
        }

        /// <summary>
        /// Đổi mật khẩu với OTP
        /// </summary>
        public AuthResult ChangePasswordOTP(ChangePasswordRequest model)
        {
            model.IsUseOTP = true;
            model.OldPassword = "_";
            AuthResult authResult = new AuthResult();

            ValidateResult validateResult = ValidateBeforeChangePassword(model, out User user);

            // Nếu model không hợp lệ thì return luôn
            if (!validateResult.IsValid)
            {
                authResult.Code = System.Net.HttpStatusCode.BadRequest;
                authResult.ValidateInfo = validateResult.ValidateFields.CloneJson();
                return authResult;
            }

            // Check OTP
            string queryCommand = @$"SELECT 
                                    o.Id, o.UserId, o.ExpriedTime, o.ProvidedDate 
                                    FROM otp o 
                                    WHERE o.UserId = @UserId 
                                    AND Otp = @Otp
                                    AND o.IsUsed = b'0' 
                                    AND o.ExpriedTime >= NOW() 
                                    AND o.Type = @OtpType";
            OTP otp = _unitOfWork.QueryFirstOrDefault<OTP>(queryCommand, new { UserId = user.Id, Otp = model.OTP, OtpType = (int)OtpType.Password });

            // Nếu không có OTP hoặc OTP đã hết hạn
            if (otp == null)
            {
                authResult.Code = System.Net.HttpStatusCode.BadRequest;
                authResult.ErrorMessage = AuthConstant.INVALID_OTP;
                return authResult;
            }

            string updateCommand = @"UPDATE user u SET u.PasswordHash = @PasswordHash WHERE u.Id = @UserId;
                                    UPDATE otp o SET o.IsUsed = b'1' WHERE o.Id = @OtpId";
            int result = _unitOfWork.Execute(updateCommand, new { PasswordHash = model.NewPassword.ToMD5(), UserId = user.Id, OtpId = otp.Id });
            if (result == 0)
            {
                authResult.Code = System.Net.HttpStatusCode.InternalServerError;
                authResult.ErrorMessage = Constant.HAS_ERROR_MESSAGE;
            }
            else
            {
                _unitOfWork.Commit();
            }

            return authResult;
        }

        /// <summary>
        /// Validate trước khi đổi mật khẩu
        /// </summary>
        private ValidateResult ValidateBeforeChangePassword(ChangePasswordRequest model, out User user)
        {
            user = null;
            ValidateResult result = new Validator(_provider).Execute(model);
            if (!result.IsValid)
            {
                return result;
            }

            // Check xem có user không?
            string checkCommand = "SELECT Id, IsVerified, FirstName, LastName FROM user WHERE (PhoneNumber = @UserName OR Email = @UserName) AND PasswordHash = @PasswordHash";
            user = _unitOfWork.QuerySingleOrDefault<User>(checkCommand, new { UserName = model.UserName.ToBase64Decode(), PasswordHash = model.OldPassword.ToBase64Decode().ToMD5() });
            if (user == null)
            {
                result.ValidateFields.Add(new ValidateField { FieldName = "UserName", ErrorMessage = AuthConstant.INFO_INCORRECT });
                return result;
            }

            // Check xem verify chưa?
            if (!user.IsVerified)
            {
                result.ValidateFields.Add(new ValidateField { FieldName = "UserName", ErrorMessage = AuthConstant.USER_NOT_VERIFIED });
                return result;
            }

            // Check mật khẩu và mật khẩu xác nhận giống nhau
            if (!model.NewPassword.Equals(model.ConfirmNewPassword))
            {
                result.ValidateFields.Add(new ValidateField { FieldName = "NewPassword", ErrorMessage = AuthConstant.TWO_PWD_MUST_BE_SAME });
                return result;
            }

            // Check mật khẩu cũ và mới phải khác nhau
            if (model.OldPassword.Equals(model.NewPassword))
            {
                result.ValidateFields.Add(new ValidateField { FieldName = "OldPassword", ErrorMessage = AuthConstant.OLD_PWD_MUST_BE_DIFFERENT_NEW_PWD });
                return result;
            }

            // Nếu sử dụng OTP để đổi mật khẩu, thì validate OTP
            if (model.IsUseOTP)
            {
                string otp = model.OTP;
                if (string.IsNullOrEmpty(otp) || otp.Length != AuthConstant.OTP_LENGTH || !Int32.TryParse(otp, out int num))
                {
                    result.ValidateFields.Add(new ValidateField { FieldName = "OTP", ErrorMessage = AuthConstant.INVALID_OTP });
                }
            }

            return result;
        }

        /// <summary>
        /// Gửi OTP về Mail cho user
        /// </summary>
        /// <param name="poe"></param>
        /// <returns></returns>
        public ResetPasswordResult ResetPassword(string poe)
        {
            // Từ POE lấy ra UserId, nếu không có user thì return luôn
            User user = GetUserByPhoneOrEmail(poe);
            if (user == null)
            {
                return new ResetPasswordResult() { Success = false, ErrorMessage = AuthConstant.USER_NOT_EXIST };
            }
            // Nếu có user, kiểm tra xem đã verify chưa?
            if (!user.IsVerified)
            {
                return new ResetPasswordResult() { Success = false, ErrorMessage = AuthConstant.USER_NOT_VERIFIED };
            }

            // Bắn OTP
            ProvideOtpResult provideOtpResult = ProvideOtpToMail(new SendOtpMailConfig() { User = user, OtpType = OtpType.Password }, out string newOtp);
            if (!provideOtpResult.Success)
            {
                return new ResetPasswordResult() { Success = false, ErrorMessage = provideOtpResult.ErrorMessage };
            }

            return new ResetPasswordResult();
        }

        /// <summary>
        /// Xác minh tài khoản
        /// </summary>
        public VerifyOtpResult VerifyAccount(VerifyRequest verify)
        {
            VerifyOtpResult result = new VerifyOtpResult();

            // Validate model
            ValidateResult validateResult = new Validator(_provider).Execute(verify);
            if (!validateResult.IsValid)
            {
                result.Code = System.Net.HttpStatusCode.BadRequest;
                if (validateResult.ValidateFields != null && validateResult.ValidateFields.Count > 0)
                {
                    result.ErrorMessage = validateResult.ValidateFields[0].ErrorMessage;
                }
                return result;
            }

            // Get user
            User user = GetUserByPhoneOrEmail(verify.UserName);
            if (user == null)
            {
                result.Code = System.Net.HttpStatusCode.BadRequest;
                result.ErrorMessage = AuthConstant.USER_NOT_EXIST;
            }
            else
            {
                // Nếu tài khoản đã đc xác minh rồi thì báo lỗi
                if (user.IsVerified)
                {
                    result.Code = System.Net.HttpStatusCode.BadRequest;
                    result.ErrorMessage = AuthConstant.USER_VERIFIED;
                }
                else
                {
                    // Validate OTP
                    if (string.IsNullOrEmpty(verify.OTP) || verify.OTP.Length != AuthConstant.OTP_LENGTH || !Int32.TryParse(verify.OTP, out int num))
                    {
                        result.Code = System.Net.HttpStatusCode.BadRequest;
                        result.ErrorMessage = AuthConstant.INVALID_OTP;
                    }
                    else
                    {
                        // Check xem OTP có đúng loại verify không?
                        List<OTP> otps = GetListOtp(user.Id);
                        if (otps.Count == 0 || otps.Find(o => verify.OTP == o.Otp && o.Type == OtpType.Verify) == null)
                        {
                            result.Code = System.Net.HttpStatusCode.BadRequest;
                            result.ErrorMessage = AuthConstant.INVALID_OTP;
                        }
                        // Nếu OTP hợp lệ thì verify tài khoản
                        else
                        {
                            string verifyCommand = @"UPDATE user u SET u.IsVerified = b'1' WHERE u.Id = @UserId;
                                                     UPDATE otp o SET o.IsUsed = b'1' WHERE o.UserId = @UserId AND o.Type = @OtpType";
                            int verifyResult = _unitOfWork.Execute(verifyCommand, new { UserId = user.Id, OtpType = (int)OtpType.Verify });
                            if (verifyResult > 0)
                            {
                                _unitOfWork.Commit();
                            }
                            else
                            {
                                result.Code = System.Net.HttpStatusCode.InternalServerError;
                                result.ErrorMessage = Constant.HAS_ERROR_MESSAGE;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Cấp OTP mới
        /// </summary>
        public BaseResponse ProvideNewOtp(string u, int type)
        {
            BaseResponse result = new BaseResponse();

            // Validate type
            if (Enum.IsDefined(typeof(OtpType), type) == false)
            {
                result.Success = false;
                result.ErrorMessage = "type không hợp lệ";
                return result;
            }

            u = u.ToBase64Decode();
            User user = GetUserByPhoneOrEmail(u);
            if (user == null)
            {
                result.Success = false;
                result.ErrorMessage = AuthConstant.USER_NOT_EXIST;
                return result;
            }

            // Nếu tài khoản đã xác minh mà yêu cầu cấp OTP xác minh thì báo lỗi
            if (type == (int)OtpType.Verify && user.IsVerified)
            {
                result.Success = false;
                result.ErrorMessage = AuthConstant.USER_VERIFIED;
            }
            else
            {
                SendOtpMailConfig sendMailConfig = new SendOtpMailConfig();
                sendMailConfig.OtpType = (OtpType)type;
                sendMailConfig.User = user;
                sendMailConfig.DeleteOldOtp = true;
                sendMailConfig.WaitRequired = true;
                var provideResult = ProvideOtpToMail(sendMailConfig, out string otp);
                if (provideResult.Success == false)
                {
                    result.Success = false;
                    result.ErrorMessage = provideResult.ErrorMessage;
                }
                else
                {
                    result.Message = $"OTP đã được gửi về mail {user.Email}. Nếu không thấy vui lòng kiểm tra thư rác/spam. Xin cảm ơn!";
                }
            }

            return result;
        }

        /// <summary>
        /// Cấp OTP qua mail
        /// </summary>
        private ProvideOtpResult ProvideOtpToMail(SendOtpMailConfig sendMailConfig, out string newOtp)
        {
            ProvideOtpResult result = new ProvideOtpResult();
            newOtp = "";

            if (sendMailConfig.OtpType == OtpType.None)
            {
                result.Success = false;
                return result;
            }

            // Check xem có được cấp OTP không?
            string queryCommand = @"SELECT 
                                    o.Id, o.UserId, o.ExpriedTime, o.ProvidedDate 
                                    FROM otp o 
                                    WHERE o.UserId = @UserId 
                                    AND o.IsUsed = b'0' 
                                    AND DATE_SUB(o.ExpriedTime, INTERVAL 5 SECOND) >= NOW() 
                                    AND o.Type = @OtpType";
            List<OTP> otps = _unitOfWork.Query<OTP>(queryCommand, new { UserId = sendMailConfig.User.Id, OtpType = sendMailConfig.OtpType }).ToList();
            /**
             * Nếu có OTP chưa sử dụng, check xem đã cấp đc đủ 1 phút chưa?
             * Nếu đủ rồi thì cấp OTP mới
             */
            OTP otp = null;

            // Nếu cần đợi để cấp otp
            if (sendMailConfig.WaitRequired)
            {
                otp = otps.Find(o => (DateTime.Now - o.ProvidedDate).TotalSeconds < AuthConstant.MIN_TIME_TO_REQUIRED_OTP * 60);
            }

            if (otp != null)
            {
                result.Success = false;
                result.ErrorMessage = AuthConstant.REQUIRED_SO_FAST;
                return result;
            }

            // Cấp OTP mới
            newOtp = Libraries.Utility.RandomNumber(AuthConstant.OTP_LENGTH);
            int maxTimeOtp = AuthConstant.MAX_TIME_OTP * 60;
            if (sendMailConfig.OtpType == OtpType.Verify)
            {
                maxTimeOtp = 1 * 24 * 60 * 60;
            }

            // Nếu cần xóa mã OTP cũ
            if (sendMailConfig.DeleteOldOtp)
            {
                string deleteCommand = "DELETE FROM otp WHERE UserId = @UserId AND Type = @Type";
                _unitOfWork.ExecuteAutoCommit(deleteCommand, new { @UserId = sendMailConfig.User.Id, Type = (int)sendMailConfig.OtpType });
            }

            // Thêm OTP vào bảng
            string insertCommand = @$"INSERT INTO otp (Id, UserId, Otp, ExpriedTime, IsUsed, ProvidedDate, Type) 
                                      VALUES (
                                            UUID(), 
                                            @UserId,
                                            @NewOTP, 
                                            DATE_ADD(NOW(), INTERVAL {maxTimeOtp} SECOND), 
                                            0, 
                                            NOW(),
                                            @OtpType
                                      );";
            result.Success = _unitOfWork.Execute(insertCommand, new { @UserId = sendMailConfig.User.Id, NewOTP = newOtp, OtpType = sendMailConfig.OtpType }) > 0;
            if (result.Success)
            {
                // Bắn OTP về mail
                string otpTask = newOtp;
                string subject = string.Empty;
                string body = string.Empty;
                string clientUrl = _config.GetSection("ClientUrl").Value;

                // template reset password
                if (sendMailConfig.OtpType == OtpType.Password)
                {
                    subject = AuthConstant.TITLE_RESET_PASSWORD;
                    body = String.Format(AuthConstant.CONTENT_RESET_PASSWORD, sendMailConfig.User.FullName.ToUpper(), otpTask, ""); ;
                }
                // template verify account
                else if (sendMailConfig.OtpType == OtpType.Verify)
                {
                    string mailEncode = sendMailConfig.User.Email.ToBase64Encode();
                    subject = AuthConstant.TITLE_PROVIDE_ACCOUNT;
                    body = String.Format(AuthConstant.CONTENT_PROVIDE_ACCOUNT, sendMailConfig.User.FullName.ToUpper(), otpTask, $"{clientUrl}/auth/verify-register/{mailEncode}");
                }
                // template two factor authentication
                else if (sendMailConfig.OtpType == OtpType.TFA)
                {
                    string mailEncode = sendMailConfig.User.Email.ToBase64Encode();
                    subject = AuthConstant.TITLE_TWO_FACTOR_AUTHENTICATION;
                    body = String.Format(AuthConstant.CONTENT_TWO_FACTOR_AUTHENTICATION, sendMailConfig.User.FullName.ToUpper(), otpTask, $"{clientUrl}/auth/login-register/{mailEncode}");
                }

                new Task(() =>
                {
                    MailParameter parameter = new MailParameter(_config);
                    parameter.To = sendMailConfig.User.Email;
                    parameter.Subject = subject;
                    parameter.Body = body;
                    EmailHelper.SendMailAsync(parameter).Wait();
                }).Start();

                _unitOfWork.Commit();
            }
            else
            {
                result.ErrorMessage = Constant.HAS_ERROR_MESSAGE;
            }

            return result;
        }

        /// <summary>
        /// Get user từ email hoặc số điện thoại
        /// </summary>
        public User GetUserByPhoneOrEmail(string poe)
        {
            if (string.IsNullOrEmpty(poe))
            {
                return null;
            }

            // Từ POE lấy ra UserId, nếu không có user thì return luôn
            User user = _unitOfWork.QuerySingleOrDefault<User>("SELECT * FROM user WHERE (PhoneNumber = @POE OR Email = @POE)", new { POE = poe });
            return user;
        }

        /// <summary>
        /// Lấy ra List OTP chưa hết hạn, chưa đc sử dụng theo user id
        /// </summary>
        private List<OTP> GetListOtp(Guid userId)
        {
            string sqlCommand = "SELECT * FROM otp WHERE UserId = @UserId AND IsUsed = b'0' AND DATE_SUB(ExpriedTime, INTERVAL 5 SECOND) > NOW()";
            return _unitOfWork.Query<OTP>(sqlCommand, new { UserId = userId.ToString() }).ToList();
        }

        /// <summary>
        /// Sinh token
        /// </summary>
        private string GenerateToken(LoginResult login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>();
            var secretKey = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);
            var symmetricSecurityKey = new SymmetricSecurityKey(secretKey);
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            // add claims
            claims.Add(new Claim("UserId", login.Id.ToString()));
            claims.Add(new Claim("UserName", login.Email));
            claims.Add(new Claim("RoleId", login.RoleId.ToString()));
            claims.Add(new Claim("RoleName", login.RoleName.ToString()));
            claims.Add(new Claim("CreatedAt", DateTime.Now.ToString()));

            var accessToken = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddSeconds(AuthConstant.TOKEN_TIME),
                    signingCredentials: credentials
                );

            return tokenHandler.WriteToken(accessToken);
        }

        /// <summary>
        /// Sinh refresh token
        /// </summary>
        private string GenerateRefreshToken()
        {
            return Libraries.Utility.MixRandom(128);
        }

        /// <summary>
        /// Lưu refresh token
        /// </summary>
        private int SaveRefreshToken(string uid, out string refreshToken)
        {
            refreshToken = GenerateRefreshToken();
            string sqlCommand = @$"DELETE FROM refresh_token WHERE UserId = @UserId; 
                                  INSERT INTO refresh_token(UserId, RefreshToken, ExpriedTime) 
                                  VALUES (@UserId, @Refreshtoken, DATE_ADD(NOW(), INTERVAL {AuthConstant.REFRESH_TOKEN_TIME} SECOND))";
            return _unitOfWork.ExecuteAutoCommit(sqlCommand, new { UserId = uid, RefreshToken = refreshToken });
        }
        #endregion
    }

    internal class ProvideOtpResult
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }

    internal class SendOtpMailConfig
    {
        public User User { get; set; }

        public OtpType OtpType { get; set; }

        public bool DeleteOldOtp { get; set; } = false;

        public bool WaitRequired { get; set; } = true;
    }
}
