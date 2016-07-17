using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt.ExpressionExt
{
    public class CompareCondtionBox : ExpBox
    {
        private MemberExpression MemberExp { get; set; }
        private ExpressionType CompareType { get; set; }
        private object Value { get; set; }

        public CompareCondtionBox(MemberExpression memberExp, ExpressionType CompareType, object value)
        {
            this.MemberExp = memberExp;

            this.CompareType = CompareType;

            this.Value = value;
        }

        public override string ToSQL(ICollection<KeyValuePair<string, object>> para)
        {
            string result = string.Empty;

            string memberParamter = ExpressionUtility.ToTableFieldString(MemberExp);

            if (this.Value != null)
            {
                string key = "@para" + para.Count.ToString();

                para.Add(new KeyValuePair<string, object>(key, this.Value));

                result = $"{memberParamter} {GetCompareString()} {key}";
            }
            else
            {
                result = $"{memberParamter} {GetCompareStringWhenNull()} {"NULL"}";
            }


            return result;
        }

        public string GetCompareStringWhenNull()
        {
            string result = string.Empty;

            switch (this.CompareType)
            {
                case ExpressionType.Equal:
                    result = "IS";
                    break;
                case ExpressionType.NotEqual:
                    result = "IS NOT";
                    break;
            }

            return result;
        }

        public string GetCompareString()
        {
            string result = string.Empty;

            switch (this.CompareType)
            {
                case ExpressionType.Equal:
                    result = "=";
                    break;
                case ExpressionType.NotEqual:
                    result = "!=";
                    break;
                case ExpressionType.GreaterThan:
                    result = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    result = ">=";
                    break;
                case ExpressionType.LessThan:
                    result = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    result = "<=";
                    break;
            }

            return result;
        }

    }
}
