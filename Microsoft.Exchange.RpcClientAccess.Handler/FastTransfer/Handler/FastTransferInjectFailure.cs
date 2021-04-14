using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferInjectFailure : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IDisposable
	{
		public FastTransferInjectFailure(Exception failureException) : base(true)
		{
			this.failureException = failureException;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			yield return this.ThrowFailureException();
			yield break;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferInjectFailure>(this);
		}

		private FastTransferStateMachine? ThrowFailureException()
		{
			throw this.failureException;
		}

		private readonly Exception failureException;
	}
}
