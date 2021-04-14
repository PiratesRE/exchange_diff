using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferDownloadDelimitedObject : BaseObject, IFastTransferProcessor<FastTransferDownloadContext>, IDisposable
	{
		public FastTransferDownloadDelimitedObject(IFastTransferProcessor<FastTransferDownloadContext> enclosedObject, PropertyTag startMarker, PropertyTag endMarker)
		{
			this.enclosedObject = enclosedObject;
			this.startMarker = startMarker;
			this.endMarker = endMarker;
		}

		protected override void InternalDispose()
		{
			this.enclosedObject.Dispose();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferDownloadDelimitedObject>(this);
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			context.DataInterface.PutMarker(this.startMarker);
			yield return null;
			yield return new FastTransferStateMachine?(context.CreateStateMachine(this.enclosedObject));
			context.DataInterface.PutMarker(this.endMarker);
			yield break;
		}

		private readonly IFastTransferProcessor<FastTransferDownloadContext> enclosedObject;

		private readonly PropertyTag startMarker;

		private readonly PropertyTag endMarker;
	}
}
