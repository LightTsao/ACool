using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DB.SQL.Extension
{
    public static class SqlServerExt
    {
        public static SQLBox ToInsertSqlBox<T>(this T entity)
        {
            IEnumerable<string> fieldNames = typeof(T).GetProperties().Select(x => x.Name);

            string _ColumnNameList = string.Join(",", fieldNames.Select(x => "[" + x + "]").ToArray());

            string _ValueList = string.Join(",", fieldNames.Select(x => "@" + x).ToArray());

            string sql = string.Format("INSERT INTO [{0}] ({1}) VALUES ({2})", typeof(T).Name, _ColumnNameList, _ValueList);

            Dictionary<string, object> parameters = entity.ConvertToParameter();

            return new SQLBox(sql, parameters);
        }
        public static List<SQLBox> ConvertToInsertSqlBoxList<T>(this IEnumerable<T> entities)
        {
            if (entities == null || entities.Count() == 0)
            {
                return null;
            }

            List<SQLBox> result = new List<SQLBox>();

            IEnumerable<string> fieldNames = typeof(T).GetProperties().Select(x => x.Name);

            int SplitCount = 2100 / fieldNames.Count() - 1;

            string _ColumnNameList = string.Join(",", fieldNames.Select(x => "[" + x + "]").ToArray());

            List<string> ValueListList = new List<string>();

            Dictionary<string, object> dicPara = new Dictionary<string, object>();

            int cnt = entities.Count();

            for (int i = 0; i < cnt; i++)
            {
                string _ValueList = string.Join(",", fieldNames.Select(x => "@" + x + i.ToString()).ToArray());

                ValueListList.Add(_ValueList);

                Dictionary<string, object> dicEntity = entities.ElementAt(i).ConvertToParameter();

                foreach (KeyValuePair<string, object> item in dicEntity)
                {
                    dicPara.Add(item.Key + i.ToString(), item.Value);
                }

                if ((i + 1) % SplitCount == 0 || (i + 1) == cnt)
                {
                    string sql = string.Format("INSERT INTO [{0}] ({1}) VALUES {2}", typeof(T).Name, _ColumnNameList, string.Join(",", ValueListList.Select(x => "(" + x + ")")));

                    result.Add(new SQLBox(sql, dicPara));

                    dicPara = new Dictionary<string, object>();

                    ValueListList.Clear();
                }

            }

            return result;
        }
        public static Dictionary<string, object> ConvertToParameter<T>(this T entity)
        {
            return typeof(T).GetProperties().ToDictionary(x => "@" + x.Name.TrimStart('@'), x =>
            {
                object value = x.GetValue(entity);

                if (x.PropertyType == typeof(DateTime))
                {
                    if (value == null)
                    {
                        value = DBNull.Value;
                    }
                    else
                    {
                        value = ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                }
                else if (x.PropertyType == typeof(Guid))
                {
                    if (value == null)
                    {
                        value = DBNull.Value;
                    }
                }
                else if (x.PropertyType == typeof(string))
                {

                }

                return value;
            });
        }
    }
}
