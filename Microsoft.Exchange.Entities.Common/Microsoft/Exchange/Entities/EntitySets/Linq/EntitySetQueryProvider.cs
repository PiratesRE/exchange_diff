using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors;

namespace Microsoft.Exchange.Entities.EntitySets.Linq
{
	public class EntitySetQueryProvider<TEntity> : RootQueryProvider<TEntity> where TEntity : class, IEntity
	{
		public EntitySetQueryProvider(IEntitySet<TEntity> entitySet, CommandContext commandContext) : base(entitySet.ToString())
		{
			this.entitySet = entitySet;
			this.commandContext = commandContext;
		}

		protected sealed override object ExecuteQuery(Expression queryExpression)
		{
			this.Validate(queryExpression);
			if (queryExpression.IsMethodCall(QueryableMethods.Count, QueryableMethods.LongCount))
			{
				return this.entitySet.EstimateTotalCount(this.lastExpressionQueryOptions, this.commandContext);
			}
			string key;
			if (this.lastExpressionQueryOptions.Filter.IsWhereIdEqualsKey(out key))
			{
				if (this.lastExpressionQueryOptions.Skip.GetValueOrDefault(0) == 0)
				{
					TEntity tentity = this.entitySet.Read(key, this.commandContext);
					if (tentity != null)
					{
						return new TEntity[]
						{
							tentity
						};
					}
				}
				return Enumerable.Empty<TEntity>();
			}
			return this.entitySet.Find(this.lastExpressionQueryOptions, this.commandContext);
		}

		protected sealed override IEnumerable<TEntity> FindAll()
		{
			return this.entitySet.Find(null, this.commandContext);
		}

		protected virtual IEntityQueryOptions GetQueryOptions(Expression expression)
		{
			EntityQueryOptionsBuilder entityQueryOptionsBuilder = new EntityQueryOptionsBuilder();
			EntityQueryOptionsVisitor entityQueryOptionsVisitor = new EntityQueryOptionsVisitor(entityQueryOptionsBuilder, this.lastValidatedExpression, this.lastExpressionQueryOptions);
			entityQueryOptionsVisitor.Visit(expression);
			return entityQueryOptionsBuilder;
		}

		protected sealed override void Validate(Expression expression)
		{
			if (this.lastValidatedExpression != expression)
			{
				IEntityQueryOptions queryOptions = this.GetQueryOptions(expression);
				this.lastValidatedExpression = expression;
				this.lastExpressionQueryOptions = queryOptions;
			}
		}

		private readonly CommandContext commandContext;

		private readonly IEntitySet<TEntity> entitySet;

		private IEntityQueryOptions lastExpressionQueryOptions;

		private Expression lastValidatedExpression;
	}
}
