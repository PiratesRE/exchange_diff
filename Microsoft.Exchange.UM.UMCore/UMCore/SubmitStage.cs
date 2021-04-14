using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class SubmitStage : SynchronousPipelineStageBase
	{
		internal MessageItem MessageToSubmit
		{
			get
			{
				if (this.message == null)
				{
					this.message = base.WorkItem.Message.MessageToSubmit;
				}
				return this.message;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SubmitStage>(this);
		}

		private MessageItem message;
	}
}
