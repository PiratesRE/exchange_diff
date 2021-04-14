using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class DCAdminActionsLoggerSettings : ActivityContextLogFileSettings
	{
		protected override string LogTypeName
		{
			get
			{
				return "Admin Audit Logs for DC Admin Actions";
			}
		}

		protected override string LogSubFolderName
		{
			get
			{
				return "DCAdminActionsLog";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AdminAuditLogTracer;
			}
		}

		internal static DCAdminActionsLoggerSettings Load()
		{
			return new DCAdminActionsLoggerSettings();
		}

		private DCAdminActionsLoggerSettings()
		{
		}

		internal const string DCAdminActionsLoggerSubFolderName = "DCAdminActionsLog";
	}
}
