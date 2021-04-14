using System;
using Microsoft.Exchange.Common.HA;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class ExpectedDismountTagHandler : TagHandler
	{
		internal ExpectedDismountTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("ExpectedDismountTagHandler", watcher, dbfi)
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
