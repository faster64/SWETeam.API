using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWETeam.Common.Caching;
using SWETeam.Common.MongoDB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static SWETeam.Common.Mail.MailEnumeration;

namespace SWETeam.Common.Mail
{
    public static class EmailHelper
    {
        private static IServiceProvider _provider;
        private static IConfiguration _config;

        /// <summary>
        /// Gửi mail async
        /// Note: tránh SpamAssassin gắn cờ:
        ///     + Priority không để là high nếu không cần thiết (Spam is often marked with high priority, so your message will be treated with more scrutiny)
        ///     + IsBodyHTML nếu là true, thì phải dùng template html
        /// </summary>
        public static async Task<bool> SendMailAsync(MailParameter parameter)
        {
            // Validate
            ValidateMailResult validateMailResult = ValidateMail(parameter);
            if (!validateMailResult.IsValid)
            {
                throw new Exception(validateMailResult.ErrorMessage);
            }

            // GetConfig
            GetConfig(parameter, out MailMessage message, out SmtpClient client);

            // Execute
            bool success = await Execute(client, message);
            if (success) return true;

            #region  Cơ chế retry nếu xảy ra lỗi, thì sử dụng source mail khác để gửi lại
            var cacheKey = _config.GetSection("CacheManager:Keys:SourceMail").Value;
            var source = MemoryCacheHelper.Get(cacheKey);
            var currentIndex = 0;
            if (source == null)
            {
                source = _provider.GetRequiredService<IMongoService<SourceMail>>().GetAll();
                MemoryCacheHelper.Set(cacheKey, source, 24 * 60);
            }

            var sourceDeserialized = source as List<SourceMail>;
            while (!success)
            {
                if (currentIndex >= sourceDeserialized.Count)
                    return false;

                var option = sourceDeserialized[currentIndex++];
                client.Credentials = new NetworkCredential(option.Mail, option.AppPassword);
            }
            #endregion

            return true;
        }

        /// <summary>
        /// Get config
        /// </summary>
        private static void GetConfig(MailParameter param, out MailMessage message, out SmtpClient client)
        {
            message = new MailMessage(from: param.From, to: param.To);
            message.From = new MailAddress(param.From, param.DisplayName);
            message.Subject = param.Subject;
            message.SubjectEncoding = Encoding.UTF8;
            message.Body = param.Body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = param.IsBodyHTML;
            message.Priority = param.Priority;
            message.ReplyToList.Add(new MailAddress(param.From));

            client = new SmtpClient()
            {
                Host = _config.GetSection("MailSettings:Host").Value,
                Port = Int32.Parse(_config.GetSection("MailSettings:Port").Value),
                UseDefaultCredentials = true,
                EnableSsl = true
            };
            client.Credentials = new NetworkCredential(param.From, _config.GetSection("MailSettings:AppPassword").Value);
        }

        /// <summary>
        /// Validate mail
        /// </summary>
        private static ValidateMailResult ValidateMail(MailParameter parameter)
        {
            ValidateMailResult result = new ValidateMailResult();
            if (parameter == null)
            {
                result.ErrorCode = MailErrorCode.ParameterNull;
                result.ErrorMessage = MailConstant.MAIL_PARAM_NULL;
            }
            else if (string.IsNullOrWhiteSpace(parameter.To))
            {
                result.FieldError = "To";
                result.ErrorMessage = MailConstant.TO_MAIL_EMPTY_ERROR;
            }
            else if (string.IsNullOrEmpty(parameter.Subject))
            {
                result.FieldError = "Subject";
                result.ErrorMessage = MailConstant.SUBJECT_EMPTY_ERROR;
            }
            else if (string.IsNullOrEmpty(parameter.Body))
            {
                result.FieldError = "Body";
                result.ErrorMessage = MailConstant.BODY_EMPTY_ERROR;
            }
            else if (!string.IsNullOrWhiteSpace(parameter.To))
            {
                try
                {
                    new MailAddress(parameter.To);
                }
                catch (FormatException)
                {
                    result.FieldError = "To";
                    result.ErrorMessage = MailConstant.TO_MAIL_INVALID_ERROR;
                }
            }

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                result.ErrorCode = MailErrorCode.Required;
            }

            return result;
        }

        /// <summary>
        /// Thực hiện gửi mail
        /// </summary>
        private static async Task<bool> Execute(SmtpClient smtpClient, MailMessage message)
        {
            try
            {
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Inject provider
        /// </summary>
        /// <param name="provider"></param>
        public static void InjectProvider(IServiceProvider provider)
        {
            _provider = provider;
            InjectDependency();
        }

        /// <summary>
        /// DI tại đây
        /// </summary>
        public static void InjectDependency()
        {
            _config = _provider.GetRequiredService<IConfiguration>();
        }
    }
}
