using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace SWETeam.Common.Mail
{
    public class EmailSetting
    {
        private IConfiguration _config;
        public EmailSetting(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Mail address người gửi
        /// </summary>
        public string Sender
        {
            get
            {
                return _config.GetSection("mail_settings:source_default").Value;
            }
        }

        /// <summary>
        /// Mail address người nhận
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Tên hiển thị
        /// </summary>
        private string _displayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    return _config.GetSection("mail_settings:display_name").Value;
                }
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        /// <summary>
        /// Mức ưu tiên
        /// </summary>
        public MailPriority Priority { get; set; } = MailPriority.Normal;

        /// <summary>
        /// Có sử dụng HTML không?
        /// </summary>
        public bool IsBodyHTML
        {
            get
            {
                return Body != HttpUtility.HtmlEncode(Body);
            }
        }
    }
}
