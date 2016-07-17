using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt
{
    public class SqlServerInsertExt
    {
        public static SQLBox ToInsertSqlBox<T>(params T[] entities)
        {
            List<Dictionary<string, object>> dicParas = new List<Dictionary<string, object>>();

            int max = entities.Count();

            for (int i = 0; i < max; i++)
            {
                dicParas.Add(ConvertToParameter(entities.ElementAt(i), n => "@" + n + i.ToString()));
            }

            string sql = ConvertToSql<T>(dicParas.Select(x => x.Keys).ToArray());

            return new SQLBox(sql, dicParas.SelectMany(x => x).ToDictionary(x => x.Key, x => x.Value));
        }
        public static List<SQLBox> ToInsertSqlBoxes<T>(IEnumerable<T> entities)
        {
            //Sql Server Max Parameter Count : 2100 -1
            int SqlBoxIncludeCount = 2100 / typeof(T).GetProperties().Count() - 1;

            List<T> lstEntities = entities.ToList();

            IEnumerable<List<T>> GroupEntities = lstEntities.GroupBy(x => lstEntities.IndexOf(x) / SqlBoxIncludeCount).Select(x => x.ToList());

            return GroupEntities.Select(x => ToInsertSqlBox(x.ToArray())).ToList();
        }
        public static string ConvertToSql<T>(params IEnumerable<string>[] ParaArray)
        {
            IEnumerable<string> fields = typeof(T).GetProperties().Select(x => $"[{ x.Name }]");

            IEnumerable<string> values = ParaArray.Select(paras => string.Join(",", paras));

            return $"INSERT INTO [{typeof(T).Name}] ({string.Join(",", fields)}) VALUES {string.Join(",", values.Select(x => $"({x})"))}";
        }
        public static Dictionary<string, object> ConvertToParameter<T>(T entity, Func<string, string> NameRule)
        {
            return typeof(T).GetProperties().ToDictionary(x => NameRule(x.Name), x =>
            {
                object value = x.GetValue(entity);

                if (value == null)
                {
                    //Check This Field is Allow Null

                    bool isNullable = x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

                    bool isString = x.PropertyType == typeof(string);

                    if (isNullable || isString)
                    {
                        value = DBNull.Value;
                    }
                }
                else
                {
                    if (x.PropertyType == typeof(DateTime))
                    {
                        value = ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                }

                return value;
            });
        }
    }
}
