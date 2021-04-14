using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class GenericMountFailureTagHandler : TagHandler
	{
		internal GenericMountFailureTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("GenericMountFailureTagHandler", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcGenericMountFailure9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtGenericMountFailure9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcGenericMountFailure9b);
			}
		}

		internal override bool IsTPRMoveTheActiveRecoveryAction
		{
			get
			{
				return false;
			}
		}

		internal override void ActiveRecoveryActionInternal()
		{
		}

		internal override void PassiveRecoveryActionInternal()
		{
		}
	}
}
