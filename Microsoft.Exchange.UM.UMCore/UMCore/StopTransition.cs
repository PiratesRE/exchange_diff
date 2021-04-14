using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class StopTransition : TransitionBase
	{
		internal StopTransition() : base(null, string.Empty, null, false, false, null)
		{
		}

		protected override void DoTransition(ActivityManager manager, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Executing null StopTransition.DoTransition.", new object[0]);
		}
	}
}
