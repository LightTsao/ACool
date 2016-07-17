using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public static class DataReaderExt
    {
        public static T ToEntity<T>(this IDataReader reader)
        {
            T entity = (T)Activator.CreateInstance(typeof(T));

            for (int i = 0; i < reader.FieldCount; i++)
            {
                PropertyInfo property = typeof(T).GetProperty(reader.GetName(i));

                if (property != null)
                {
                    object value = reader.GetValue(i);

                    property.SetValue(entity, Convert.ChangeType(value, property.PropertyType));
                }
            }

            return entity;
        }
        public static List<T> ToEntities<T>(this IDataReader reader)
        {
            List<T> result = new List<T>();

            while (reader.Read())
            {
                T entity = ToEntity<T>(reader);

                result.Add(entity);
            }

            return result;
        }
    }
}
