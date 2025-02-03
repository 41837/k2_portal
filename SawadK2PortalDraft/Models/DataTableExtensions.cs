using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SawadK2PortalDraft.Models
{
    public static  class DataTableExtensions
    {
        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties();

            foreach (DataRow row in dataTable.Rows)
            {
                var item = new T();
                foreach (var property in properties)
                {
                    if (dataTable.Columns.Contains(property.Name))
                    {
                        var value = row[property.Name];
                        if (value != DBNull.Value)
                        {
                            property.SetValue(item, Convert.ChangeType(value, property.PropertyType), null);
                        }
                    }
                }
                list.Add(item);
            }
            return list;
        }
    }
}