using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class AutoLatencyTracker : DisposeTrackableBase
	{
		public AutoLatencyTracker(AgentLatencyTracker agentLatencyTracker, LatencyComponent eventComponent, LatencyTracker tmiLatencyTracker)
		{
			ArgumentValidator.ThrowIfNull("agentLatencyTracker", agentLatencyTracker);
			if (eventComponent != LatencyComponent.None && tmiLatencyTracker != null)
			{
				this.agentLatencyTracker = agentLatencyTracker;
				this.agentLatencyTracker.BeginTrackLatency(eventComponent, tmiLatencyTracker);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AutoLatencyTracker>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.agentLatencyTracker != null)
			{
				this.agentLatencyTracker.EndTrackLatency();
			}
		}

		private readonly AgentLatencyTracker agentLatencyTracker;
	}
}
