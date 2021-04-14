using System;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic.AuditLog;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit.Schema;
using Microsoft.Office.Compliance.Audit.Schema.Admin;
using Microsoft.Office.Compliance.Audit.Schema.Mailbox;
using Microsoft.Office.Compliance.Audit.Schema.Monitoring;
using Microsoft.Office.Compliance.Audit.Schema.SharePoint;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AuditRecordDatabaseWriterVisitor : IAuditRecordVisitor
	{
		public AuditRecordDatabaseWriterVisitor(Trace tracer)
		{
			this.adminAuditWriter = new AdminAuditWriter(tracer);
			this.mailboxAuditWriter = new MailboxAuditWriter();
		}

		public void Visit(ExchangeAdminAuditRecord record)
		{
			this.adminAuditWriter.Write(record);
		}

		public void Visit(ExchangeMailboxAuditRecord record)
		{
			this.mailboxAuditWriter.Write(record);
		}

		public void Visit(ExchangeMailboxAuditGroupRecord record)
		{
			this.mailboxAuditWriter.Write(record);
		}

		public void Visit(SharePointAuditRecord record)
		{
			throw new NotSupportedException();
		}

		public void Visit(SyntheticProbeAuditRecord record)
		{
			throw new NotSupportedException();
		}

		internal static OrganizationId GetOrganizationId(string organizationIdEncoded)
		{
			if (string.IsNullOrWhiteSpace(organizationIdEncoded))
			{
				return OrganizationId.ForestWideOrgId;
			}
			byte[] bytes;
			try
			{
				bytes = Convert.FromBase64String(organizationIdEncoded);
			}
			catch (FormatException)
			{
				throw new InvalidOrganizationException(organizationIdEncoded);
			}
			OrganizationId organizationId;
			OrganizationId.TryCreateFromBytes(bytes, Encoding.UTF8, out organizationId);
			if (organizationId == null)
			{
				throw new InvalidOrganizationException(organizationIdEncoded);
			}
			return organizationId;
		}

		private readonly AdminAuditWriter adminAuditWriter;

		private readonly MailboxAuditWriter mailboxAuditWriter;
	}
}
