using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AndPropertyFilter : IPropertyFilter
	{
		internal AndPropertyFilter(IEnumerable<IPropertyFilter> propertyFilters)
		{
			Util.ThrowOnNullArgument(propertyFilters, "propertyFilters");
			this.propertyFilters = propertyFilters;
		}

		public bool IncludeProperty(PropertyTag propertyTag)
		{
			foreach (IPropertyFilter propertyFilter in this.propertyFilters)
			{
				if (!propertyFilter.IncludeProperty(propertyTag))
				{
					return false;
				}
			}
			return true;
		}

		private readonly IEnumerable<IPropertyFilter> propertyFilters;
	}
}
