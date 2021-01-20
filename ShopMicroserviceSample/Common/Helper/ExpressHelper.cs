using Common.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Helper
{
    /// <summary>
    /// 表达式操作类
    /// </summary>
    public static class ExpressHelper
    {
        /// <summary>
        /// 赋值
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <param name="expression">u.Name="yang" &amp;&amp; u.RealName="杨天华"</param>
        /// <param name="replaceName1">将name1的名称替换为T1的类名</param>
        /// <returns></returns>
        public static string Set<T1>(Expression<Func<T1, bool>> expression, string replaceName1 = null)
        {
            var result = SqlWhere(expression).Replace(" and ", ",").ReplaceName<T1>(replaceName1);

            return result;
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <typeparam name="T1">对象1</typeparam>
        /// <typeparam name="T2">对象2</typeparam>
        /// <param name="expression">u.Name="yang" &amp;&amp; u.RealName=d.RealName</param>
        /// <param name="replaceName1">将name1的名称替换为T1的类名</param>
        /// <param name="replaceName2">将name1的名称替换为T2的类名</param>
        /// <returns></returns>
        public static string Set<T1, T2>(Expression<Func<T1, T2, bool>> expression, string replaceName1 = null, string replaceName2 = null)
        {
            var result = SqlWhere(expression).ReplaceName<T1>(replaceName1).ReplaceName<T2>(replaceName2);

            return result;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <param name="expression">u.Name="yang" &amp;&amp; u.RealName="杨天华"</param>
        /// <param name="replaceName1">将name1的名称替换为T1的类名</param>
        /// <returns></returns>
        public static string Where<T1>(Expression<Func<T1, bool>> expression, string replaceName1 = null)
        {
            var result = SqlWhere(expression).ReplaceName<T1>(replaceName1);
            return result;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T1">对象</typeparam>
        /// <typeparam name="T2">对象2</typeparam>
        /// <param name="expression">u.Name="yang" &amp;&amp; u.RealName="杨天华"</param>
        /// <param name="replaceName1">将name1的名称替换为T1的类名</param>
        /// <param name="replaceName2">将name1的名称替换为T2的类名</param>
        /// <returns></returns>
        public static string Where<T1, T2>(Expression<Func<T1, T2, bool>> expression, string replaceName1 = null, string replaceName2 = null)
        {
            var result = SqlWhere(expression).ReplaceName<T1>(replaceName1).ReplaceName<T2>(replaceName2);
            return result;
        }

        private static string ReplaceName<T>(this string result, string replaceName)
        {
            if (!string.IsNullOrEmpty(replaceName))
            {
                var className = ObjectUtil.GetTableName<T>(true);
                result = result.Replace($"{replaceName}.", $"{ className}.");
            }
            return result;
        }

        /// <summary>
        /// 表达式转换为SQL
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static string SqlWhere(Expression expression)
        {
            var sb = new StringBuilder();
            ExpressResolve(expression, sb);
            return sb.Replace("= null", "is null").Replace("<> null", "is not null").ToString();
        }

        private static void ExpressResolve(Expression expression, StringBuilder sb)
        {
            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                expression = lambda.Body;
                ExpressResolve(expression, sb);
            }
            else if (expression is BinaryExpression)
            {
                BinaryExpression binaryExpress = expression as BinaryExpression;
                var nodeType = binaryExpress.NodeType;
                var left = binaryExpress.Left;
                var right = binaryExpress.Right;

                if (nodeType.Equals(ExpressionType.OrElse))
                {
                    sb.Append("(");
                    ExpressResolve(left, sb);
                    sb.AppendFormat(") {0} (", GetOperator(nodeType));
                    ExpressResolve(right, sb);
                    sb.Append(")");
                }
                else if (nodeType.Equals(ExpressionType.AndAlso))
                {
                    ExpressResolve(left, sb);
                    sb.AppendFormat(" {0} ", GetOperator(nodeType));
                    ExpressResolve(right, sb);
                }
                else
                {
                    sb.AppendFormat("{0}", Splicing(left, right, nodeType));
                }
            }
            else if (expression is MethodCallExpression)
            {
                MethodCallExpression methodCall = expression as MethodCallExpression;
                if (methodCall.Method.Name == "Contains")
                {
                    var @object = methodCall.Object;
                    if (@object.ToString().Contains("("))
                    {
                        var value = GetValue(@object);

                        if (value.Value is IList list1)
                        {
                            var type = list1[0].GetType().Name;
                            var list = list1.Cast<object>().Select(o => o.ToString()).ToList();
                            if (type == "String" || type == "DateTime")
                            {
                                sb.AppendFormat("{0} in ({1})", methodCall.Arguments[0], list.JoinToString("'"));
                            }
                            else
                            {
                                sb.AppendFormat("{0} in ({1})", methodCall.Arguments[0], list.JoinToString());
                            }
                        }
                    }
                    else
                    {
                        var value = methodCall.Arguments[0];
                        if (value.ToString().Contains("("))
                        {
                            value = GetValue(value);
                        }
                        if (value != null)
                        {
                            sb.AppendFormat("{0} like '%{1}%'", methodCall.Object, value.ToString().Replace("\"", ""));
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Format("不支持此种表达式：{0}", methodCall));
                }
            }
            else
            {
                throw new Exception(string.Format("不支持此种表达式：{0}", expression));
            }
        }

        /// <summary>
        /// 左边右边符号拼接
        /// </summary>
        /// <param name="left">左边表达式</param>
        /// <param name="right">右边表达式</param>
        /// <param name="type">表达式类型</param>
        /// <returns></returns>
        private static string Splicing(Expression left, Expression right, ExpressionType type)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2}", ResolveValue(left), GetOperator(type), ResolveValue(right));

            /*
            //var l = ResolveValue(left);
            //var r = ResolveValue(right);

            if (left.ToString().Contains("("))
            {
                if (left.ToString().StartsWith("Convert("))
                {
                    sb.Append(left.ToString().Replace("Convert(", "").Replace(")", ""));
                }
                else
                {
                    var value = GetValue(left);
                    sb.Append(value);
                }
            }
            else
            {
                sb.Append(left);
            }

            sb.AppendFormat(" {0} ", GetOperator(type));

            if (right.ToString().Contains("(") || right.ToString().Contains("DateTime"))
            {
                var value = GetValue(right);

                if (value != null && value.Value != null && value.Value.GetType().Name == "DateTime")
                {
                    sb.AppendFormat("'{0}'", ((DateTime)value.Value).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    sb.Append(FormatValue(value));
                }
            }
            else
            {
                sb.Append(FormatValue(right));
            }
            */
            return sb.ToString();
        }

        private static string ResolveValue(Expression expression)
        {
            var sb = new StringBuilder();

            if (expression is BinaryExpression)
            {
                BinaryExpression binaryExpress = expression as BinaryExpression;
                var nodeType = binaryExpress.NodeType;
                var left = binaryExpress.Left;
                var right = binaryExpress.Right;
                sb.AppendFormat("({0}) {1} ({2})", ResolveValue(left), GetOperator(nodeType), ResolveValue(right));
            }
            else
            {
                if (expression.ToString().Contains("(") || expression.ToString().Contains("DateTime"))
                {
                    if (expression.ToString().StartsWith("Convert("))
                    {
                        sb.Append(expression.ToString().Replace("Convert(", "").Replace(")", ""));
                    }
                    else
                    {
                        var value = GetValue(expression);

                        if (value != null && value.Value != null && value.Value.GetType().Name == "DateTime")
                        {
                            sb.AppendFormat("'{0}'", ((DateTime)value.Value).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            sb.Append(FormatValue(value));
                        }
                    }
                }
                else
                {
                    sb.Append(FormatValue(expression));
                }
            }

            return sb.ToString();
        }

        private static string FormatValue(Expression value)
        {
            if (value.NodeType == ExpressionType.Constant)
            {
                if (value.ToString().Contains("("))
                    return value.ToString().Replace("\"", "");
                else
                    return value.ToString().Replace("\"", "'");
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 动态获得变量值
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private static ConstantExpression GetValue(Expression expression)
        {
            var body = expression.ToString();
            if (body.Contains(".Floor") || body.Contains(".Sum"))
            {
                body=new Regex(@"value\(.+?\)\.", RegexOptions.IgnoreCase).Replace(body, "");
                return Expression.Constant(body);
            }

            LambdaExpression lambda = Expression.Lambda(expression);
            try
            {
                Delegate fn = lambda.Compile();
                ConstantExpression value = Expression.Constant(fn.DynamicInvoke(null), expression.Type);
                return value;
            }
            catch
            {
                return Expression.Constant(lambda.Body.ToString());
            }
        }

        /// <summary>
        /// 获取SQL查询操作符
        /// </summary>
        /// <param name="expressionType">表达式类型</param>
        /// <returns></returns>
        private static string GetOperator(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.And:
                    return "and";
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.Or:
                    return "or";
                case ExpressionType.OrElse:
                    return "or";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                    return "*";
                default:
                    throw new Exception(string.Format("不支持{0}此种运算符查找！", expressionType));
            }
        }
    }
}
