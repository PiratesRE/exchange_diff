using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class HealthSetEscalationHelper
	{
		internal virtual HealthSetEscalationState LockHealthSetEscalationStateIfRequired(string healthSetName, EscalationState escalationState, string lockOwnerId)
		{
			return null;
		}

		internal virtual bool SetHealthSetEscalationState(string healthSetName, EscalationState escalationState, string lockOwnerId)
		{
			return false;
		}

		internal virtual void ExtendEscalationMessage(string healthSetName, ref string escalationMessage)
		{
		}
	}
}
