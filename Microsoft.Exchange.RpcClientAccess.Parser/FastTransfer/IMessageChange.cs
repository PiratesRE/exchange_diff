using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMessageChange : IDisposable
	{
		IMessage Message { get; }

		int MessageSize { get; }

		bool IsAssociatedMessage { get; }

		IPropertyBag MessageHeaderPropertyBag { get; }

		IMessageChangePartial PartialChange { get; }
	}
}
