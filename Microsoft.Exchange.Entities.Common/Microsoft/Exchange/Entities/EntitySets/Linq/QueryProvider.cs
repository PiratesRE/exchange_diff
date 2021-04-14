using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors;

namespace Microsoft.Exchange.Entities.EntitySets.Linq
{
	public abstract class QueryProvider : IQueryProvider
	{
		IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
		{
			IQueryable queryable = this.NormalizeExpressionAndCreateQuery(typeof(Query<TElement>), typeof(IQueryable<TElement>), expression);
			return (IQueryable<TElement>)queryable;
		}

		IQueryable IQueryProvider.CreateQuery(Expression expression)
		{
			Type enumerableElementTypeOrSameType = expression.Type.GetEnumerableElementTypeOrSameType();
			Type queryType = typeof(Query<>).MakeGenericType(new Type[]
			{
				enumerableElementTypeOrSameType
			});
			return this.NormalizeExpressionAndCreateQuery(queryType, typeof(IQueryable), expression);
		}

		TResult IQueryProvider.Execute<TResult>(Expression expression)
		{
			object obj = this.OnExecute(expression);
			if (obj is IConvertible)
			{
				return (TResult)((object)Convert.ChangeType(obj, typeof(TResult)));
			}
			return (TResult)((object)obj);
		}

		object IQueryProvider.Execute(Expression expression)
		{
			return this.OnExecute(expression);
		}

		protected virtual Expression Normalize(Expression expression)
		{
			return expression.Normalize();
		}

		protected abstract object OnExecute(Expression expression);

		protected abstract void Validate(Expression expression);

		private IQueryable NormalizeExpressionAndCreateQuery(Type queryType, Type queryInterface, Expression expression)
		{
			Expression expression2 = this.Normalize(expression);
			ConstantExpression constantExpression = expression2 as ConstantExpression;
			if (constantExpression != null && queryInterface.IsInstanceOfType(constantExpression.Value))
			{
				return (IQueryable)constantExpression.Value;
			}
			this.Validate(expression2);
			return (IQueryable)Activator.CreateInstance(queryType, new object[]
			{
				this,
				expression2
			});
		}
	}
}
