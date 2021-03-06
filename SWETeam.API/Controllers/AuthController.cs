using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using SWETeam.Common.Auth;
using SWETeam.Common.Entities;
using SWETeam.Common.Libraries;
using System;
using System.Text;

namespace SWETeam.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Declares
        private readonly IServiceProvider _provider;
        private readonly IHttpContextAccessor _accessor;
        #endregion

        #region Constructors
        public AuthController(IServiceProvider provider)
        {
            _provider = provider;
            _accessor = provider.GetRequiredService<IHttpContextAccessor>();
        }
        #endregion

        #region Controllers
        /// <summary>
        /// Check live token
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping([FromQuery] string uid)
        {
            return Ok("pong");
        }

        /// <summary>
        /// Đăng ký tài khoản
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(User userInfo)
        {
            var authResult = _provider.GetRequiredService<IAuthService>().Register(userInfo);
            return Ok(authResult);
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var authResult = _provider.GetRequiredService<IAuthService>().Login(loginRequest);
            return Ok(authResult);
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        [HttpGet("logout")]
        public IActionResult Logout([FromQuery] string uid)
        {
            var authResult = _provider.GetRequiredService<IAuthService>().Logout(uid);
            return Ok(authResult);
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        [HttpPost("change-password")]
        public IActionResult ChangePassword(ChangePasswordRequest model)
        {
            var authResult = _provider.GetRequiredService<IAuthService>().ChangePassword(model);
            return Ok(authResult);
        }

        /// <summary>
        /// Đổi mật khẩu với mã OTP
        /// </summary>
        [AllowAnonymous]
        [HttpPost("change-password-otp")]
        public IActionResult ChangePasswordOTP(ChangePasswordRequest model)
        {
            var authResult = _provider.GetRequiredService<IAuthService>().ChangePasswordOTP(model);
            return Ok(authResult);
        }

        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <param name="poe">PhoneNo or Email</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("reset-password")]
        public IActionResult ResetPassword([FromQuery] string poe)
        {
            var result = _provider.GetRequiredService<IAuthService>().ResetPassword(poe?.ToBase64Decode());
            if (result.Success)
            {
                result.Message = AuthConstant.SEND_OTP_SUCCESS_MESSAGE;
            }
            return Ok(result);
        }

        /// <summary>
        /// Cấp OTP mới
        /// u: UserName
        /// </summary>
        [AllowAnonymous]
        [HttpGet("provide-new-otp")]
        public IActionResult ProvideNewOtp([FromQuery] string u, [FromQuery] int type)
        {
            var authResult = new AuthResult();
            _provider.GetRequiredService<IAuthService>().ProvideNewOtp(u, type).Downcast(out authResult);
            return Ok(authResult);
        }

        /// <summary>
        /// Xác minh tài khoản
        /// </summary>
        [AllowAnonymous]
        [HttpPost("verify-account")]
        public IActionResult VerifyAccount(VerifyRequest verify)
        {
            var result = _provider.GetRequiredService<IAuthService>().VerifyAccount(verify);
            return Ok(result);
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(RefreshTokenRequest refresh)
        {
            var result = _provider.GetRequiredService<IAuthService>().RefreshToken(refresh);
            return Ok(result);
        }

        /// <summary>
        /// Kiểm tra xem có tồn tại user 
        /// u: UserName
        /// </summary>
        [AllowAnonymous]
        [HttpGet("check-has-user")]
        public IActionResult CheckHasUser([FromQuery] string u)
        {
            bool result = _provider.GetRequiredService<IAuthService>().GetUserByPhoneOrEmail(u?.ToBase64Decode()) != null;
            return Ok(result);
        }
        #endregion
    }
}
