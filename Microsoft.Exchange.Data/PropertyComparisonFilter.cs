using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class PropertyComparisonFilter : QueryFilter
	{
		public PropertyComparisonFilter(ComparisonOperator comparisonOperator, PropertyDefinition property1, PropertyDefinition property2)
		{
			if (property1 == null)
			{
				throw new ArgumentNullException("property1");
			}
			if (property2 == null)
			{
				throw new ArgumentNullException("property2");
			}
			this.property1 = property1;
			this.property2 = property2;
			this.comparisonOperator = comparisonOperator;
		}

		public PropertyDefinition Property1
		{
			get
			{
				return this.property1;
			}
		}

		public PropertyDefinition Property2
		{
			get
			{
				return this.property2;
			}
		}

		public ComparisonOperator ComparisonOperator
		{
			get
			{
				return this.comparisonOperator;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.property1.Name);
			sb.Append(" ");
			sb.Append(this.comparisonOperator.ToString());
			sb.Append(" ");
			sb.Append(this.property2.Name);
			sb.Append(")");
		}

		public override bool Equals(object obj)
		{
			PropertyComparisonFilter propertyComparisonFilter = obj as PropertyComparisonFilter;
			return propertyComparisonFilter != null && propertyComparisonFilter.GetType() == base.GetType() && this.comparisonOperator == propertyComparisonFilter.comparisonOperator && this.property1.Equals(propertyComparisonFilter.property1) && this.property2.Equals(propertyComparisonFilter.property2);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == null)
			{
				this.hashCode = new int?(base.GetType().GetHashCode() ^ this.comparisonOperator.GetHashCode() ^ this.property1.GetHashCode() ^ this.property2.GetHashCode());
			}
			return this.hashCode.Value;
		}

		internal override IEnumerable<PropertyDefinition> FilterProperties()
		{
			return new List<PropertyDefinition>(2)
			{
				this.Property1,
				this.Property2
			};
		}

		private int? hashCode;

		private readonly PropertyDefinition property1;

		private readonly PropertyDefinition property2;

		private readonly ComparisonOperator comparisonOperator;
	}
}
