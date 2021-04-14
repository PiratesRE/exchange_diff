using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	internal class EntityQueryOptionsBuilder : IEntityQueryOptions
	{
		public Expression Filter { get; private set; }

		public IReadOnlyList<OrderByClause> OrderBy
		{
			get
			{
				return this.orderBy;
			}
		}

		public int? Skip { get; private set; }

		public int? Take { get; private set; }

		public void ApplyOrderBy(Expression argument)
		{
			this.AddOrderingExpression(new OrderByClause(argument, ListSortDirection.Ascending), true);
		}

		public void ApplyOrderByDescending(Expression argument)
		{
			this.AddOrderingExpression(new OrderByClause(argument, ListSortDirection.Descending), true);
		}

		public void ApplySkip(Expression argument)
		{
			if (this.Skip != null)
			{
				throw new NotSupportedException("TODO: LOC: Cannot Apply Skip twice.");
			}
			if (this.Take != null)
			{
				throw new NotSupportedException("TODO: LOC: Skip needs to be applied before Top is applied.");
			}
			int num = ReduceToConstantVisitor.Reduce<int>(argument);
			if (num < 0)
			{
				throw new ArgumentException("TODO: LOC: 'Skip' argument must be non-negative integer.");
			}
			this.Skip = new int?(num);
		}

		public void ApplyTake(Expression argument)
		{
			if (this.Take != null)
			{
				throw new NotSupportedException("TODO: LOC: Cannot Apply Take twice.");
			}
			int num = ReduceToConstantVisitor.Reduce<int>(argument);
			if (num < 0)
			{
				throw new ArgumentException("TODO: LOC: 'Take' argument must be non-negative integer.");
			}
			this.Take = new int?(num);
		}

		public void ApplyThenBy(Expression argument)
		{
			this.AddOrderingExpression(new OrderByClause(argument, ListSortDirection.Ascending), false);
		}

		public void ApplyThenByDescending(Expression argument)
		{
			this.AddOrderingExpression(new OrderByClause(argument, ListSortDirection.Descending), false);
		}

		public void ApplyWhere(Expression argument)
		{
			if (this.OrderBy != null || this.Skip != null || this.Take != null)
			{
				throw new NotSupportedException("TODO: LOC: 'Where' needs to be called before ordering functions, 'Skip' or 'Take' are called.");
			}
			if (this.Filter != null)
			{
				throw new NotSupportedException("TODO: LOC: Only one filter is supported and one is already specified.");
			}
			this.Filter = argument;
		}

		public void CopyFrom(IEntityQueryOptions options)
		{
			if (this.Skip != null || this.Take != null || this.Filter != null || this.OrderBy != null)
			{
				throw new InvalidOperationException();
			}
			this.Filter = options.Filter;
			IReadOnlyList<OrderByClause> readOnlyList = options.OrderBy;
			if (readOnlyList == null)
			{
				this.orderBy = null;
			}
			else
			{
				for (int i = 0; i < readOnlyList.Count; i++)
				{
					OrderByClause clause = readOnlyList[i];
					this.AddOrderingExpression(clause, i == 0);
				}
			}
			this.Skip = options.Skip;
			this.Take = options.Take;
		}

		private void AddOrderingExpression(OrderByClause clause, bool beginNewList)
		{
			if (this.Skip != null || this.Take != null)
			{
				throw new NotSupportedException("TODO: LOC: Ordering functions needs to be called before 'Skip' or 'Take' are called.");
			}
			if (beginNewList)
			{
				if (this.OrderBy != null)
				{
					throw new NotSupportedException("TODO: LOC: Only one set of ordering expressions is supported and one is already specified.");
				}
				this.orderBy = new List<OrderByClause>();
			}
			else if (this.OrderBy == null)
			{
				throw new NotSupportedException("ApplyOrderBy or ApplyOrderByDescending needs to be called before ApplyThenBy/ApplyThenByDescending");
			}
			this.orderBy.Add(clause);
		}

		private List<OrderByClause> orderBy;
	}
}
