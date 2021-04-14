using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Entities.DataModel
{
	[ImmutableObject(true)]
	public sealed class OrderByClause
	{
		public OrderByClause(Expression expression, ListSortDirection direction)
		{
			this.Expression = expression;
			this.Direction = direction;
		}

		public ListSortDirection Direction { get; private set; }

		public Expression Expression { get; private set; }
	}
}
