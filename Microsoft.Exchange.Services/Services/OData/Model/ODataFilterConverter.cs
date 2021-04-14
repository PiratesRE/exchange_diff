using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class ODataFilterConverter
	{
		public ODataFilterConverter(EntitySchema schema)
		{
			ArgumentValidator.ThrowIfNull("schema", schema);
			this.EntitySchema = schema;
		}

		public EntitySchema EntitySchema { get; private set; }

		protected ODataFilterConverter.BinaryOperandPair ParseBinaryFunctionParameters(SingleValueFunctionCallNode functionNode)
		{
			ArgumentValidator.ThrowIfNull("functionNode", functionNode);
			QueryNode[] array = functionNode.Parameters.ToArray<QueryNode>();
			if (array == null || array.Length != 2)
			{
				throw new InvalidFilterNodeException(functionNode);
			}
			return new ODataFilterConverter.BinaryOperandPair(array[0], array[1]);
		}

		protected QueryNode UnwrapConvertNode(QueryNode queryNode)
		{
			ConvertNode convertNode = queryNode as ConvertNode;
			if (convertNode != null)
			{
				return convertNode.Source;
			}
			return queryNode;
		}

		protected PropertyDefinition GetEntityProperty(QueryNode queryNode)
		{
			queryNode = this.UnwrapConvertNode(queryNode);
			SingleValuePropertyAccessNode singleValuePropertyAccessNode = queryNode as SingleValuePropertyAccessNode;
			if (singleValuePropertyAccessNode == null)
			{
				throw new InvalidFilterNodeException(queryNode);
			}
			PropertyDefinition propertyDefinition = this.EntitySchema.ResolveProperty(singleValuePropertyAccessNode.Property.Name);
			if (!propertyDefinition.Flags.HasFlag(PropertyDefinitionFlags.CanFilter))
			{
				throw new PropertyNotSupportFilterException(propertyDefinition.Name);
			}
			return propertyDefinition;
		}

		protected object ExtractConstantNodeValue(QueryNode queryNode, Type type)
		{
			queryNode = this.UnwrapConvertNode(queryNode);
			ConstantNode constantNode = queryNode as ConstantNode;
			if (constantNode == null)
			{
				throw new InvalidFilterNodeException(queryNode);
			}
			if (constantNode.Value is ODataEnumValue)
			{
				ODataEnumValue odataEnumValue = constantNode.Value as ODataEnumValue;
				return Enum.Parse(type, odataEnumValue.Value);
			}
			return constantNode.Value;
		}

		public const string Contains = "contains";

		public const string StartsWith = "startswith";

		public const string EndsWith = "endswith";

		protected struct BinaryOperandPair
		{
			public BinaryOperandPair(QueryNode left, QueryNode right)
			{
				this.Left = left;
				this.Right = right;
			}

			public QueryNode Left;

			public QueryNode Right;
		}
	}
}
