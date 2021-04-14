using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceRequestResume : ReplicaInstanceQueuedItem
	{
		public override string Name
		{
			get
			{
				return ReplayStrings.ResumeOperationName;
			}
		}

		public ReplicaInstanceRequestResume(ReplicaInstance instance) : base(instance)
		{
		}

		internal DatabaseCopyActionFlags Flags { get; set; }

		protected override void CheckOperationApplicable()
		{
			if (base.ReplicaInstance.CurrentContext.IsFailoverPending())
			{
				throw new ReplayServiceResumeInvalidDuringMoveException(base.DbCopyName);
			}
		}

		protected override void ExecuteInternal()
		{
			base.ExecuteInternal();
			base.ReplicaInstance.RequestResume(this.Flags);
		}
	}
}
