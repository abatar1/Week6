using System;
using System.Linq.Expressions;

namespace Week6
{
    public static class ExpressionExtension
    {
        private static Expression<Func<double, double>> AddSubDifferentiate
            (Expression exp, ParameterExpression x)
        {
            var dLeftPart = Expression.Lambda<Func<double, double>>((exp as BinaryExpression).Left, x).Differentiate().Body;
            var dRightPart = Expression.Lambda<Func<double, double>>((exp as BinaryExpression).Right, x).Differentiate().Body;

            var tmpExp = (exp.NodeType == ExpressionType.Add) ?
                Expression.Add(dLeftPart, dRightPart) :
                Expression.Subtract(dLeftPart, dRightPart);

            return Expression.Lambda<Func<double, double>>(tmpExp, x);
        }

        private static Expression<Func<double, double>> MultiplyDifferentiate
            (Expression exp, ParameterExpression x)
        {
            var leftPart = (exp as BinaryExpression).Left;
            var rightPart = (exp as BinaryExpression).Right;
            var dLeftPart = Expression.Lambda<Func<double, double>>(leftPart, x).Differentiate().Body;
            var dRightPart = Expression.Lambda<Func<double, double>>(rightPart, x).Differentiate().Body;

            if (leftPart is ConstantExpression && rightPart is ConstantExpression)
            {
                return Expression.Lambda<Func<double, double>>(
                    Expression.Constant(0),
                    x);
            }
            else if (leftPart is ConstantExpression)
            {
                return Expression.Lambda<Func<double, double>>(
                        Expression.Multiply(
                            leftPart,
                            dRightPart),
                    x);
            }
            else if (rightPart is ConstantExpression)
            {
                return Expression.Lambda<Func<double, double>>(
                        Expression.Multiply(
                            dLeftPart,
                            rightPart),
                    x);
            }
            else
            {
                return Expression.Lambda<Func<double, double>>(
                  Expression.Add(
                      Expression.Multiply(
                          leftPart, dRightPart),
                      Expression.Multiply(
                          dLeftPart, rightPart)),
                  x);
            }
        }

        private static Expression<Func<double, double>> DivideDifferentiate
            (Expression exp, ParameterExpression x)
        {
            var leftPart = (exp as BinaryExpression).Left;
            var rightPart = (exp as BinaryExpression).Right;
            var dLeftPart = Expression.Lambda<Func<double,double>>(leftPart, x).Differentiate().Body;
            var dRightPart = Expression.Lambda<Func<double, double>>(rightPart, x).Differentiate().Body;

            return Expression.Lambda<Func<double, double>>(
                Expression.Divide(
                    Expression.Subtract(
                        Expression.Multiply(
                            dLeftPart, rightPart),
                        Expression.Multiply(
                            leftPart, dRightPart)),
                    Expression.Multiply(
                        rightPart,
                        rightPart)),
                x);
        }

        private static Expression<Func<double, double>> SinDifferentiate(ParameterExpression x)
        {
            return Expression.Lambda<Func<double, double>>(
                Expression.Call(typeof(Math).GetMethod("Cos"), x)
                , x);
        }

        private static double GetConstant(Expression exp)
        {
            return (double)Expression.Lambda(exp).Compile().DynamicInvoke();
        }

        public static Expression<Func<double, double>> Differentiate
            (this Expression<Func<double, double>> expression)
        {
            var exp = expression.Body;
            var parameter = expression.Parameters[0];

            if (exp is ConstantExpression)
            {
                return Expression.Lambda<Func<double, double>>(Expression.Constant(0.0), parameter);
            }
            else if (exp is ParameterExpression)
            {
                return Expression.Lambda<Func<double, double>>(Expression.Constant(1.0), parameter);
            }
            else if (exp.NodeType == ExpressionType.Add)
            {
                return AddSubDifferentiate(exp, parameter);
            }
            else if (exp.NodeType == ExpressionType.Subtract)
            {
                return AddSubDifferentiate(exp, parameter);
            }
            else if (exp.NodeType == ExpressionType.Multiply)
            {
                return MultiplyDifferentiate(exp, parameter);
            }
            else if (exp.NodeType == ExpressionType.Divide)
            {
                return DivideDifferentiate(exp, parameter);
            }
            else if (exp.NodeType == ExpressionType.Call)
            {
                if (((MethodCallExpression)exp).Method.Name == "Sin")
                {
                    return SinDifferentiate(parameter);
                }
            }
            throw new NotImplementedException();
        }
    }
}
