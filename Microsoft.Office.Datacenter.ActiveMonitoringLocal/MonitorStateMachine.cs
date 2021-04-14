using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class MonitorStateMachine
	{
		internal MonitorStateMachine(MonitorStateTransition[] transitions)
		{
			this.GreenState = ServiceHealthStatus.Healthy;
			if (transitions != null && transitions.Length > 0)
			{
				for (int i = 1; i < transitions.Length; i++)
				{
					if (transitions[i].TransitionTimeout <= transitions[i - 1].TransitionTimeout)
					{
						throw new ArgumentException(string.Format("Transition timeout should be in increasing order. TransitionId={0}", i));
					}
				}
				this.transitions = transitions;
				this.IsEnabled = true;
			}
		}

		internal static MonitorStateTransition[] DefaultUnhealthyTransition
		{
			get
			{
				return MonitorStateMachine.defaultStateTransition;
			}
		}

		internal MonitorStateTransition[] Transitions
		{
			get
			{
				return this.transitions;
			}
		}

		internal ServiceHealthStatus GreenState { get; set; }

		internal bool IsEnabled { get; private set; }

		internal static MonitorStateTransition[] ConstructSimpleTransitions(int unhealthyTimeout, int unrecoverableTimeout)
		{
			return new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, unhealthyTimeout),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, unrecoverableTimeout)
			};
		}

		internal MonitorStateTransition GetTransitionInfo(int transitionId)
		{
			if (transitionId != -1 && transitionId < this.transitions.Length)
			{
				return this.transitions[transitionId];
			}
			throw new IndexOutOfRangeException("Transition id is out of range");
		}

		internal int GetNextTransitionId(int currentTransitionId)
		{
			int num = currentTransitionId + 1;
			if (num >= this.transitions.Length)
			{
				num = -1;
			}
			return num;
		}

		internal const int InvalidTransitionId = -1;

		private static MonitorStateTransition[] defaultStateTransition = new MonitorStateTransition[]
		{
			new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
		};

		private MonitorStateTransition[] transitions;
	}
}
