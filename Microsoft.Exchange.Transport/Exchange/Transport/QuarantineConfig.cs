using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class QuarantineConfig
	{
		internal string Mailbox
		{
			get
			{
				return this.quarantineMailbox;
			}
		}

		internal static ADObjectId GetConfigObjectId()
		{
			ADObjectId adobjectId = QuarantineConfig.session.GetOrgContainerId();
			adobjectId = adobjectId.GetChildId("Transport Settings");
			adobjectId = adobjectId.GetChildId("Message Hygiene");
			return adobjectId.GetChildId("ContentFilterConfig");
		}

		internal bool Load()
		{
			ContentFilterConfig[] configObjects = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADObjectId configObjectId = QuarantineConfig.GetConfigObjectId();
				configObjects = QuarantineConfig.session.Find<ContentFilterConfig>(configObjectId, QueryScope.Base, null, null, 1);
			});
			if (!adoperationResult.Succeeded)
			{
				QuarantineConfig.LogConfigError(adoperationResult.Exception);
				return false;
			}
			if (configObjects == null || configObjects.Length == 0 || configObjects[0].QuarantineMailbox == null || configObjects[0].QuarantineMailbox.Value.Length == 0)
			{
				this.quarantineMailbox = string.Empty;
				return true;
			}
			this.quarantineMailbox = configObjects[0].QuarantineMailbox.ToString();
			ExTraceGlobals.DSNTracer.TraceDebug<string>((long)this.GetHashCode(), "Quarantine configuration was loaded, mailbox: {0}", this.quarantineMailbox);
			return true;
		}

		private static void LogConfigError(Exception configException)
		{
			ExTraceGlobals.DSNTracer.TraceError<string>(0L, "Failed to read quarantine-email address, exception message: {0}", configException.Message);
			string periodicKey = DateTime.UtcNow.Hour.ToString();
			QuarantineConfig.logger.LogEvent(TransportEventLogConstants.Tuple_DsnUnableToReadQuarantineConfig, periodicKey, null);
		}

		private static ExEventLog logger = new ExEventLog(ExTraceGlobals.DSNTracer.Category, TransportEventLog.GetEventSource());

		private static IConfigurationSession session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 55, "session", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\transport\\QuarantineConfig.cs");

		private string quarantineMailbox;
	}
}
