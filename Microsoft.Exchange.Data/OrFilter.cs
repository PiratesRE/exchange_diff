using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class OrFilter : CompositeFilter
	{
		public OrFilter(params QueryFilter[] filters) : this(false, filters)
		{
		}

		public OrFilter(bool ignoreWhenVerifyingMaxDepth, params QueryFilter[] filters) : base(ignoreWhenVerifyingMaxDepth, filters)
		{
		}

		public OrFilter(bool ignoreWhenVerifyingMaxDepth, bool isAtomic, params QueryFilter[] filters) : base(ignoreWhenVerifyingMaxDepth, filters)
		{
			base.IsAtomic = isAtomic;
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(|(");
			foreach (QueryFilter queryFilter in base.Filters)
			{
				queryFilter.ToString(sb);
			}
			sb.Append("))");
		}

		public override QueryFilter CloneWithPropertyReplacement(IDictionary<PropertyDefinition, PropertyDefinition> propertyMap)
		{
			return new OrFilter(true, base.CloneFiltersWithPropertyReplacement(propertyMap));
		}
	}
}
