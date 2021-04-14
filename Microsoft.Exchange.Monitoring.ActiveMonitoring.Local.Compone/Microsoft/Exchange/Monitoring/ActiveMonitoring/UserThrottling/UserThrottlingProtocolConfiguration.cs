using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UserThrottling
{
	public sealed class UserThrottlingProtocolConfiguration
	{
		public UserThrottlingProtocolConfiguration(string protocol, string process) : this(protocol, process, ExchangeComponent.UserThrottling.EscalationTeam, UserThrottlingProtocolConfiguration.DefaultLockedOutUsersAlertingThreshold, UserThrottlingProtocolConfiguration.DefaultLockedOutUsersAlertingDuration)
		{
		}

		public UserThrottlingProtocolConfiguration(string protocol, string process, string escalationTeam) : this(protocol, process, escalationTeam, UserThrottlingProtocolConfiguration.DefaultLockedOutUsersAlertingThreshold, UserThrottlingProtocolConfiguration.DefaultLockedOutUsersAlertingDuration)
		{
		}

		public UserThrottlingProtocolConfiguration(string protocol, string process, string escalationTeam, int lockedOutUsersAlertingThreshold, int lockedOutUsersAlertingSampleCount)
		{
			this.Protocol = protocol;
			this.Process = process;
			this.EscalationTeam = escalationTeam;
			this.LockedOutUsersAlertingThreshold = lockedOutUsersAlertingThreshold;
			this.LockedOutUsersAlertingSampleCount = lockedOutUsersAlertingSampleCount;
		}

		public string Protocol { get; private set; }

		public string Process { get; private set; }

		public string EscalationTeam { get; private set; }

		public int LockedOutUsersAlertingThreshold { get; private set; }

		public int LockedOutUsersAlertingSampleCount { get; private set; }

		private static readonly int DefaultLockedOutUsersAlertingThreshold = 10;

		private static readonly int DefaultLockedOutUsersAlertingDuration = 12;
	}
}
