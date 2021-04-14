using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class AndFilter : CompositeFilter
	{
		public AndFilter(params QueryFilter[] filters) : this(false, filters)
		{
		}

		public AndFilter(bool ignoreWhenVerifyingMaxDepth, params QueryFilter[] filters) : base(ignoreWhenVerifyingMaxDepth, filters)
		{
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(&(");
			foreach (QueryFilter queryFilter in base.Filters)
			{
				queryFilter.ToString(sb);
			}
			sb.Append("))");
		}

		public override QueryFilter CloneWithPropertyReplacement(IDictionary<PropertyDefinition, PropertyDefinition> propertyMap)
		{
			return new AndFilter(true, base.CloneFiltersWithPropertyReplacement(propertyMap));
		}
	}
}
