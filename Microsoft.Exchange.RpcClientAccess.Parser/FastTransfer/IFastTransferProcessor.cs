using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFastTransferProcessor<TContext> : IDisposable where TContext : BaseObject
	{
		IEnumerator<FastTransferStateMachine?> Process(TContext context);
	}
}
