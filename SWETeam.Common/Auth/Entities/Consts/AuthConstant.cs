using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Auth
{
    public class AuthConstant
    {
        #region Congig
        public const int MIN_TIME_TO_REQUIRED_OTP = 1; // Số phút tối thiểu để yêu cầu cấp OTP mới
        public const int MAX_TIME_OTP = 15; // OTP có giá trị trong 15p
        public const int OTP_LENGTH = 6; // Length OTP
        public const int PASSWORD_MIN_LENGTH = 8; // Min length của password
        public const int PASSWORD_MAX_LENGTH = 64; // Max length của password
        public const int TOKEN_TIME = 5 * 60; // Seconds
        public const int REFRESH_TOKEN_TIME = 12 * 60 * 60; // Seconds

        #endregion

        #region Store
        public const string STORE_REGISTER = "Proc_RegisterUser";
        public const string STORE_LOGIN_RETURN_USER_AND_ROLE = "Proc_LoginReturnUserAndRole";
        #endregion

        #region Message
        public const string USER_NOT_EXIST = "Người dùng không tồn tại trên hệ thống";
        public const string USER_NOT_VERIFIED = "Tài khoản của bạn chưa được xác minh";
        public const string USER_VERIFIED = "Tài khoản của bạn đã được xác minh trước đó";
        public const string REQUIRED_SO_FAST = "OTP chỉ được cấp 1 lần trong 1 phút. Vui lòng chờ và thử lại sau.";
        public const string INVALID_OTP = "OTP không hợp lệ hoặc đã hết hạn";
        public const string SEND_OTP_SUCCESS_MESSAGE = "Gửi OTP thành công. Nếu không nhận được mail, vui lòng kiểm tra trong thư rác/spam. Xin cảm ơn!";
        public const string TWO_PWD_MUST_BE_SAME = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
        public const string OLD_PWD_MUST_BE_DIFFERENT_NEW_PWD = "Mật khẩu cũ và mật khẩu mới phải khác nhau";
        public const string LOGIN_INFO_INCORRECT = "Thông tin đăng nhập không chính xác";
        public const string INFO_INCORRECT = "Thông tin không chính xác";
        public const string ACCOUNT_OR_PASSWORD_NOT_ALLOW_EMPTY = "Tài khoản hoặc mật khẩu không được để trống";
        #endregion

        #region Mail template
        public const string TITLE_RESET_PASSWORD = "[QUAN TRỌNG] THÔNG BÁO CUNG CẤP MÃ OTP ĐẶT LẠI MẬT KHẨU";
        public const string CONTENT_RESET_PASSWORD =
            @"<div>Kính gửi quý khách: <b style='font-size: 16px; font-weight: bold'>{0}<b></div>
              <div>SWE Team xin trân trọng cảm ơn quý khách đã tin tưởng và cùng đồng hành với công ty trong suốt thời gian qua.</div><br>
              <div>Chúng tôi xin gửi đến quý khách thông tin mã OTP để đặt lại mật khẩu: <b style='font-size: 20px; font-weight: bold'>{1}<b></div>
              <div>Link xác thực: <a href='{2}'>tại đây</a></div><br>
              <div style='color: red; font-weight: bold'>MÃ CÓ THỜI HẠN SỬ DỤNG LÀ 15 PHÚT, KỂ TỪ LÚC GỬI YÊU CẦU</div><br>
              <div>Thanks,</div>
              <div>The SWE Team</div>";
        public const string TITLE_PROVIDE_ACCOUNT = "[QUAN TRỌNG] THÔNG BÁO CUNG CẤP MÃ OTP KÍCH HOẠT TÀI KHOẢN";
        public const string CONTENT_PROVIDE_ACCOUNT =
              @"<div>Kính gửi quý khách: <b style='font-size: 16px; font-weight: bold'>{0}<b></div>
              <div>SWE Team xin trân trọng cảm ơn quý khách đã tin tưởng và sử dụng sản phẩm của công ty.</div><br>
              <div>Chúng tôi xin gửi đến quý khách thông tin mã OTP để kích hoạt tài khoản: <b style='font-size: 20px; font-weight: bold'>{1}<b></div>
              <div>Link xác thực: <a href='{2}'>tại đây</a></div><br>
              <div style='color: red; font-weight: bold'>MÃ CÓ THỜI HẠN SỬ DỤNG LÀ 1 NGÀY, KỂ TỪ LÚC GỬI YÊU CẦU</div><br>
              <div>Thanks,</div>
              <div>The SWE Team</div>";
        public const string TITLE_TWO_FACTOR_AUTHENTICATION = "[QUAN TRỌNG] THÔNG BÁO BẢO MẬT";
        public const string CONTENT_TWO_FACTOR_AUTHENTICATION =
              @"<div>Kính gửi quý khách: <b style='font-size: 16px; font-weight: bold'>{0}<b></div>
              <div>Chúng tôi xin gửi đến quý khách thông tin mã OTP để xác nhận đăng nhập: <b style='font-size: 20px; font-weight: bold'>{1}<b></div>
              <div>Link xác thực: <a href='{2}'>tại đây</a></div><br>
              <div style='color: red; font-weight: bold'>MÃ CÓ THỜI HẠN SỬ DỤNG LÀ 15 PHÚT, KỂ TỪ LÚC GỬI YÊU CẦU</div><br>
              <div>Thanks,</div>
              <div>The SWE Team</div>";
        public const string TITLE_CHANGE_PASSWORD = "[QUAN TRỌNG] THÔNG BÁO BẢO MẬT";
        public const string CONTENT_CHANGE_PASSWORD =
             @"<div>Xin chào <b style='font-size: 16px; font-weight: bold'>{0},<b></div><br>
              <div style='color: red; font-weight: bold'>Yêu cầu thay đổi mật khẩu của bạn đã được hoàn thành.</div><br><br>
              <div>Thanks,</div>
              <div>The SWE Team</div>";
        #endregion
    }
}
