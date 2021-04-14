using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit.Schema.Mailbox;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxAuditWriter
	{
		public void Write(ExchangeMailboxAuditRecord record)
		{
			MailboxSession mailboxSession = this.GetMailboxSession(record.OrganizationId, record.MailboxGuid);
			AuditEventRecordAdapter auditEvent = new ItemOperationAuditEventRecordAdapter(record, mailboxSession.OrganizationId.ToString());
			mailboxSession.AuditMailboxAccess(auditEvent, true);
		}

		public void Write(ExchangeMailboxAuditGroupRecord record)
		{
			MailboxSession mailboxSession = this.GetMailboxSession(record.OrganizationId, record.MailboxGuid);
			AuditEventRecordAdapter auditEvent = new GroupOperationAuditEventRecordAdapter(record, mailboxSession.OrganizationId.ToString());
			mailboxSession.AuditMailboxAccess(auditEvent, true);
		}

		private MailboxSession GetMailboxSession(string organizationIdEncoded, Guid mailboxGuid)
		{
			CacheEntry<MailboxSession> cacheEntry;
			MailboxSession result;
			if (this.sessions.TryGetValue(mailboxGuid, DateTime.UtcNow, out cacheEntry))
			{
				result = cacheEntry.Value;
			}
			else
			{
				OrganizationId organizationId = AuditRecordDatabaseWriterVisitor.GetOrganizationId(organizationIdEncoded);
				ADSessionSettings adSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, null, false);
				ExchangePrincipal mailboxOwner = ExchangePrincipal.FromMailboxGuid(adSettings, mailboxGuid, RemotingOptions.AllowCrossSite, null);
				result = MailboxSession.OpenAsSystemService(mailboxOwner, CultureInfo.InvariantCulture, "Client=Management;Action=AuditLog");
			}
			return result;
		}

		private const string ClientInfoString = "Client=Management;Action=AuditLog";

		public const int SessionCacheSize = 32;

		public static TimeSpan SessionLifeTime = TimeSpan.FromMinutes(5.0);

		private readonly CacheWithExpiration<Guid, CacheEntry<MailboxSession>> sessions = new CacheWithExpiration<Guid, CacheEntry<MailboxSession>>(32, MailboxAuditWriter.SessionLifeTime, null);
	}
}
