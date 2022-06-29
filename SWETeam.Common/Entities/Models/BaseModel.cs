using SWETeam.Common.Libraries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace SWETeam.Common.Entities
{
    public class BaseModel
    {
        #region Common Properties
        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Người tạo bản ghi
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Ngày sửa bản ghi
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Người sửa bản ghi
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Đã xóa bản ghi hay chưa?
        /// </summary>
        public bool IsDeleted { get; set; }

        #endregion

        #region Indexer

        /// <summary>
        /// Indexer get value hoặc set value với propertyName
        /// CreatedBy: nvcuong2 (01/05/2022)
        /// </summary>
        [JsonIgnore]
        [Ignore]
        public object this[string propertyName]
        {
            get
            {
                PropertyInfo prop = this.GetType().GetProperty(propertyName);
                if (prop == null)
                {
                    throw new Exception(string.Format("Property {0} does not exists in {1}", propertyName, this.GetType().Name));
                }
                return prop.GetValue(this);
            }

            set
            {
                PropertyInfo prop = this.GetType().GetProperty(propertyName);
                if (prop == null)
                {
                    throw new Exception(string.Format("Property {0} does not exists in {1}", propertyName, this.GetType().Name));
                }
                prop.SetValue(this, value);
            }
        }

        #endregion

        #region Sql Command


        #endregion

    }
}
