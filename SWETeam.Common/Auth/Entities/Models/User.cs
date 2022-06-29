using SWETeam.Common.Entities;
using SWETeam.Common.Libraries;
using System;

namespace SWETeam.Common.Auth
{
    public class User : BaseModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        //[Required]
        //[Duplication]
        //[PhoneNumber]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [Duplication]
        [Email]
        public string Email { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Lastname
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Required]
        [Password]
        public string Password { get; set; }

        /// <summary>
        /// Fullname
        /// </summary>
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Giới tính.0 - Nam, 1 - Nữ, 2 - Khác
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Đã Verify hay chưa?
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Đã verify phone hay chưa?
        /// </summary>
        public bool IsPhoneVerified { get; set; }
    }
}
