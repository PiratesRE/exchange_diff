using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class SizeFilter : QueryFilter
	{
		public SizeFilter(ComparisonOperator comparisonOperator, PropertyDefinition property, uint propertySize)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this.comparisonOperator = comparisonOperator;
			this.property = property;
			this.propertySize = propertySize;
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.property.ToString());
			sb.Append(" ");
			sb.Append(this.comparisonOperator.ToString());
			sb.Append(" ");
			sb.Append(this.propertySize);
			sb.Append(")");
		}

		public ComparisonOperator ComparisonOperator
		{
			get
			{
				return this.comparisonOperator;
			}
		}

		public PropertyDefinition Property
		{
			get
			{
				return this.property;
			}
		}

		public uint PropertySize
		{
			get
			{
				return this.propertySize;
			}
		}

		internal override IEnumerable<PropertyDefinition> FilterProperties()
		{
			return new List<PropertyDefinition>(1)
			{
				this.Property
			};
		}

		private readonly ComparisonOperator comparisonOperator;

		private readonly PropertyDefinition property;

		private readonly uint propertySize;
	}
}
