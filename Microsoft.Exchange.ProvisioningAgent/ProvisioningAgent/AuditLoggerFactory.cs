using System;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal static class AuditLoggerFactory
	{
		public static IAuditLog CreateForFFO(OrganizationId orgId)
		{
			return new FfoAuditLogger(orgId);
		}

		public static IAuditLog Create(ExchangePrincipal principal, ArbitrationMailboxStatus status)
		{
			if (status == ArbitrationMailboxStatus.R5 || status == ArbitrationMailboxStatus.UnableToKnow)
			{
				if (AuditLoggerFactory.Tracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					AuditLoggerFactory.Tracer.TraceDebug<string>(0L, "The log mailbox is on the local server. Hence writing mail directly to mailbox {0}.", principal.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				return new XsoAuditLogger(principal, AuditLoggerFactory.CanUsePartitionedLogs(status));
			}
			if (AuditFeatureManager.IsAdminAuditLocalQueueEnabled(principal))
			{
				if (AuditLoggerFactory.Tracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					AuditLoggerFactory.Tracer.TraceDebug<int, string>(0L, "Tenant arbitration mailbox version is {0}. Write audit record to the local queue. Mailbox {1}.", principal.MailboxInfo.Location.ServerVersion, principal.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				OrganizationId organizationId = principal.MailboxInfo.OrganizationId;
				string organizationId2 = (organizationId == null || organizationId.OrganizationalUnit == null || organizationId.ConfigurationUnit == null) ? null : Convert.ToBase64String(organizationId.GetBytes(Encoding.UTF8));
				return new UnifiedAdminAuditLog(organizationId2, AuditRecordFactory.GetOrgNameFromOrgId(organizationId));
			}
			if (AuditLoggerFactory.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				AuditLoggerFactory.Tracer.TraceDebug<int, string>(0L, "Tenant arbitration mailbox version is {0}. Use EWS to send CreateItem in mailbox {1}.", principal.MailboxInfo.Location.ServerVersion, principal.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			return new EwsAuditLogger(principal);
		}

		public static IAuditLog CreateAsync(ExchangePrincipal principal, ArbitrationMailboxStatus status)
		{
			if (AuditLoggerFactory.AsyncReceiver == null)
			{
				lock (AuditLoggerFactory.AsyncReceiverSyncroot)
				{
					if (AuditLoggerFactory.AsyncReceiver == null)
					{
						if (AuditLoggerFactory.Tracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							AuditLoggerFactory.Tracer.TraceDebug(0L, "Starting background async receiver thread");
						}
						AuditLoggerFactory.AsyncReceiver = new AsyncAuditReceiver();
						AuditLoggerFactory.AsyncReceiver.Start();
					}
				}
			}
			if (AuditLoggerFactory.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				AuditLoggerFactory.Tracer.TraceDebug<string>(0L, "Returning async logger for mailbox {0}", principal.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			return new AsyncAuditLogger(AuditLoggerFactory.Create(principal, status));
		}

		private static bool IsMailboxOnCurrentServer(ExchangePrincipal mailbox)
		{
			return 0 == string.Compare(LocalServerCache.LocalServerFqdn, mailbox.MailboxInfo.Location.ServerFqdn, StringComparison.OrdinalIgnoreCase);
		}

		private static bool CanUsePartitionedLogs(ArbitrationMailboxStatus status)
		{
			return status == ArbitrationMailboxStatus.E15;
		}

		private static readonly Trace Tracer = ExTraceGlobals.AdminAuditLogTracer;

		internal static AsyncAuditReceiver AsyncReceiver = null;

		private static object AsyncReceiverSyncroot = new object();
	}
}
