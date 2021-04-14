using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IncludeAllPropertyFilter : IPropertyFilter
	{
		private IncludeAllPropertyFilter()
		{
		}

		public bool IncludeProperty(PropertyTag propertyTag)
		{
			return true;
		}

		public static readonly IPropertyFilter Instance = new IncludeAllPropertyFilter();
	}
}
