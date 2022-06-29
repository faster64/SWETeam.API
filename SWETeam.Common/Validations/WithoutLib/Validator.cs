using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWETeam.Common.Entities;
using SWETeam.Common.Libraries;
using SWETeam.Common.MySQL;
using System;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SWETeam.Common.Validations
{
    public class Validator : IValidator
    {
        #region Declares
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructors
        public Validator(IServiceProvider provider)
        {
            _provider = provider;
            _config = provider.GetRequiredService<IConfiguration>();
            _unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Common validate
        /// </summary>
        public ValidateResult Execute(object obj, string tableName = "")
        {
            ValidateResult result = new ValidateResult();
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                // Bỏ qua nếu có attribute Ignore
                if (prop.IsDefined(typeof(Ignore), false))
                {
                    continue;
                }

                string name = prop.Name;
                object value = prop.GetValue(obj);

                // Check required
                if (prop.IsDefined(typeof(Required), false))
                {
                    if (value == null || (value is string && value.ToString() == ""))
                    {
                        result.ValidateFields.Add(new ValidateField()
                        {
                            FieldName = name,
                            ErrorMessage = $"Property {name} không được phép để trống",
                            Code = Enumeration.ValidateCode.Required
                        });
                    }
                }

                // Check duplicate
                if (prop.IsDefined(typeof(Duplication), false) && !string.IsNullOrEmpty(tableName))
                {
                    if (CheckExists(name, value, tableName))
                    {
                        result.ValidateFields.Add(new ValidateField()
                        {
                            FieldName = name,
                            ErrorMessage = $"Property {name} đã tồn tại trên hệ thống",
                            Code = Enumeration.ValidateCode.Duplicate
                        });
                    }
                }

                // Check phone number
                if (prop.IsDefined(typeof(PhoneNumber), false) && value != null)
                {
                    if (!new Regex(RegexPattern.PHONE_PATTERN).IsMatch(value.ToString()))
                    {
                        result.ValidateFields.Add(new ValidateField()
                        {
                            FieldName = name,
                            ErrorMessage = $"SĐT không hợp lệ",
                            Code = Enumeration.ValidateCode.Invalid
                        });
                    }
                }

                // Check email
                if (prop.IsDefined(typeof(Email), false) && value != null)
                {
                    try
                    {
                        new MailAddress(value.ToString());
                    }
                    catch (Exception)
                    {
                        result.ValidateFields.Add(new ValidateField()
                        {
                            FieldName = name,
                            ErrorMessage = $"Mail không hợp lệ",
                            Code = Enumeration.ValidateCode.Invalid
                        });
                    }
                }

                // Check password
                if (prop.IsDefined(typeof(Password), false) && value != null)
                {
                    int length = value.ToString().Length;
                    if (length < 8 || length > 64)
                    {
                        result.ValidateFields.Add(new ValidateField()
                        {
                            FieldName = name,
                            ErrorMessage = "Mật khẩu phải có độ dài từ 8-64 ký tự",
                            Code = Enumeration.ValidateCode.Invalid
                        });
                    }
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Check tồn tại
        /// </summary>
        /// <returns></returns>
        private bool CheckExists(string field, object value, string tableName, object id = null)
        {
            if (id == null)
            {
                string sqlCommand = $"SELECT {field} FROM {tableName} WHERE {field} = @Value";
                return _unitOfWork.QueryFirstOrDefault(sqlCommand, new { Value = value }) != null;
            }
            else
            {
                string sqlCommand = $"SELECT {field} FROM {tableName} WHERE {field} = @Value AND Id <> @Id";
                return _unitOfWork.QueryFirstOrDefault(sqlCommand, new { Value = value, Id = id }) != null;
            }
        }
    }
}
