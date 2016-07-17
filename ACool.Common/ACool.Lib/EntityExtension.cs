using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public static class EntityExtension
    {
        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> entites)
        {
            DataTable dt = new DataTable(typeof(T).Name);

            foreach (var p in typeof(T).GetProperties())
            {
                dt.Columns.Add(p.Name, p.PropertyType);
            }

            foreach (T entity in entites)
            {
                DataRow dr = dt.NewRow();

                foreach (var p in typeof(T).GetProperties())
                {
                    dr[p.Name] = p.GetValue(entity);
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static T ToEntity<T>(this DataRow dr)
        {
            T data = Activator.CreateInstance<T>();

            foreach (PropertyInfo p in typeof(T).GetProperties())
            {
                object value = dr[p.Name];

                p.SetValue(data, Convert.ChangeType(value, p.PropertyType));
            }

            return data;
        }

        public static IEnumerable<T> ToEntities<T>(DataTable dt)
        {
            return dt.Rows.Cast<DataRow>().Select(x => x.ToEntity<T>());
        }

        public static IEnumerable<object> GetValues<T>(this T entity)
        {
            IEnumerable<object> values = typeof(T).GetProperties().Select(p => Convert.ChangeType(p.GetValue(entity), p.PropertyType));

            return values;
        }

    }
}
