using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class JsonUtility
    {
        private static DataTable ConvertToDataTable(JArray array)
        {

            DataTable dt = null;


            JObject firstItem = (array.FirstOrDefault() as JObject);

            if (firstItem != null)
            {
                List<string> ColumnNames = array.SelectMany(x => ((JObject)x).Properties().Select(y => y.Name)).Distinct().ToList();

                if (ColumnNames.All(x => !string.IsNullOrEmpty(x.Trim())))
                {
                    if (firstItem.Values().All(x => x is JValue))
                    {
                        dt = JsonConvert.DeserializeObject<DataTable>(array.ToString());
                    }
                    else
                    {
                        dt = new DataTable();

                        dt.Columns.AddRange(ColumnNames.Select(x => new DataColumn(x)).ToArray());

                        foreach (JToken value in array)
                        {
                            DataRow dr = dt.NewRow();

                            foreach (JToken va in value.Children())
                            {
                                if (va is JProperty)
                                {
                                    string Jkey = ((JProperty)va).Name;

                                    string Jvalue = (((JProperty)va).Value).ToString();

                                    dr[Jkey] = Jvalue;
                                }
                                else
                                {

                                }
                            }

                            dt.Rows.Add(dr);
                        }

                    }
                }
            }
            else
            {
                return OnlyValue(string.Empty);
            }


            return dt;
        }

        private static DataTable OnlyValue(object value, string TableName = "Value")
        {
            DataTable dt = new DataTable(TableName);

            dt.Columns.Add(TableName);

            dt.LoadDataRow(new object[] { value }, false);

            return dt;
        }

        public static DataTable ConvertToDataTable(object value, string TableName = "Value")
        {
            if (value is JArray)
            {
                return ConvertToDataTable(value as JArray);
            }

            return OnlyValue(value, TableName);
        }

        public static DataTable ConvertToDataTable(string JsonString)
        {
            if (isJArray(JsonString))
            {
                return ConvertToDataTable(JArray.Parse(JsonString));
            }
            else
            {
                return ConvertToDataTable(JObject.Parse(JsonString));
            }
        }

        public static object ConvertToJObject(string text)
        {
            JObject result = JObject.Parse(text);

            return result;
        }


        public static bool isJObject(string text)
        {
            try
            {
                JObject result = JObject.Parse(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool isJObject(object obj)
        {
            return obj is JObject;
        }

        public static bool isJArray(string text)
        {
            try
            {
                JArray result = JArray.Parse(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Dictionary<string, object> CastJObject(object obj)
        {
            return (obj as JObject).ToObject<Dictionary<string, object>>();
        }

        public static bool HasJTokenValue(object value)
        {
            if ((value as JToken) == null)
            {
                return false;
            }

            return (value as JToken).HasValues;
        }

   


    }
}
