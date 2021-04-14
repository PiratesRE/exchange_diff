using System;
using Microsoft.Exchange.Common.HA;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class NoOpTagHandler : TagHandler
	{
		internal NoOpTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("NoOp", watcher, dbfi)
		{
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
