using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public interface IThrottleHelper
	{
		ThrottleSettingsBase Settings { get; }

		GlobalTunables Tunables { get; }

		string[] GetServersInGroup(string groupName);

		int GetServerVersion(string serverName);
	}
}
