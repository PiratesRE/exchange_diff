using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExcludingPropertyFilter : IPropertyFilter
	{
		internal ExcludingPropertyFilter(PropertyTag[] propertiesToExclude)
		{
			this.propertiesToExclude = propertiesToExclude;
		}

		internal ExcludingPropertyFilter(ICollection<PropertyTag> propertiesToExclude)
		{
			this.propertiesToExclude = propertiesToExclude;
		}

		public bool IncludeProperty(PropertyTag propertyTag)
		{
			return !this.propertiesToExclude.Contains(propertyTag);
		}

		private readonly ICollection<PropertyTag> propertiesToExclude;
	}
}
