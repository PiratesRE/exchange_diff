using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class EwsFilterConverter : ODataFilterConverter
	{
		public EwsFilterConverter(EntitySchema schema) : base(schema)
		{
		}

		public SearchExpressionType ConvertFilterClause(FilterClause filterClause)
		{
			ArgumentValidator.ThrowIfNull("filterClause", filterClause);
			return this.ConvertFilterNode(filterClause.Expression);
		}

		public SortResults[] ConvertOrderByClause(OrderByClause orderByClause)
		{
			List<SortResults> list = new List<SortResults>();
			for (OrderByClause orderByClause2 = orderByClause; orderByClause2 != null; orderByClause2 = orderByClause.ThenBy)
			{
				list.Add(this.CreateSortResults(orderByClause2));
			}
			return list.ToArray();
		}

		private SearchExpressionType ConvertFilterNode(QueryNode queryNode)
		{
			SearchExpressionType searchExpressionType = null;
			if (queryNode.Kind == 2)
			{
				ConvertNode convertNode = (ConvertNode)queryNode;
				if (convertNode.Source != null)
				{
					return this.ConvertFilterNode(convertNode.Source);
				}
			}
			if (queryNode.Kind == 5)
			{
				UnaryOperatorNode unaryOperatorNode = (UnaryOperatorNode)queryNode;
				if (unaryOperatorNode.OperatorKind == 1)
				{
					searchExpressionType = new NotType
					{
						Item = this.ConvertFilterNode(unaryOperatorNode.Operand)
					};
				}
			}
			else if (queryNode.Kind == 4)
			{
				BinaryOperatorNode binaryOperatorNode = (BinaryOperatorNode)queryNode;
				if (binaryOperatorNode.OperatorKind == 1 || binaryOperatorNode.OperatorKind == null)
				{
					SearchExpressionType searchExpressionType2 = this.ConvertFilterNode(binaryOperatorNode.Left);
					SearchExpressionType searchExpressionType3 = this.ConvertFilterNode(binaryOperatorNode.Right);
					MultipleOperandBooleanExpressionType multipleOperandBooleanExpressionType = (binaryOperatorNode.OperatorKind == 1) ? new AndType() : new OrType();
					multipleOperandBooleanExpressionType.Items = new SearchExpressionType[]
					{
						searchExpressionType2,
						searchExpressionType3
					};
					searchExpressionType = multipleOperandBooleanExpressionType;
				}
				else
				{
					EwsPropertyProvider propertyProvider = this.GetPropertyProvider(binaryOperatorNode.Left);
					PropertyPath propertyPath = propertyProvider.PropertyInformation.PropertyPath;
					FieldURIOrConstantType constantValue = this.GetConstantValue(binaryOperatorNode.Right, base.GetEntityProperty(binaryOperatorNode.Left));
					TwoOperandExpressionType twoOperandExpressionType = null;
					switch (binaryOperatorNode.OperatorKind)
					{
					case 2:
						twoOperandExpressionType = new IsEqualToType();
						break;
					case 3:
						twoOperandExpressionType = new IsNotEqualToType();
						break;
					case 4:
						twoOperandExpressionType = new IsGreaterThanType();
						break;
					case 5:
						twoOperandExpressionType = new IsGreaterThanOrEqualToType();
						break;
					case 6:
						twoOperandExpressionType = new IsLessThanType();
						break;
					case 7:
						twoOperandExpressionType = new IsLessThanOrEqualToType();
						break;
					}
					if (twoOperandExpressionType != null)
					{
						twoOperandExpressionType.Item = propertyPath;
						twoOperandExpressionType.FieldURIOrConstant = constantValue;
						searchExpressionType = twoOperandExpressionType;
					}
				}
			}
			else if (queryNode.Kind == 8)
			{
				SingleValueFunctionCallNode singleValueFunctionCallNode = (SingleValueFunctionCallNode)queryNode;
				ODataFilterConverter.BinaryOperandPair binaryOperandPair = base.ParseBinaryFunctionParameters(singleValueFunctionCallNode);
				EwsPropertyProvider propertyProvider2 = this.GetPropertyProvider(binaryOperandPair.Left);
				PropertyPath propertyPath2 = propertyProvider2.PropertyInformation.PropertyPath;
				FieldURIOrConstantType constantValue2 = this.GetConstantValue(binaryOperandPair.Right, base.GetEntityProperty(binaryOperandPair.Left));
				ContainsExpressionType containsExpressionType = new ContainsExpressionType
				{
					ContainmentComparisonString = ContainmentComparisonType.IgnoreCase.ToString(),
					Item = propertyPath2,
					Constant = (ConstantValueType)constantValue2.Item
				};
				string name;
				if ((name = singleValueFunctionCallNode.Name) != null)
				{
					if (!(name == "contains"))
					{
						if (!(name == "startswith"))
						{
							goto IL_283;
						}
						containsExpressionType.ContainmentModeString = ContainmentModeType.Prefixed.ToString();
					}
					else
					{
						containsExpressionType.ContainmentModeString = ContainmentModeType.Substring.ToString();
					}
					searchExpressionType = containsExpressionType;
					goto IL_28E;
				}
				IL_283:
				throw new InvalidFilterNodeException(singleValueFunctionCallNode);
			}
			IL_28E:
			if (searchExpressionType == null)
			{
				throw new InvalidFilterNodeException(queryNode);
			}
			return searchExpressionType;
		}

		private EwsPropertyProvider GetPropertyProvider(QueryNode queryNode)
		{
			return base.GetEntityProperty(queryNode).EwsPropertyProvider.GetEwsPropertyProvider(base.EntitySchema);
		}

		private FieldURIOrConstantType GetConstantValue(QueryNode queryNode, PropertyDefinition propertyDefinition)
		{
			object value = base.ExtractConstantNodeValue(queryNode, propertyDefinition.Type);
			EwsPropertyProvider ewsPropertyProvider = propertyDefinition.EwsPropertyProvider.GetEwsPropertyProvider(base.EntitySchema);
			return new FieldURIOrConstantType
			{
				Item = new ConstantValueType
				{
					Value = ewsPropertyProvider.GetQueryConstant(value)
				}
			};
		}

		private SortResults CreateSortResults(OrderByClause orderByClause)
		{
			EwsPropertyProvider propertyProvider = this.GetPropertyProvider(orderByClause.Expression);
			PropertyPath propertyPath = propertyProvider.PropertyInformation.PropertyPath;
			SortResults sortResults = new SortResults();
			if (orderByClause.Direction == null)
			{
				sortResults.Order = SortDirection.Ascending;
			}
			else
			{
				sortResults.Order = SortDirection.Descending;
			}
			sortResults.SortByProperty = propertyPath;
			return sortResults;
		}
	}
}
