using System;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	public static class LamdbaExpressionFinder
	{
		public static LambdaExpression FindLambdaExpression(this Expression expression)
		{
			LamdbaExpressionFinder.FindLambdaExpressionVisitor findLambdaExpressionVisitor = new LamdbaExpressionFinder.FindLambdaExpressionVisitor();
			findLambdaExpressionVisitor.Visit(expression);
			return findLambdaExpressionVisitor.LambdaExpression;
		}

		private class FindLambdaExpressionVisitor : ExpressionVisitor
		{
			public LambdaExpression LambdaExpression { get; private set; }

			public override Expression Visit(Expression node)
			{
				if (this.LambdaExpression == null)
				{
					base.Visit(node);
				}
				return node;
			}

			protected override Expression VisitLambda<T>(Expression<T> node)
			{
				this.LambdaExpression = node;
				return node;
			}
		}
	}
}
