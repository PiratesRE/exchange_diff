using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Logging
{
	internal static class LogSettings
	{
		public static bool IsMonitored(string name)
		{
			return LogSettings.monitoredEvents.Contains(name);
		}

		public const string DefaultServiceName = "EDiscovery";

		public const string DefaultComponentName = "Unknown";

		private static HashSet<string> monitoredEvents = new HashSet<string>
		{
			NotificationItem.GenerateResultName("EDiscovery", "Error", "Error"),
			NotificationItem.GenerateResultName(ExchangeComponent.EdiscoveryProtocol.Name, "Search.FailureMonitor", null),
			NotificationItem.GenerateResultName(ExchangeComponent.EdiscoveryProtocol.Name, "Mailbox.FailureMonitor", null)
		};
	}
}
