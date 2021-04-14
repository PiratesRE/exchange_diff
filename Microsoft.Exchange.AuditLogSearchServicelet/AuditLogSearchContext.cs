using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.AuditLogSearchServicelet;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	internal static class AuditLogSearchContext
	{
		internal static ITopologyConfigurationSession ConfigurationSession { get; private set; } = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 61, ".cctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\AuditLogSearch\\Program\\AuditLogSearchContext.cs");

		internal static Server Localhost
		{
			get
			{
				if (AuditLogSearchContext.localhost == null)
				{
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						AuditLogSearchContext.localhost = AuditLogSearchContext.ConfigurationSession.FindLocalServer();
						if (!AuditLogSearchContext.localhost.IsMailboxServer)
						{
							ExTraceGlobals.ServiceletTracer.TraceError<Server>(83371L, "server {0} does not have mailbox role, not running log search service", AuditLogSearchContext.localhost);
							AuditLogSearchContext.localhost = null;
							throw new InvalidOperationException("Only mailbox servers should be running service");
						}
					});
					if (!adoperationResult.Succeeded)
					{
						ExTraceGlobals.ServiceletTracer.TraceError(67552L, "Could not query ad for local server");
						throw (adoperationResult.Exception is LocalServerNotFoundException) ? adoperationResult.Exception : new InvalidOperationException("Failed to find local server information.", adoperationResult.Exception);
					}
				}
				return AuditLogSearchContext.localhost;
			}
		}

		internal static MicrosoftExchangeRecipient Sender
		{
			get
			{
				if (AuditLogSearchContext.sender == null)
				{
					AuditLogSearchContext.sender = AuditLogSearchContext.ConfigurationSession.FindMicrosoftExchangeRecipient();
				}
				return AuditLogSearchContext.sender;
			}
		}

		public const string AuditLogSearchServiceFullName = "MSExchange AuditLogSearch";

		internal static readonly ExEventLog EventLogger = new ExEventLog(AuditLogSearchItemSchema.BasePropertyGuid, "MSExchange AuditLogSearch");

		private static Server localhost;

		private static MicrosoftExchangeRecipient sender;
	}
}
