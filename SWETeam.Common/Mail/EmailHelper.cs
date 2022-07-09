using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWETeam.Common.Caching;
using SWETeam.Common.Exceptions;
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
        public static async Task<bool> SendMailAsync(EmailSetting setting, Action<bool> callback = null)
        {
            // Validate
            ValidateMail(setting);

            InjectDependency();

            // GetConfig
            GetConfig(setting, out MailMessage message, out SmtpClient client);

            // Execute
            bool success = await Execute(client, message);
            if (success)
            {
                if (callback != null)
                {
                    callback.Invoke(true);
                }
                return true;
            }

            #region  Cơ chế retry nếu xảy ra lỗi, thì sử dụng source mail khác để gửi lại
            var cacheKey = _config.GetSection("cache_manager:email:keys:source_mail").Value;
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
                {
                    if (callback != null)
                    {
                        callback.Invoke(false);
                    }
                    return false;
                }

                var option = sourceDeserialized[currentIndex++];
                client.Credentials = new NetworkCredential(option.Mail, option.AppPassword);
            }
            #endregion

            return true;
        }

        /// <summary>
        /// Get config
        /// </summary>
        private static void GetConfig(EmailSetting setting, out MailMessage message, out SmtpClient client)
        {
            message = new MailMessage(from: setting.Sender, to: setting.To);
            message.From = new MailAddress(setting.Sender, setting.DisplayName);
            message.Subject = setting.Subject;
            message.SubjectEncoding = Encoding.UTF8;
            message.Body = setting.Body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = setting.IsBodyHTML;
            message.Priority = setting.Priority;
            message.ReplyToList.Add(new MailAddress(setting.Sender));

            client = new SmtpClient()
            {
                Host = _config.GetSection("mail_settings:host").Value,
                Port = Int32.Parse(_config.GetSection("mail_settings:port").Value),
                UseDefaultCredentials = true,
                EnableSsl = true
            };
            client.Credentials = new NetworkCredential(setting.Sender, _config.GetSection("mail_settings:app_password").Value);
        }

        /// <summary>
        /// Validate mail
        /// </summary>
        private static ValidateMailResult ValidateMail(EmailSetting setting)
        {
            if (setting == null)
                throw new CaughtableException(MailConstant.MAIL_PARAM_NULL);

            else if (string.IsNullOrWhiteSpace(setting.To))
                throw new CaughtableException(MailConstant.TO_MAIL_EMPTY_ERROR);

            else if (string.IsNullOrEmpty(setting.Subject))
                throw new CaughtableException(MailConstant.SUBJECT_EMPTY_ERROR);

            else if (string.IsNullOrEmpty(setting.Body))
                throw new CaughtableException(MailConstant.BODY_EMPTY_ERROR);

            else if (!string.IsNullOrWhiteSpace(setting.To))
                try
                {
                    new MailAddress(setting.To);
                }
                catch (FormatException)
                {
                    throw new CaughtableException(MailConstant.TO_MAIL_INVALID_ERROR);
                }

            return new ValidateMailResult(); ;
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
            if (_provider == null)
            {
                _provider = provider;
            }
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
