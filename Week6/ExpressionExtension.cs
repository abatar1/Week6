using System;
using System.Linq.Expressions;

namespace Week6
{
    public static class ExpressionExtension
    {
        private static Expression SimpleDifferentiate
            (Expression exp, ParameterExpression x)
        {
            if (exp is ConstantExpression)
            {
               return Expression.Constant(0.0);
            }
            else if (exp is ParameterExpression)
            {
                return Expression.Constant(1.0);
            }
            else
            {
                return (Expression.Lambda<Func<double, double>>(exp, x)
                    .Differentiate() as Expression<Func<double, double>>).Body;
            }
        }

        private static Expression<Func<double, double>> AddSubDifferentiate
            (Expression exp, ParameterExpression x)
        {
            var dLeftPart = SimpleDifferentiate((exp as BinaryExpression).Left, x);
            var dRightPart = SimpleDifferentiate((exp as BinaryExpression).Right, x);

            if (dLeftPart is ConstantExpression && dRightPart is ConstantExpression)
            {
                var sign = (exp.NodeType == ExpressionType.Add) ? 1 : -1; 
                return Expression.Lambda<Func<double, double>>(
                    Expression.Constant(GetConstant(dLeftPart) + (sign) * GetConstant(dRightPart)),
                    x);
            }
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
            var dLeftPart = SimpleDifferentiate(leftPart, x);
            var dRightPart = SimpleDifferentiate(rightPart, x);

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
            var dLeftPart = SimpleDifferentiate(leftPart, x);
            var dRightPart = SimpleDifferentiate(rightPart, x);

            if (leftPart is ConstantExpression && rightPart is ConstantExpression)
            {
                return Expression.Lambda<Func<double, double>>(
                    Expression.Constant(0),
                    x);
            }
            else if (rightPart is ConstantExpression)
            {
                if (GetConstant(rightPart) == 0)
                {
                    throw new Exception("Divide by zero.");
                }
                return Expression.Lambda<Func<double, double>>(
                    Expression.Divide(
                        Expression.Multiply(
                            dLeftPart, rightPart),
                        Expression.Constant(
                            Math.Pow(GetConstant(rightPart), 2))),
                    x);
            }
            else if (leftPart is ConstantExpression)
            {
                return Expression.Lambda<Func<double, double>>(
                    Expression.Divide(
                        Expression.Multiply(
                            dRightPart, leftPart),
                        Expression.Multiply(
                            rightPart,
                            rightPart)),
                    x);
            }
            else
            {
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
        }

        private static Expression<Func<double, double>> SinDifferentiate(Expression exp, ParameterExpression x)
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

            if (exp.NodeType == ExpressionType.Add)
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
                    return SinDifferentiate(exp, parameter);
                }
            }
            throw new NotImplementedException();
        }
    }
}
