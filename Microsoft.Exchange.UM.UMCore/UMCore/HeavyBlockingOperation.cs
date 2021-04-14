using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class HeavyBlockingOperation : IUMHeavyBlockingOperation
	{
		internal HeavyBlockingOperation(ActivityManager manager, BaseUMCallSession vo, FsmAction action, TransitionBase originalTransition)
		{
			this.manager = manager;
			this.vo = vo;
			this.action = action;
			this.originalTransition = originalTransition;
		}

		public void Execute()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Starting heavy blocking operation: {0}...", new object[]
			{
				this.action
			});
			this.autoEvent = this.action.Execute(this.manager, this.vo);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Completed heavy blocking operation: {0}.", new object[]
			{
				this.action
			});
		}

		internal void CompleteHeavyBlockingOperation()
		{
			this.originalTransition.ProcessAutoEvent(this.manager, this.vo, this.autoEvent);
		}

		private ActivityManager manager;

		private BaseUMCallSession vo;

		private FsmAction action;

		private TransitionBase originalTransition;

		private TransitionBase autoEvent;
	}
}
