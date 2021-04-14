using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	internal class EntityQueryOptionsVisitor : ExpressionVisitor
	{
		public EntityQueryOptionsVisitor(EntityQueryOptionsBuilder entityQueryOptionsBuilder, Expression knownExpression = null, IEntityQueryOptions knownQueryOptions = null)
		{
			this.entityQueryOptionsBuilder = entityQueryOptionsBuilder;
			this.knownExpression = knownExpression;
			this.knownQueryOptions = knownQueryOptions;
			this.oneArgMethods = new Dictionary<MethodInfo, Action<Expression>>
			{
				{
					QueryableMethods.Take,
					new Action<Expression>(this.entityQueryOptionsBuilder.ApplyTake)
				},
				{
					QueryableMethods.Skip,
					new Action<Expression>(this.entityQueryOptionsBuilder.ApplySkip)
				},
				{
					QueryableMethods.Where,
					new Action<Expression>(this.entityQueryOptionsBuilder.ApplyWhere)
				},
				{
					QueryableMethods.OrderBy,
					new Action<Expression>(this.entityQueryOptionsBuilder.ApplyOrderBy)
				},
				{
					QueryableMethods.ThenBy,
					new Action<Expression>(this.entityQueryOptionsBuilder.ApplyThenBy)
				},
				{
					QueryableMethods.OrderByDescending,
					new Action<Expression>(this.entityQueryOptionsBuilder.ApplyOrderByDescending)
				},
				{
					QueryableMethods.ThenByDescending,
					new Action<Expression>(this.entityQueryOptionsBuilder.ApplyThenByDescending)
				}
			};
		}

		public override Expression Visit(Expression node)
		{
			if (node == this.knownExpression)
			{
				this.entityQueryOptionsBuilder.CopyFrom(this.knownQueryOptions);
				return node;
			}
			if (node is ConstantExpression || node is MethodCallExpression)
			{
				return base.Visit(node);
			}
			throw new NotSupportedException(string.Format("TODO: LOC: EntityProviderExpressionVisitor does not support {0}", node.GetType().Name));
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (this.TryApplyMethodCall(node))
			{
				return node;
			}
			throw new NotSupportedException(string.Format("TODO: LOC: EntityProviderExpressionVisitor does not support calls to {0}", node.Method));
		}

		private bool TryApplyMethodCall(MethodCallExpression node)
		{
			ReadOnlyCollection<Expression> arguments = node.Arguments;
			if (arguments.Count > 0)
			{
				this.Visit(arguments[0]);
			}
			MethodInfo genericMethodDefinition = node.GetGenericMethodDefinition();
			switch (arguments.Count)
			{
			case 1:
				return genericMethodDefinition == QueryableMethods.Count || genericMethodDefinition == QueryableMethods.LongCount;
			case 2:
				return this.TryApplyMethodCall(genericMethodDefinition, arguments[1]);
			default:
				return false;
			}
		}

		private bool TryApplyMethodCall(MethodInfo method, Expression argument)
		{
			Action<Expression> action;
			if (this.oneArgMethods.TryGetValue(method, out action))
			{
				action(argument);
				return true;
			}
			return false;
		}

		private readonly EntityQueryOptionsBuilder entityQueryOptionsBuilder;

		private readonly Expression knownExpression;

		private readonly IEntityQueryOptions knownQueryOptions;

		private readonly Dictionary<MethodInfo, Action<Expression>> oneArgMethods;
	}
}
