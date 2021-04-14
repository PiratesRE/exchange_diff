using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregatedAccountInfo
	{
		[DataMember(Name = "Id")]
		public Guid MailboxGuid { get; set; }

		[DataMember]
		public SmtpAddress SmtpAddress { get; set; }

		[DataMember]
		[Obsolete("AccountRootId is no longer required for AggregatedAccountInfo.", true)]
		internal StoreId AccountRootId { get; set; }

		[DataMember]
		internal Guid RequestGuid { get; set; }

		public AggregatedAccountInfo(Guid mailboxGuid, SmtpAddress smtpAddress, Guid requestGuid)
		{
			Util.ThrowOnNullArgument(mailboxGuid, "mailboxGuid");
			Util.ThrowOnNullArgument(smtpAddress, "smtpAddress");
			this.MailboxGuid = mailboxGuid;
			this.SmtpAddress = smtpAddress;
			this.RequestGuid = requestGuid;
		}
	}
}
