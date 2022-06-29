using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SWETeam.Common.Libraries
{
    /// <summary>
    /// Các phương thức mở rộng
    /// </summary>
    public static class ExtensionMethod
    {
        #region Dictionary
        /// <summary>
        /// Thêm mới nếu chưa tồn tại key, otherwise cập nhật value
        /// CreatedBy: nvcuong (01/05/2022)
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void AddOrUpdate(this Dictionary<string, object> dictionary, string key, object value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Rename key của Dictionary
        /// CreatedBy: nvcuong (16/05/2022)
        /// </summary>
        public static void RenameDictionaryKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey fromKey, TKey toKey)
        {
            TValue value = dict[fromKey];
            dict.Remove(fromKey);
            dict[toKey] = value;
        }
        #endregion

        #region String
        /// <summary>
        /// Chuyển camel case sang snake case
        /// CreatedBy: nvcuong (02/05/2022)
        /// </summary>
        /// <param name="input">CamelCase</param>
        /// <returns>SnakkeCase</returns>
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
        }

        /// <summary>
        /// Chuyển camel case sang snake case lower
        /// CreatedBy: nvcuong (02/05/2022)
        /// </summary>
        /// <param name="input">CamelCase</param>
        /// <returns>SnakeCase</returns>
        public static string ToSnakeCaseLower(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString().ToLower() : x.ToString())).ToLower();
        }

        /// <summary>
        /// Convert string sang mã hóa MD5
        /// CreatedBy: nvcuong (01/05/2022)
        /// </summary>
        public static string ToMD5(this string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Base64 encode
        /// CreatedBy: nvcuong (01/05/2022)
        /// </summary>
        public static string ToBase64Encode(this string plainText)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException("");
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Base64 Decode
        /// CreatedBy: nvcuong (01/05/2022)
        /// </summary>
        public static string ToBase64Decode(this string base64EncodedData)
        {
            if (base64EncodedData == null)
            {
                throw new ArgumentNullException("");
            }
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        #region Object
        /// <summary>
        /// Convert object to dictionary
        /// CreatedBy: nvcuong (16/05/2022)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null)
            {
                throw new Exception("Object cannot be null");
            }

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties().Where(item => item.GetIndexParameters().Length == 0);
            foreach (PropertyInfo prop in properties)
            {
                dictionary.Add(prop.Name, prop.GetValue(obj));
            }

            return dictionary;
        }



        /// <summary>
        /// Downcast parent class to child class
        /// CreatedBy: nvcuong (02/05/2022)
        /// </summary>
        public static void Downcast<T>(this object parentInstance, out T childInstance)
        {
            var serializedParent = JsonConvert.SerializeObject(parentInstance);
            childInstance = JsonConvert.DeserializeObject<T>(serializedParent);
        }
        #endregion

        #region T
        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialization method. NOTE: Private members are not cloned using this method.
        /// CreatedBy: nvcuong (02/05/2022)
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default;

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            JsonSerializerSettings deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
        #endregion
    }
}
