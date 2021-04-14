using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	public static class QueryFilterBuilder
	{
		public static QueryFilter ToQueryFilter(this Expression expression, IPropertyDefinitionMap propertyDefinitionMap)
		{
			QueryFilterBuilder.QueryFilterBuilderVisitor queryFilterBuilderVisitor = new QueryFilterBuilder.QueryFilterBuilderVisitor(propertyDefinitionMap);
			QueryFilter filter = queryFilterBuilderVisitor.GetFilter(expression);
			return QueryFilter.SimplifyFilter(filter);
		}

		public static IEnumerable<SortBy> ToSortBy(this IEnumerable<OrderByClause> clauses, IPropertyDefinitionMap propertyDefinitionMap)
		{
			QueryFilterBuilder.QueryFilterBuilderVisitor visitor = new QueryFilterBuilder.QueryFilterBuilderVisitor(propertyDefinitionMap);
			return from clause in clauses
			select clause.ToSortBy(visitor);
		}

		private static SortBy ToSortBy(this OrderByClause clause, QueryFilterBuilder.QueryFilterBuilderVisitor visitor)
		{
			return new SortBy(visitor.GetPropertyDefinition(clause.Expression), clause.Direction.ToSortOrder());
		}

		private static SortOrder ToSortOrder(this ListSortDirection sortDirection)
		{
			if (sortDirection != ListSortDirection.Ascending)
			{
				return SortOrder.Descending;
			}
			return SortOrder.Ascending;
		}

		private sealed class QueryFilterBuilderVisitor : ExpressionVisitor
		{
			public QueryFilterBuilderVisitor(IPropertyDefinitionMap propertyDefinitionMap)
			{
				this.propertyDefinitionMap = propertyDefinitionMap;
			}

			public QueryFilter GetFilter(Expression expression)
			{
				QueryFilter result;
				try
				{
					try
					{
						this.Visit(expression);
					}
					catch (NotSupportedException innerException)
					{
						throw new UnsupportedExpressionException(Strings.UnsupportedFilterExpression(expression), innerException);
					}
					QueryFilter queryFilter;
					if ((queryFilter = this.filter) == null)
					{
						queryFilter = (this.propertyDefinition.AsBooleanComparisonQueryFilter() ?? (this.hasPropertyValue ? this.propertyValue.AsBooleanQueryFilter() : null));
					}
					this.filter = queryFilter;
					if (this.filter == null)
					{
						throw new UnsupportedExpressionException(Strings.UnsupportedFilterExpression(expression));
					}
					result = this.filter;
				}
				finally
				{
					this.ResetState();
				}
				return result;
			}

			public PropertyDefinition GetPropertyDefinition(Expression expression)
			{
				PropertyDefinition result;
				try
				{
					try
					{
						this.Visit(expression);
					}
					catch (NotSupportedException innerException)
					{
						throw new UnsupportedExpressionException(Strings.UnsupportedPropertyExpression(expression), innerException);
					}
					if (this.propertyDefinition == null)
					{
						throw new UnsupportedExpressionException(Strings.UnsupportedPropertyExpression(expression));
					}
					result = this.propertyDefinition;
				}
				finally
				{
					this.ResetState();
				}
				return result;
			}

			public object GetPropertyValue(Expression expression)
			{
				object result;
				try
				{
					try
					{
						this.Visit(expression);
					}
					catch (NotSupportedException innerException)
					{
						throw new UnsupportedExpressionException(Strings.UnsupportedPropertyValue(expression), innerException);
					}
					if (!this.hasPropertyValue)
					{
						throw new UnsupportedExpressionException(Strings.UnsupportedPropertyValue(expression));
					}
					result = this.propertyValue;
				}
				finally
				{
					this.ResetState();
				}
				return result;
			}

			public override Expression Visit(Expression node)
			{
				ExAssert.RetailAssert(this.filter == null && this.propertyDefinition == null && !this.hasPropertyValue, "Cannot visit a new node until filter, propertyDefiniton or value from previous visit has been consumed.");
				if (node is BinaryExpression || node is ConstantExpression || node is MemberExpression || node is UnaryExpression || node is MethodCallExpression || node is NewArrayExpression || node is LambdaExpression)
				{
					return base.Visit(node);
				}
				return node;
			}

			protected override Expression VisitBinary(BinaryExpression node)
			{
				if (!this.ProduceAndFilter(node) && !this.ProduceOrFilter(node) && !this.ProduceBlobComparisonFilter(node) && !this.ProduceStringComparisonFilter(node))
				{
					this.ProduceGeneralComparisonFilter(node);
				}
				return node;
			}

			protected override Expression VisitConstant(ConstantExpression node)
			{
				this.propertyValue = node.Value;
				this.hasPropertyValue = true;
				return node;
			}

			protected override Expression VisitLambda<T>(Expression<T> node)
			{
				this.Visit(node.Body);
				return node;
			}

			protected override Expression VisitMember(MemberExpression node)
			{
				if (this.ProducePropertyDefinition(node) || this.ProduceConstantValue(node))
				{
					return node;
				}
				return node;
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (this.ProduceTextFilter(node))
				{
					return node;
				}
				throw new UnsupportedExpressionException(Strings.UnsupportedMethodCall(node.Method));
			}

			protected override Expression VisitNewArray(NewArrayExpression node)
			{
				if (node.Type == typeof(byte[]))
				{
					this.propertyValue = Expression.Lambda(node, new ParameterExpression[0]).Compile().DynamicInvoke(new object[0]);
					this.hasPropertyValue = true;
				}
				return node;
			}

			protected override Expression VisitUnary(UnaryExpression node)
			{
				ExpressionType nodeType = node.NodeType;
				if (nodeType != ExpressionType.Not)
				{
					if (nodeType == ExpressionType.Quote)
					{
						base.VisitUnary(node);
					}
				}
				else
				{
					this.filter = new NotFilter(this.GetFilter(node.Operand));
				}
				return node;
			}

			private bool ProduceAndFilter(BinaryExpression node)
			{
				if (node.NodeType == ExpressionType.AndAlso)
				{
					this.filter = new AndFilter(new QueryFilter[]
					{
						this.GetFilter(node.Left),
						this.GetFilter(node.Right)
					});
					return true;
				}
				return false;
			}

			private bool ProduceBlobComparisonFilter(BinaryExpression node)
			{
				if (node.Left.Type == typeof(byte[]))
				{
					PropertyDefinition propertyDefinition = this.GetPropertyDefinition(node.Left);
					byte[] array = (byte[])this.GetPropertyValue(node.Right);
					this.filter = node.NodeType.ToBlobComparisonFilter(propertyDefinition, array);
					return true;
				}
				return false;
			}

			private bool ProduceConstantValue(Expression node)
			{
				bool result;
				try
				{
					this.propertyValue = ReduceToConstantVisitor.Reduce<object>(node);
					this.hasPropertyValue = true;
					result = true;
				}
				catch (NotSupportedException)
				{
					result = false;
				}
				return result;
			}

			private bool ProduceGeneralComparisonFilter(BinaryExpression node)
			{
				PropertyDefinition property = this.GetPropertyDefinition(node.Left);
				object obj = this.GetPropertyValue(node.Right);
				this.filter = ((obj != null) ? new ComparisonFilter(node.NodeType.ToComparisonOperator(), property, obj) : node.NodeType.ToNullComparisonFilter(property));
				return true;
			}

			private bool ProduceOrFilter(BinaryExpression node)
			{
				if (node.NodeType == ExpressionType.OrElse)
				{
					this.filter = new OrFilter(new QueryFilter[]
					{
						this.GetFilter(node.Left),
						this.GetFilter(node.Right)
					});
					return true;
				}
				return false;
			}

			private bool ProducePropertyDefinition(MemberExpression node)
			{
				if (node.Expression is ParameterExpression)
				{
					PropertyInfo propertyInfo = node.Member as PropertyInfo;
					if (propertyInfo != null)
					{
						this.propertyDefinitionMap.TryGetPropertyDefinition(propertyInfo, out this.propertyDefinition);
					}
					return true;
				}
				return false;
			}

			private bool ProduceStringComparisonFilter(BinaryExpression node)
			{
				if (!node.Left.IsMethodCall(StringMethods.Compare))
				{
					return false;
				}
				MethodCallExpression methodCallExpression = (MethodCallExpression)node.Left;
				int num = ReduceToConstantVisitor.Reduce<int>(node.Right);
				if (num != 0)
				{
					throw new UnsupportedExpressionException(Strings.StringCompareMustCompareToZero);
				}
				this.filter = new ComparisonFilter(node.NodeType.ToComparisonOperator(), this.GetPropertyDefinition(methodCallExpression.Arguments[0]), this.GetPropertyValue(methodCallExpression.Arguments[1]));
				return true;
			}

			private bool ProduceTextFilter(MethodCallExpression node)
			{
				MatchOptions? textFilterMatchOptions = node.Method.GetTextFilterMatchOptions();
				if (textFilterMatchOptions != null)
				{
					PropertyDefinition property = this.GetPropertyDefinition(node.Object);
					string text = (string)this.GetPropertyValue(node.Arguments[0]);
					this.filter = new TextFilter(property, text, textFilterMatchOptions.Value, MatchFlags.IgnoreCase);
					return true;
				}
				return false;
			}

			private void ResetState()
			{
				this.filter = null;
				this.propertyDefinition = null;
				this.hasPropertyValue = false;
				this.propertyValue = null;
			}

			private readonly IPropertyDefinitionMap propertyDefinitionMap;

			private QueryFilter filter;

			private bool hasPropertyValue;

			private PropertyDefinition propertyDefinition;

			private object propertyValue;
		}
	}
}
