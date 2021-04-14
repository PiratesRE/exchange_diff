using System;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	public static class ExpressionNormalizer
	{
		public static Expression Normalize(this Expression expression)
		{
			return ExpressionNormalizer.Visitor.Visit(expression);
		}

		private static bool IsNoOpSelectCall(this Expression expression)
		{
			if (expression.IsMethodCall(QueryableMethods.Select, QueryableMethods.IndexedSelect))
			{
				MethodCallExpression methodCallExpression = (MethodCallExpression)expression;
				LambdaExpression lambdaExpression = methodCallExpression.Arguments[1].FindLambdaExpression();
				return lambdaExpression != null && lambdaExpression.Body == lambdaExpression.Parameters[0];
			}
			return false;
		}

		private static readonly ExpressionNormalizer.NormalizeVisitor Visitor = new ExpressionNormalizer.NormalizeVisitor();

		private class NormalizeVisitor : ExpressionVisitor
		{
			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (node.IsNoOpSelectCall())
				{
					return this.Visit(node.Arguments[0]);
				}
				return base.VisitMethodCall(node);
			}

			protected override Expression VisitUnary(UnaryExpression node)
			{
				if (node.NodeType == ExpressionType.Convert && node.Type == node.Operand.Type)
				{
					return this.Visit(node.Operand);
				}
				return base.VisitUnary(node);
			}
		}
	}
}
