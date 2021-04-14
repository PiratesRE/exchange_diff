using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMessageIteratorClient : IDisposable
	{
		IMessage UploadMessage(bool isAssociatedMessage);
	}
}
