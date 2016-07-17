using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public static class DataTableExtension
    {
        public static DataTable Filter(this DataTable dtSource, string filterText)
        {
            try
            {
                dtSource.DefaultView.RowFilter = filterText;

                return dtSource.DefaultView.ToTable();
            }
            catch
            {
                return dtSource.Clone();
            }
        }
        public static DataTable R2C(this DataTable dtSource, List<string> FixColumn, List<string> ConvertColumn)
        {
            List<string> TotalColumns = dtSource.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            List<string> ValueColumn = TotalColumns.Where(x => !FixColumn.Concat(ConvertColumn).Contains(x)).ToList();


            // col

            DataTable dtTarget = dtSource.DefaultView.ToTable(true, FixColumn.ToArray());

            if (ConvertColumn.Count > 0)
            {


                List<DataColumn> ConvertValueColumns = new List<DataColumn>();

                Dictionary<DataColumn, string> dicValueColumn = new Dictionary<DataColumn, string>();

                Dictionary<DataColumn, Dictionary<string, string>> dicdicExValue = new Dictionary<DataColumn, Dictionary<string, string>>();


                foreach (string ExValue in ValueColumn)
                {
                    foreach (DataRow SourceRow in dtSource.DefaultView.ToTable(true, ConvertColumn.ToArray()).AsEnumerable())
                    {
                        Dictionary<string, string> dicExValue = ConvertColumn.ToDictionary(x => x, x => Convert.ToString(SourceRow[x]));

                        string ExColumnConvert = string.Join(DataValueFormat.JoinConvertColumns, dicExValue.Keys.Select(x => string.Format(DataValueFormat.ConvertColumnFormart, x, dicExValue[x])).ToArray());

                        DataColumn dc = new DataColumn(ExColumnConvert + DataValueFormat.JoinBetweenConvertColumnAndValueColumn + ExValue);

                        dtTarget.Columns.Add(dc);

                        ConvertValueColumns.Add(dc);

                        //紀錄這個欄位對應的 ValueColumn Name (只有一個)
                        dicValueColumn.Add(dc, ExValue);

                        //紀錄這個欄位對應的 ConvertColumn Name & Value (會有多個)
                        dicdicExValue.Add(dc, dicExValue);

                    }
                }


                // row

                foreach (DataRow dr in dtTarget.Rows)
                {
                    foreach (DataColumn dc in ConvertValueColumns)
                    {
                        string formatColToValue = "[{0}]='{1}'";

                        //FixColumn Distinct
                        List<string> rowGroup = FixColumn.Select(col => string.Format(formatColToValue, col, dr[col])).ToList();

                        //ConvertColumn Distinct
                        List<string> colGroup = dicdicExValue[dc].Select(x => string.Format(formatColToValue, x.Key, x.Value)).ToList();

                        string SourceKey = string.Join(" AND ", rowGroup.Concat(colGroup));

                        List<object> TargetValues = dtSource.Select(SourceKey).Select(row => row[dicValueColumn[dc]]).Where(x => !string.IsNullOrEmpty(Convert.ToString(x))).ToList();

                        if (TargetValues.Count() == 0)
                        {
                            //None
                        }
                        else if (TargetValues.Count() == 1)
                        {
                            //First
                            dr[dc] = TargetValues[0];
                        }
                        else if (TargetValues.Count() > 1)
                        {
                            //Summary
                            dr[dc] = string.Join(DataValueFormat.JoinDataValue, TargetValues);
                        }
                    }
                }

            }

            return dtTarget;
        }
        public static List<object> GetDistinctData(this DataTable dtSource, string columnName)
        {
            return dtSource.DefaultView.ToTable(true, columnName).Rows.Cast<DataRow>().Select(x => x[columnName]).OrderBy(x => x).ToList();
        }

        public static DataTable C2R(this DataTable TempDataSource)
        {
            string[] formats = DataValueFormat.ConvertColumnFormart.Split(new string[] { DataValueFormat.XColumn, DataValueFormat.XValue }, StringSplitOptions.None);

            List<string> lstNewSourceColumn = TempDataSource.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            List<string> split = new List<string>();

            List<string> ConvertColumns = new List<string>();

            List<string> ValueColumns = new List<string>();


            foreach (string col in lstNewSourceColumn)
            {
                if (col.IndexOf(DataValueFormat.JoinBetweenConvertColumnAndValueColumn) >= 0)
                {
                    string[] columnSplit = col.Split(new string[] { DataValueFormat.JoinBetweenConvertColumnAndValueColumn }, StringSplitOptions.RemoveEmptyEntries);

                    if (columnSplit.Count() == 2)
                    {
                        foreach (string con in columnSplit[0].Split(new string[] { DataValueFormat.JoinConvertColumns }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string column = con.Replace(formats[0], "").Replace(formats[2], "").Split(new string[] { formats[1] }, StringSplitOptions.None)[0];

                            string value = con.Replace(formats[0], "").Replace(formats[2], "").Split(new string[] { formats[1] }, StringSplitOptions.None)[1];

                            ConvertColumns.Add(column);
                        }

                        ValueColumns.Add(columnSplit[1]);

                        split.Add(col);
                    }
                }
            }

            ConvertColumns = ConvertColumns.Distinct().ToList();

            ValueColumns = ValueColumns.Distinct().ToList();

            DataTable dtTarget = new DataTable();

            foreach (string col in lstNewSourceColumn.Except(split))
            {
                dtTarget.Columns.Add(col);
            }

            foreach (string col in ConvertColumns)
            {
                dtTarget.Columns.Add(col);
            }

            foreach (string col in ValueColumns)
            {
                dtTarget.Columns.Add(col);
            }

            foreach (DataRow dr in TempDataSource.Rows)
            {
                Dictionary<string, object> dicValue = lstNewSourceColumn.Except(split).ToDictionary(x => x, x => dr[x]);

                foreach (string col in split)
                {
                    string[] columnSplit = col.Split(new string[] { DataValueFormat.JoinBetweenConvertColumnAndValueColumn }, StringSplitOptions.RemoveEmptyEntries);

                    string convertCol = columnSplit[0];

                    string valueCol = columnSplit[1];

                    string[] values = Convert.ToString(dr[col]).Split(new string[] { DataValueFormat.JoinDataValue }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string value in values)
                    {
                        DataRow drT = dtTarget.NewRow();

                        foreach (KeyValuePair<string, object> item in dicValue)
                        {
                            drT[item.Key] = item.Value;
                        }

                        foreach (string con in convertCol.Split(new string[] { DataValueFormat.JoinConvertColumns }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string column = con.Replace(formats[0], "").Replace(formats[2], "").Split(new string[] { formats[1] }, StringSplitOptions.None)[0];

                            string valueA = con.Replace(formats[0], "").Replace(formats[2], "").Split(new string[] { formats[1] }, StringSplitOptions.None)[1];

                            drT[column] = valueA;
                        }


                        drT[valueCol] = value;

                        dtTarget.Rows.Add(drT);
                    }
                }
            }




            return dtTarget;
        }
    }
}
