using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Framework.Extension
{
    public class QueryFilter
    {
        public int PageIndex
        {
            get;
            set;
        }
        
        public int PageSize
        {
            get;
            set;
        }

        public int Start
        {
            get
            {
                return (PageIndex > 0 ? PageIndex - 1 : 0) * PageSize;
            }
        }
        
        public string SortFields
        {
            get;
            set;
        }

        /// <summary>
        /// 从dataTable传过来的请求使用
        /// </summary>
        /// <param name="request"></param>
        public void BuildQueryFilter(DataTableQueryRequest request)
        {
            this.PageIndex = request.start/ request.length;
            this.PageSize = request.length;
            if (request.order != null && request.order.Count > 0)
            {
                this.SortFields = $"{request.columns[request.order.FirstOrDefault().column].name} {request.order.FirstOrDefault().dir}";
            }
        }
    }

    public class DataTableQueryRequest
    {
        public int draw { get; set; }

        public List<DataTableColumn> columns { get; set; }

        public List<DataTableOrder> order { get; set; }

        public int start { get; set; }

        public int length { get; set; }
    }

    public class DataTableColumn
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool orderable { get; set; }
    }

    public class DataTableOrder
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}
