using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferUploadDelimitedObject : BaseObject, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferUploadDelimitedObject(IFastTransferProcessor<FastTransferUploadContext> enclosedObject, PropertyTag startMarker, PropertyTag endMarker)
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
			return DisposeTracker.Get<FastTransferUploadDelimitedObject>(this);
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			context.DataInterface.ReadMarker(this.startMarker);
			yield return null;
			yield return new FastTransferStateMachine?(context.CreateStateMachine(this.enclosedObject));
			context.DataInterface.ReadMarker(this.endMarker);
			yield break;
		}

		private readonly IFastTransferProcessor<FastTransferUploadContext> enclosedObject;

		private readonly PropertyTag startMarker;

		private readonly PropertyTag endMarker;
	}
}
