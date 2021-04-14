using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class MonitorStateTransition
	{
		public MonitorStateTransition(ServiceHealthStatus toState, TimeSpan transitionTimeout)
		{
			this.ToState = toState;
			this.TransitionTimeout = transitionTimeout;
		}

		public MonitorStateTransition(ServiceHealthStatus toState, int transitionTimeoutSeconds) : this(toState, TimeSpan.FromSeconds((double)transitionTimeoutSeconds))
		{
		}

		public ServiceHealthStatus ToState { get; private set; }

		public TimeSpan TransitionTimeout { get; private set; }
	}
}
