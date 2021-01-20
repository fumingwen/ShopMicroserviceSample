using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public class ConditionBuilderVisitor : ExpressionVisitor
    {
        public ConditionBuilderVisitor()
        {
        }

        private bool IsRight = true;
        private Stack<string> _StringStack = new Stack<string>();

        public string Condition()
        {
            string condition = string.Concat(this._StringStack.ToArray());
            this._StringStack.Clear();
            return condition;
        }

        /// <summary>
        /// 二元表达式
        /// </summary>
        /// <param name="binaryExpression"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (binaryExpression == null)
            {
                throw new ArgumentException("BinaryExpression");

            }

            IsRight = true;
            this._StringStack.Push(")");
            base.Visit(binaryExpression.Right);//解析右边
            IsRight = false;
            this._StringStack.Push($" {binaryExpression.NodeType.ToSqlOperator()} ");
            base.Visit(binaryExpression.Left);//解析左边
            this._StringStack.Push("(");
            return binaryExpression;
        }

        /// <summary>
        /// 成员表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
            {
                throw new ArgumentException("MemberExpression");
            }
            if (IsRight)
            {
                this._StringStack.Push($" @{node.Member.Name}");
            }
            else
            {
                this._StringStack.Push(node.Member.GetMappingName());
            }
            return node;
        }

        /// <summary>
        /// 常量
        /// </summary>
        /// <param name="constantExpression"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            if (constantExpression == null)

            {
                throw new ArgumentException("ConstantExpression");
            }

            this._StringStack.Push("'" + constantExpression.Value + "'");
            return constantExpression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            if (methodCall == null)
            {
                throw new ArgumentException("MethodCallExpression");
            }
            var type = methodCall.Method.MemberType;
            string format;
            switch (methodCall.Method.Name)
            {
                case "StartsWith":
                    format = "({0} LIKE {1}+'%')";
                    break;

                case "Contains":
                    if (methodCall.Method.ReflectedType.Name.Contains("List"))
                    {
                        format = "({0} IN ({1}))";
                    }
                    else
                    {
                        format = "({0} LIKE '%'+{1}+'%')";
                    }
                    break;

                case "EndsWith":
                    format = "({0} LIKE '%'+{1})";
                    break; 

                default:
                    throw new NotSupportedException(methodCall.NodeType + " is not supported!");
            }
            IsRight = false;
            this.Visit(methodCall.Object);
            IsRight = true;
            this.Visit(methodCall.Arguments[0]);
            string right = this._StringStack.Pop();
            string left = this._StringStack.Pop();
            this._StringStack.Push(String.Format(format, left, right));

            return methodCall;
        }
    }
}
