using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IFastTransferDataInterface : IDisposable
	{
		void NotifyCanSplitBuffers();
	}
}
