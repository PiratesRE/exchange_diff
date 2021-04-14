using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class PushNotificationSetupConfig
	{
		public PushNotificationSetupConfig(PushNotificationApp[] installableBySetup, PushNotificationApp[] installableBySetupDedicated, string[] retiredBySetup, string[] retiredBySetupDedicated, string acsHierarchyNode, Dictionary<string, string> fallbackPartitionPerApp)
		{
			ArgumentValidator.ThrowIfNull("installableBySetup", installableBySetup);
			ArgumentValidator.ThrowIfNull("installableBySetupDedicated", installableBySetupDedicated);
			ArgumentValidator.ThrowIfNull("retiredBySetup", retiredBySetup);
			ArgumentValidator.ThrowIfNull("retiredBySetupDedicated", retiredBySetupDedicated);
			ArgumentValidator.ThrowIfNull("fallbackPartitionPerApp", fallbackPartitionPerApp);
			this.InstallableBySetup = installableBySetup;
			this.InstallableBySetupDedicated = installableBySetupDedicated;
			this.RetiredBySetup = retiredBySetup;
			this.RetiredBySetupDedicated = retiredBySetupDedicated;
			this.AcsHierarchyNode = acsHierarchyNode;
			this.FallbackPartitionMapping = fallbackPartitionPerApp;
		}

		public PushNotificationApp[] InstallableBySetup { get; private set; }

		public PushNotificationApp[] InstallableBySetupDedicated { get; private set; }

		public string[] RetiredBySetup { get; private set; }

		public string[] RetiredBySetupDedicated { get; private set; }

		public string AcsHierarchyNode { get; private set; }

		public Dictionary<string, string> FallbackPartitionMapping { get; private set; }

		public const string ExchangeOnlineDefaultAcsHierarchyNodeName = "exo";
	}
}
