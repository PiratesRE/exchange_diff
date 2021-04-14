using System;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	internal class ReduceToConstantVisitor : ExpressionVisitor
	{
		public static bool CanReduce(Expression expression)
		{
			return expression is ConstantExpression || expression is MemberExpression;
		}

		public static T Reduce<T>(Expression expression)
		{
			ReduceToConstantVisitor reduceToConstantVisitor = new ReduceToConstantVisitor();
			ConstantExpression constantExpression = (ConstantExpression)reduceToConstantVisitor.Visit(expression);
			return (T)((object)constantExpression.Value);
		}

		public override Expression Visit(Expression node)
		{
			if (node == null || ReduceToConstantVisitor.CanReduce(node))
			{
				return base.Visit(node);
			}
			throw new NotSupportedException(string.Format("TODO: LOC: ReduceToConstantVisitor needs to handle {0}", node.GetType().Name));
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			MemberExpression body = node.Update(this.Visit(node.Expression));
			object value = Expression.Lambda(body, new ParameterExpression[0]).Compile().DynamicInvoke(new object[0]);
			return Expression.Constant(value);
		}
	}
}
