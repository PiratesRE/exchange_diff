using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IncludeAllMessagePropertyFilter : IMessagePropertyFilter, IPropertyFilter
	{
		public bool IncludeProperty(PropertyTag propertyTag)
		{
			return true;
		}

		public bool IncludeRecipients
		{
			get
			{
				return true;
			}
		}

		public bool IncludeAttachments
		{
			get
			{
				return true;
			}
		}

		public static readonly IMessagePropertyFilter Instance = new IncludeAllMessagePropertyFilter();
	}
}
