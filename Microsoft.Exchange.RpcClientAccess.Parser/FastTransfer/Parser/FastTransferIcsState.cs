using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferIcsState : FastTransferObject, IFastTransferProcessor<FastTransferDownloadContext>, IFastTransferProcessor<FastTransferUploadContext>, IDisposable
	{
		public FastTransferIcsState(IIcsState icsState) : base(true)
		{
			this.icsState = icsState;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferDownloadContext>.Process(FastTransferDownloadContext context)
		{
			context.DataInterface.PutMarker(PropertyTag.IncrSyncStateBegin);
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.icsState.PropertyBag, FastTransferIcsState.AllIcsStateProperties)
			{
				SkipPropertyError = true
			}));
			context.DataInterface.PutMarker(PropertyTag.IncrSyncStateEnd);
			yield break;
		}

		IEnumerator<FastTransferStateMachine?> IFastTransferProcessor<FastTransferUploadContext>.Process(FastTransferUploadContext context)
		{
			context.DataInterface.ReadMarker(PropertyTag.IncrSyncStateBegin);
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.icsState.PropertyBag, FastTransferIcsState.AllIcsStateProperties)));
			context.DataInterface.ReadMarker(PropertyTag.IncrSyncStateEnd);
			this.icsState.Flush();
			yield break;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.icsState);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferIcsState>(this);
		}

		private readonly IIcsState icsState;

		public static readonly PropertyTag CnsetSeen = new PropertyTag(1737883906U);

		public static readonly PropertyTag CnsetSeenAssociated = new PropertyTag(1742340354U);

		public static readonly PropertyTag IdsetGiven = new PropertyTag(1075249410U);

		public static readonly PropertyTag CnsetRead = new PropertyTag(1741816066U);

		public static readonly PropertyTag[] AllIcsStateProperties = new PropertyTag[]
		{
			FastTransferIcsState.CnsetSeen,
			FastTransferIcsState.CnsetSeenAssociated,
			FastTransferIcsState.IdsetGiven,
			FastTransferIcsState.CnsetRead
		};
	}
}
