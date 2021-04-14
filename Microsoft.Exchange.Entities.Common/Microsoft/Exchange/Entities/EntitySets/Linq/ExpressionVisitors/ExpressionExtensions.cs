using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	internal static class ExpressionExtensions
	{
		public static MethodInfo GetGenericMethodDefinition(this MethodCallExpression expression)
		{
			MethodInfo method = expression.Method;
			if (!method.IsGenericMethod)
			{
				return method;
			}
			return method.GetGenericMethodDefinition();
		}

		public static MethodInfo GetGenericMethodDefinition(this LambdaExpression expression)
		{
			return (expression.Body as MethodCallExpression).GetGenericMethodDefinition();
		}

		public static bool IsConstantExpressionWithValue(this Expression expression, object expectedValue)
		{
			return expression is ConstantExpression && object.Equals((expression as ConstantExpression).Value, expectedValue);
		}

		public static bool IsMethodCall(this Expression expression, MethodInfo methodInfo)
		{
			if (expression is MethodCallExpression)
			{
				MethodInfo genericMethodDefinition = (expression as MethodCallExpression).GetGenericMethodDefinition();
				return genericMethodDefinition == methodInfo;
			}
			return false;
		}

		public static bool IsMethodCall(this Expression expression, MethodInfo methodInfo1, MethodInfo methodInfo2)
		{
			if (expression is MethodCallExpression)
			{
				MethodInfo genericMethodDefinition = (expression as MethodCallExpression).GetGenericMethodDefinition();
				return genericMethodDefinition == methodInfo1 || genericMethodDefinition == methodInfo2;
			}
			return false;
		}

		public static Expression RemoveQuote(this Expression expression)
		{
			if (expression.NodeType == ExpressionType.Quote)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;
				return unaryExpression.Operand;
			}
			return expression;
		}
	}
}
