using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt.ExpressionExt
{
    public class LogicBox : ExpBox
    {
        public ExpressionType NodeType { get; set; }
        public ExpBox Left { get; set; }
        public ExpBox Right { get; set; }

        public LogicBox(BinaryExpression binaryExp)
        {
            this.Left = ExpressionUtility.GetBox(binaryExp.Left);

            this.Right = ExpressionUtility.GetBox(binaryExp.Right);

            this.NodeType = binaryExp.NodeType;
        }
        public override string ToSQL(ICollection<KeyValuePair<string, object>> para)
        {
            string leftExpString = this.Left.ToSQL(para);

            string rightExpString = this.Right.ToSQL(para);

            return $"{leftExpString} {GetLogicString()} {rightExpString}";
        }

        public string GetLogicString()
        {
            string result = string.Empty;

            switch (this.NodeType)
            {
                case ExpressionType.AndAlso:
                    result = "AND";
                    break;
                case ExpressionType.OrElse:
                    result = "OR";
                    break;
            }

            return result;
        }
    }
}
