using System;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISetBroken
	{
		void SetBroken(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, params string[] setBrokenArgs);

		void SetBroken(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, Exception exception, params string[] setBrokenArgs);

		void ClearBroken();

		void RestartInstanceSoon(bool fPrepareToStop);

		void RestartInstanceNow(ReplayConfigChangeHints restartReason);

		void RestartInstanceSoonAdminVisible();

		bool IsBroken { get; }

		LocalizedString ErrorMessage { get; }
	}
}
