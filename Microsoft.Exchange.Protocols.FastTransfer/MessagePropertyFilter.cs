using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MessagePropertyFilter : ExcludingPropertyFilter, IMessagePropertyFilter, IPropertyFilter
	{
		public MessagePropertyFilter(ICollection<PropertyTag> excludedPropertyTags) : base(excludedPropertyTags)
		{
		}

		public bool IncludeRecipients
		{
			get
			{
				return base.IncludeProperty(PropertyTag.MessageRecipients);
			}
		}

		public bool IncludeAttachments
		{
			get
			{
				return base.IncludeProperty(PropertyTag.MessageAttachments);
			}
		}
	}
}
