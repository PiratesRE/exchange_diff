using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DataEntityFilterConverter : ODataFilterConverter
	{
		public DataEntityFilterConverter(EntitySchema schema) : base(schema)
		{
		}

		public Expression ConvertFilterClause(FilterClause filterClause)
		{
			ArgumentValidator.ThrowIfNull("filterClause", filterClause);
			Expression body = this.ConvertFilterNode(filterClause.Expression);
			return Expression.Lambda(body, new ParameterExpression[]
			{
				this.parameterExpression
			});
		}

		public IReadOnlyList<OrderByClause> ConvertOrderByClause(OrderByClause orderByClause)
		{
			ArgumentValidator.ThrowIfNull("orderByClause", orderByClause);
			return null;
		}

		private Expression ConvertFilterNode(QueryNode queryNode)
		{
			Expression expression = null;
			if (queryNode.Kind == 5)
			{
				UnaryOperatorNode unaryOperatorNode = (UnaryOperatorNode)queryNode;
				if (unaryOperatorNode.OperatorKind == 1)
				{
					expression = Expression.Not(this.ConvertFilterNode(unaryOperatorNode.Operand));
				}
			}
			else if (queryNode.Kind == 4)
			{
				BinaryOperatorNode binaryOperatorNode = (BinaryOperatorNode)queryNode;
				if (binaryOperatorNode.OperatorKind == 1 || binaryOperatorNode.OperatorKind == null)
				{
					Expression left = this.ConvertFilterNode(binaryOperatorNode.Left);
					Expression right = this.ConvertFilterNode(binaryOperatorNode.Right);
					if (binaryOperatorNode.OperatorKind == 1)
					{
						expression = Expression.AndAlso(left, right);
					}
					else
					{
						expression = Expression.OrElse(left, right);
					}
				}
				else
				{
					IExpressionQueryBuilder propertyProvider = this.GetPropertyProvider(binaryOperatorNode.Left);
					MemberExpression queryPropertyExpression = propertyProvider.GetQueryPropertyExpression();
					this.parameterExpression = (ParameterExpression)queryPropertyExpression.Expression;
					Expression constantExpression = this.GetConstantExpression(binaryOperatorNode.Right, base.GetEntityProperty(binaryOperatorNode.Left), queryPropertyExpression.Type);
					switch (binaryOperatorNode.OperatorKind)
					{
					case 2:
						expression = Expression.Equal(queryPropertyExpression, constantExpression);
						break;
					case 3:
						expression = Expression.NotEqual(queryPropertyExpression, constantExpression);
						break;
					case 4:
						expression = Expression.GreaterThan(queryPropertyExpression, constantExpression);
						break;
					case 5:
						expression = Expression.GreaterThanOrEqual(queryPropertyExpression, constantExpression);
						break;
					case 6:
						expression = Expression.LessThan(queryPropertyExpression, constantExpression);
						break;
					case 7:
						expression = Expression.LessThanOrEqual(queryPropertyExpression, constantExpression);
						break;
					}
				}
			}
			else if (queryNode.Kind == 8)
			{
				SingleValueFunctionCallNode singleValueFunctionCallNode = (SingleValueFunctionCallNode)queryNode;
				ODataFilterConverter.BinaryOperandPair binaryOperandPair = base.ParseBinaryFunctionParameters(singleValueFunctionCallNode);
				IExpressionQueryBuilder propertyProvider2 = this.GetPropertyProvider(binaryOperandPair.Left);
				MemberExpression queryPropertyExpression2 = propertyProvider2.GetQueryPropertyExpression();
				this.parameterExpression = (ParameterExpression)queryPropertyExpression2.Expression;
				Expression constantExpression2 = this.GetConstantExpression(binaryOperandPair.Right, base.GetEntityProperty(binaryOperandPair.Left));
				string name;
				if ((name = singleValueFunctionCallNode.Name) != null)
				{
					if (!(name == "contains"))
					{
						if (name == "startswith")
						{
							expression = Expression.Call(queryPropertyExpression2, "StartsWith", null, new Expression[]
							{
								constantExpression2
							});
						}
					}
					else
					{
						expression = Expression.Call(queryPropertyExpression2, "Contains", null, new Expression[]
						{
							constantExpression2
						});
					}
				}
			}
			if (expression == null)
			{
				throw new InvalidFilterNodeException(queryNode);
			}
			return expression;
		}

		private IExpressionQueryBuilder GetPropertyProvider(QueryNode queryNode)
		{
			return base.GetEntityProperty(queryNode).DataEntityPropertyProvider as IExpressionQueryBuilder;
		}

		private Expression GetConstantExpression(QueryNode queryNode, PropertyDefinition propertyDefinition)
		{
			return this.GetConstantExpression(queryNode, propertyDefinition, propertyDefinition.Type);
		}

		private Expression GetConstantExpression(QueryNode queryNode, PropertyDefinition propertyDefinition, Type type)
		{
			IExpressionQueryBuilder expressionQueryBuilder = propertyDefinition.DataEntityPropertyProvider as IExpressionQueryBuilder;
			return expressionQueryBuilder.GetQueryConstant(base.ExtractConstantNodeValue(queryNode, type));
		}

		private ParameterExpression parameterExpression;
	}
}
