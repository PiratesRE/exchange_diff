using System;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport
{
	internal class AgentLatencyTracker : IDisposable
	{
		public AgentLatencyTracker(IMExSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			this.session = session;
			this.eventComponent = LatencyComponent.None;
			if (LatencyTracker.ComponentLatencyTrackingEnabled)
			{
				this.session.Dispatcher.OnAgentInvokeStart += new AgentInvokeStartHandler(this.AgentInvokeStartHandler);
				this.session.Dispatcher.OnAgentInvokeEnd += new AgentInvokeEndHandler(this.AgentInvokeEndHandler);
				this.session.Dispatcher.OnAgentInvokeScheduled += new AgentInvokeScheduledHandler(this.AgentInvokeScheduledHandler);
				this.session.Dispatcher.OnAgentInvokeResumed += new AgentInvokeResumedHandler(this.AgentInvokeResumedHandler);
				this.subscribed = true;
			}
		}

		public static void RegisterMExRuntime(LatencyAgentGroup agentGroup, MExRuntime runtime)
		{
			string[] array = new string[runtime.AgentCount];
			for (int i = 0; i < runtime.AgentCount; i++)
			{
				array[i] = LoggingFormatter.EncodeAgentName(runtime.GetAgentName(i));
			}
			LatencyTracker.SetAgentNames(agentGroup, array);
		}

		public void Dispose()
		{
			if (this.subscribed)
			{
				this.session.Dispatcher.OnAgentInvokeStart -= new AgentInvokeStartHandler(this.AgentInvokeStartHandler);
				this.session.Dispatcher.OnAgentInvokeEnd -= new AgentInvokeEndHandler(this.AgentInvokeEndHandler);
				this.subscribed = false;
			}
			this.latencyTracker = null;
			this.session = null;
			GC.SuppressFinalize(this);
		}

		public virtual void BeginTrackLatency(LatencyComponent eventComponent, LatencyTracker latencyTracker)
		{
			if (!this.subscribed)
			{
				return;
			}
			this.eventComponent = eventComponent;
			this.latencyTracker = latencyTracker;
			LatencyTracker.BeginTrackLatency(eventComponent, latencyTracker);
		}

		public virtual void EndTrackLatency()
		{
			this.EndTrackLatency(true);
		}

		public void EndTrackLatency(bool mailItemAvailable)
		{
			if (!this.subscribed)
			{
				return;
			}
			if (mailItemAvailable)
			{
				LatencyTracker.EndTrackLatency(this.eventComponent, this.latencyTracker);
			}
			this.eventComponent = LatencyComponent.None;
			this.latencyTracker = null;
		}

		public void EndTrackingCurrentEvent()
		{
			this.EndTrackingCurrentEvent(this.latencyTracker);
			this.eventComponent = LatencyComponent.None;
			this.latencyTracker = null;
		}

		public void EndTrackingCurrentEvent(LatencyTracker tracker)
		{
			if (!this.subscribed)
			{
				return;
			}
			if (this.eventComponent != LatencyComponent.None)
			{
				if (this.session.CurrentAgent != null)
				{
					LatencyTracker.EndTrackLatency(this.eventComponent, this.session.CurrentAgent.SequenceNumber, tracker);
				}
				LatencyTracker.EndTrackLatency(this.eventComponent, tracker);
			}
		}

		private void AgentInvokeStartHandler(object source, IMExSession session)
		{
			if (this.eventComponent != LatencyComponent.None)
			{
				LatencyTracker.BeginTrackLatency(this.eventComponent, this.session.CurrentAgent.SequenceNumber, this.latencyTracker);
			}
		}

		private void AgentInvokeEndHandler(object dispatcher, IMExSession session)
		{
			if (this.eventComponent != LatencyComponent.None)
			{
				LatencyTracker.EndTrackLatency(this.eventComponent, this.session.CurrentAgent.SequenceNumber, this.latencyTracker);
			}
		}

		private void AgentInvokeScheduledHandler(object dispatcher, IMExSession session)
		{
			if (this.eventComponent != LatencyComponent.None)
			{
				LatencyTracker.BeginTrackLatency(LatencyComponent.MexRuntimeThreadpoolQueue, this.latencyTracker);
			}
		}

		private void AgentInvokeResumedHandler(object dispatcher, IMExSession session)
		{
			if (this.eventComponent != LatencyComponent.None)
			{
				LatencyTracker.EndTrackLatency(LatencyComponent.MexRuntimeThreadpoolQueue, this.latencyTracker);
			}
		}

		private IMExSession session;

		private bool subscribed;

		private LatencyTracker latencyTracker;

		private LatencyComponent eventComponent;
	}
}
