using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMessagePropertyFilter : IPropertyFilter
	{
		bool IncludeRecipients { get; }

		bool IncludeAttachments { get; }
	}
}
