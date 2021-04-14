using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Serializable]
	internal sealed class HealthSetEscalationState
	{
		internal HealthSetEscalationState(string healthSetName, EscalationState escalationState, DateTime stateTransitionTime)
		{
			this.HealthSetName = healthSetName;
			this.EscalationState = escalationState;
			this.StateTransitionTime = stateTransitionTime;
		}

		internal string HealthSetName { get; private set; }

		internal EscalationState EscalationState { get; set; }

		internal DateTime StateTransitionTime { get; set; }

		internal string LockOwnerId { get; set; }

		internal DateTime LockedUntilTime { get; set; }

		internal void ResetToGreen()
		{
			this.EscalationState = EscalationState.Green;
			this.StateTransitionTime = DateTime.UtcNow;
			this.LockOwnerId = null;
			this.LockedUntilTime = DateTime.MinValue;
		}
	}
}
