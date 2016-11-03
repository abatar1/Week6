using System;
using System.Linq.Expressions;

namespace Week6
{
    public static class ExpressionDifferentiateExtension
    {
        private static Expression AddDifferentiateRule(Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
            {
               return Expression.Empty();
            }
            else if (exp.NodeType == ExpressionType.Parameter)
            {
                return Expression.Constant(1);
            }
            else
            {
                var a = Expression.Lambda<Func<double, double>>(exp);
                return Expression.Lambda<Func<double, double>>(exp as Expression<Func<double, double>>).Differentiate();
            }
        }

        public static Expression<Func<double, double>> Differentiate(this Expression<Func<double, double>> expression)
        {
            var expBody = expression.Body;
            var expBin = (BinaryExpression)expBody;

            if (expBody.NodeType == ExpressionType.Add)
            {
                var x = Expression.Parameter(typeof(int), "x");               

                var leftPart = AddDifferentiateRule(expBin.Left);
                var RightPart = AddDifferentiateRule(expBin.Right);

                Expression newExpression = Expression.Lambda<Func<double, double>>(
                         Expression.Add(leftPart, RightPart),
                         x);
            }
            else if (expBody.NodeType == ExpressionType.Multiply)
            {
                
            }
            else if (expBody.NodeType == ExpressionType.Divide)
            {

            }
            else if (expBody.NodeType == ExpressionType.Subtract)
            {

            }
            throw new NotImplementedException();
        }
    }
}
