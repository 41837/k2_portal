using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SawadK2PortalDraft.Models
{
    public static class DataTableHelper
    {
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            // Create a new DataTable.
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get the properties of the type.
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Create columns for each property.
            foreach (PropertyInfo prop in props)
            {
                // Add a column to the DataTable with the name of the property.
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Populate rows with data.
            foreach (T item in list)
            {
                DataRow row = dataTable.NewRow();
                foreach (PropertyInfo prop in props)
                {
                    // Set the value of the cell to the value of the property.
                    row[prop.Name] = prop.GetValue(item, null);
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}