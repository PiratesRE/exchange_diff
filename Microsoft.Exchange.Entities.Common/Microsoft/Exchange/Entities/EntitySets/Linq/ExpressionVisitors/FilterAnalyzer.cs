using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	internal static class FilterAnalyzer
	{
		public static bool IsWhereIdEqualsKey<TKey>(this Expression expression, out TKey key)
		{
			key = default(TKey);
			LambdaExpression lambdaExpression = expression.FindLambdaExpression();
			if (lambdaExpression != null)
			{
				BinaryExpression binaryExpression = lambdaExpression.Body as BinaryExpression;
				if (binaryExpression != null && binaryExpression.NodeType == ExpressionType.Equal && binaryExpression.Left.Type == typeof(TKey) && binaryExpression.Right.Type == typeof(TKey) && binaryExpression.Left is MemberExpression && ReduceToConstantVisitor.CanReduce(binaryExpression.Right))
				{
					MemberExpression memberExpression = (MemberExpression)binaryExpression.Left;
					MemberInfo member = memberExpression.Member;
					if (memberExpression.Expression == lambdaExpression.Parameters[0] && member.MemberType == MemberTypes.Property && member.Name == "Id")
					{
						key = ReduceToConstantVisitor.Reduce<TKey>(binaryExpression.Right);
						return true;
					}
				}
			}
			return false;
		}
	}
}
