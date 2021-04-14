using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics
{
	internal static class LogSettings
	{
		public static bool IsMonitored(string name)
		{
			return LogSettings.monitoredEvents.Contains(name);
		}

		public const string DefaultServiceName = "DarRuntime";

		public const string DefaultComponentName = "Unknown";

		private static HashSet<string> monitoredEvents = new HashSet<string>
		{
			NotificationItem.GenerateResultName("DarRuntime", "ServiceLet", "RuntimeStartupFailed"),
			NotificationItem.GenerateResultName("DarRuntime", "TaskLifeCycle", "TaskFailedWorkloadSubmission"),
			NotificationItem.GenerateResultName("DarRuntime", "BindingTask", "BindingTask3"),
			NotificationItem.GenerateResultName("DarRuntime", "BindingTask", "BindingTask13"),
			NotificationItem.GenerateResultName("DarRuntime", "BindingTask", "BindingTask14"),
			NotificationItem.GenerateResultName("DarRuntime", "ComplianceService", "ComplianceService5"),
			NotificationItem.GenerateResultName("DarRuntime", "ComplianceService", "ComplianceService6"),
			NotificationItem.GenerateResultName("DarRuntime", "ComplianceService", "ComplianceService8"),
			NotificationItem.GenerateResultName("DarRuntime", "PolicyConfigChangeEventHandler", "PolicyConfigChangeEventHandler0"),
			NotificationItem.GenerateResultName("DarRuntime", "PolicyConfigChangeEventHandler", "PolicyConfigChangeEventHandler14"),
			NotificationItem.GenerateResultName("DarRuntime", "DarTask", "DarTask2"),
			NotificationItem.GenerateResultName("DarRuntime", "DarTask", "DarTask4"),
			NotificationItem.GenerateResultName("DarRuntime", "DarTask", "DarTask5"),
			NotificationItem.GenerateResultName("DarRuntime", "TaskGenerator", "TaskGenerator2"),
			NotificationItem.GenerateResultName("DarRuntime", "TaskGenerator", "TaskGenerator7"),
			NotificationItem.GenerateResultName("DarRuntime", "DarTaskManager", null)
		};
	}
}
