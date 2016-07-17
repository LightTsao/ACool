using ACool.SqlServerExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        Dictionary<int, string> dic = new Dictionary<int, string>()
        {
            [2] = "現金",
            [3] = "花旗信用卡",
            [4] = "中小企銀",
            [5] = "郵局",
            [6] = "中國信託",
            [7] = "ICASH",
            [8] = "隨行卡",
            [9] = "悠遊卡",


        };

        private DateTime StartTime = new DateTime(2016, 1, 1);

        private IDBConnectHelper helper = Factory.GetDBHelper();
        private CheckItem Create20160101CKI(string name, int money)
        {
            CheckItem cki = new CheckItem();

            cki.CheckItemId = Guid.NewGuid();

            cki.EffectiveDate = StartTime;

            cki.CostItem = name;
            cki.MoneySum = money;

            return cki;
        }

        private void Add20160101CKI()
        {

            helper.Delete<CheckItem>().Execute(true);

            List<CheckItem> lstCki = new List<CheckItem>();

            lstCki.Add(Create20160101CKI("現金", 2375));
            lstCki.Add(Create20160101CKI("花旗信用卡", -1504));
            lstCki.Add(Create20160101CKI("中小企銀", 345335));
            lstCki.Add(Create20160101CKI("郵局", 915));
            lstCki.Add(Create20160101CKI("中國信託", 69509));
            lstCki.Add(Create20160101CKI("ICASH", 0));
            lstCki.Add(Create20160101CKI("隨行卡", 0));
            lstCki.Add(Create20160101CKI("悠遊卡", 0));


            helper.InsertBatch(lstCki, true);

            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Add20160101CKI();

            Query();
        }

        private void Query()
        {
            string sql1 = "select * from TradeItem where EffectiveDate >= @StartTime and EffectiveDate < @EndTime";

            string sql2 = "select * from CheckItem where EffectiveDate >= @StartTime and EffectiveDate < @EndTime";

            Dictionary<string, object> para = new Dictionary<string, object>() { ["@StartTime"] = StartTime, [@"EndTime"] = DateTime.Now };


            dataGridView1.DataSource = helper.QueryDataTable(sql1, para);

            dataGridView2.DataSource = helper.QueryDataTable(sql2, para);

            DataTable dt = new DataTable();

            dt.Columns.Add("Date", typeof(DateTime));

            foreach (string a in dic.Values)
            {
                dt.Columns.Add(a, typeof(Int32));
            }

            dt.Columns.Add("Sum", typeof(Int32));

            List<TradeItem> ti = helper.Query<TradeItem>().Where(x => x.EffectiveDate >= StartTime).ToEnities();

            List<CheckItem> chi = helper.Query<CheckItem>(sql2, para).ToList();

            Dictionary<string, int> ddd = chi.ToDictionary(x => x.CostItem, x => x.MoneySum);

            foreach (var x in ti.GroupBy(x => x.EffectiveDate).OrderBy(x => x.Key))
            {
                DataRow dr = dt.NewRow();

                dr["Date"] = x.Key;


                foreach (var y in x.GroupBy(y => y.CostItem))
                {
                    string costItem = y.Key;

                    int costSum = y.Sum(z => z.CostMoney);

                    ddd[costItem] += costSum;    
                }


                foreach (string key in ddd.Keys)
                {
                    dr[key] = ddd[key];
                }

                dr["Sum"] = ddd.Sum(z => z.Value);

                dt.Rows.Add(dr);
            }

            dataGridView3.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;


            string[] ss = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<List<string>> aa = ss.Select(x => x.Split(new string[] { "\t" }, StringSplitOptions.None).ToList()).ToList();



            DateTime dt = DateTime.MinValue;

            List<TradeItem> result = new List<TradeItem>();

            foreach (List<string> a in aa)
            {
                for (int i = 0; i < a.Count; i++)
                {
                    string w = a[i];

                    if (!string.IsNullOrEmpty(w.Trim()))
                    {
                        if (i == 0)
                        {
                            bool b = DateTime.TryParse(w, out dt);
                        }
                        else
                        {
                            if (dt == DateTime.MinValue || i == 1)
                            {
                                continue;
                            }


                            TradeItem ti = new TradeItem();

                            ti.TradeItemId = Guid.NewGuid();

                            ti.CostMoney = Convert.ToInt32(w);

                            ti.Category = null;

                            ti.CostItem = dic[i];

                            ti.Description = a[1];

                            ti.EffectiveDate = dt;

                            result.Add(ti);

                        }
                    }

                }
            }
            helper.InsertBatch(result, true);

            Query();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }
    }
}
