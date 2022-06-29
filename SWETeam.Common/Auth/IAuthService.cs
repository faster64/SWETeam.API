using SWETeam.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Auth
{
    public interface IAuthService
    {
        /// <summary>
        /// Đăng ký tài khoản
        /// </summary>
        AuthResult Register(User userInfo);

        /// <summary>
        /// Đăng nhập
        /// </summary>
        AuthResult Login(LoginRequest loginRequest);

        /// <summary>
        /// Đăng xuất
        /// </summary>
        AuthResult Logout(string uid);

        /// <summary>
        /// Refresh token
        /// </summary>
        AuthResult RefreshToken(RefreshTokenRequest refreshTokenRequest);

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        AuthResult ChangePassword(ChangePasswordRequest changePasswordModel);

        /// <summary>
        /// Đổi mật khẩu với OTP
        /// </summary>
        AuthResult ChangePasswordOTP(ChangePasswordRequest changePasswordModel);

        /// <summary>
        /// Gửi thông tin để lấy lại mật khẩu
        /// </summary>
        ResetPasswordResult ResetPassword(string poe);

        /// <summary>
        /// Xác minh tài khoản
        /// </summary>
        VerifyOtpResult VerifyAccount(VerifyRequest verifyRequest);

        /// <summary>
        /// Cấp OTP mới
        /// </summary>
        BaseResponse ProvideNewOtp(string u, int type);

        /// <summary>
        /// Get user từ email hoặc số điện thoại
        /// </summary>
        User GetUserByPhoneOrEmail(string poe);
    }
}
