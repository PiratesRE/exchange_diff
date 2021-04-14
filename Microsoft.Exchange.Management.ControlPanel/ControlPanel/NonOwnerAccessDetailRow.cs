using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NonOwnerAccessDetailRow : BaseRow
	{
		public NonOwnerAccessDetailRow(MailboxAuditLogEvent logEvent) : base(logEvent)
		{
			this.MailboxAuditLogEvent = logEvent;
		}

		internal NonOwnerAccessDetailRow(Identity id, MailboxAuditLogEvent searchResult) : base(id, searchResult)
		{
			this.MailboxAuditLogEvent = searchResult;
		}

		public MailboxAuditLogEvent MailboxAuditLogEvent { get; private set; }
	}
}
