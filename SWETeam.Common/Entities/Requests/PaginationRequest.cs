using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Entities
{
    public class PaginationRequest
    {
        private int _pageIndex;
        private int _pageSize;

        /// <summary>
        /// Trang cần lấy dữ liệu
        /// </summary>
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }

            set
            {
                if (value <= 0 || value > 500)
                {
                    throw new Exception("PageIndex min is 1 and max is 500");
                }
                _pageIndex = value;
            }
        }

        /// <summary>
        /// Số bản ghi trên 1 trang
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                if (value <= 0 || value > 2500000)
                {
                    throw new Exception("PageSize min is 1 and max is 2,500,000");
                }
                _pageSize = value;
            }
        }

        /// <summary>
        /// Columns cần lấy dữ liệu
        /// </summary>
        public string Columns { get; set; }
    }
}
