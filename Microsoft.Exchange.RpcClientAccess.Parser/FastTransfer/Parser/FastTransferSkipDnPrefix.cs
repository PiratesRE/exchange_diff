using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferSkipDnPrefix : BaseObject, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferSkipDnPrefix(IFastTransferProcessor<FastTransferUploadContext> enclosedObject)
		{
			this.enclosedObject = enclosedObject;
		}

		protected override void InternalDispose()
		{
			this.enclosedObject.Dispose();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferSkipDnPrefix>(this);
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			yield return new FastTransferStateMachine?(FastTransferPropertyValue.SkipPropertyIfExists(context, PropertyTag.DNPrefix));
			yield return new FastTransferStateMachine?(context.CreateStateMachine(this.enclosedObject));
			yield break;
		}

		private readonly IFastTransferProcessor<FastTransferUploadContext> enclosedObject;
	}
}
