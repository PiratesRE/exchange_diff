using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Entities.EntitySets.Linq
{
	internal class Query<T> : IOrderedQueryable<T>, IQueryable<T>, IEnumerable<!0>, IOrderedQueryable, IQueryable, IEnumerable
	{
		public Query(IQueryProvider queryProvider, Expression expression)
		{
			this.queryProvider = queryProvider;
			this.expression = expression;
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
				return this.expression;
			}
		}

		IQueryProvider IQueryable.Provider
		{
			get
			{
				return this.queryProvider;
			}
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.queryProvider.Execute<IEnumerable<T>>(this.expression).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<!0>)this).GetEnumerator();
		}

		private readonly Expression expression;

		private readonly IQueryProvider queryProvider;
	}
}
