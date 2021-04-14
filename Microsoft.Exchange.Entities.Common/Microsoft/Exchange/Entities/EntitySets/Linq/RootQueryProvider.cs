using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors;

namespace Microsoft.Exchange.Entities.EntitySets.Linq
{
	public abstract class RootQueryProvider<T> : QueryProvider, IQueryable<T>, IEnumerable<T>, IQueryable, IEnumerable
	{
		protected RootQueryProvider(string name)
		{
			this.rootExpression = Expression.Constant(this);
			this.description = "value(" + name + ")";
		}

		Type IQueryable.ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		Expression IQueryable.Expression
		{
			get
			{
				return this.rootExpression;
			}
		}

		IQueryProvider IQueryable.Provider
		{
			get
			{
				return this;
			}
		}

		public sealed override string ToString()
		{
			return this.description;
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.FindAll().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<!0>)this).GetEnumerator();
		}

		protected sealed override object OnExecute(Expression expression)
		{
			if (expression.IsConstantExpressionWithValue(this))
			{
				return this.FindAll();
			}
			return this.ExecuteQuery(expression);
		}

		protected abstract object ExecuteQuery(Expression queryExpression);

		protected abstract IEnumerable<T> FindAll();

		private readonly string description;

		private readonly Expression rootExpression;
	}
}
