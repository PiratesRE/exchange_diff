using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NonOwnerAccessResultRow : BaseRow
	{
		public NonOwnerAccessResultRow(MailboxAuditLogRecord searchResult)
		{
			this.NonOwnerAccessResult = searchResult;
		}

		internal NonOwnerAccessResultRow(Identity id, MailboxAuditLogRecord searchResult) : base(id, searchResult)
		{
			this.NonOwnerAccessResult = searchResult;
		}

		public MailboxAuditLogRecord NonOwnerAccessResult { get; private set; }

		[DataMember]
		public string Mailbox
		{
			get
			{
				return this.NonOwnerAccessResult.MailboxResolvedOwnerName;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastAccessed
		{
			get
			{
				return this.NonOwnerAccessResult.LastAccessed.Value.ToUniversalTime().UtcToUserDateTimeString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
