using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[DataContract]
	internal class SoftDeletedRemovalData : IExtensibleDataObject
	{
		public SoftDeletedRemovalData(DirectoryIdentity sourceDatabase, DirectoryIdentity targetDatabase, DirectoryIdentity mailboxIdentity, long itemCount, DateTime? disconnectDate)
		{
			this.TargetDatabase = targetDatabase;
			this.MailboxIdentity = mailboxIdentity;
			this.ItemCount = itemCount;
			this.DisconnectDate = disconnectDate;
			this.SourceDatabase = sourceDatabase;
		}

		[DataMember]
		public DateTime? DisconnectDate { get; private set; }

		public ExtensionDataObject ExtensionData { get; set; }

		[DataMember]
		public long ItemCount { get; private set; }

		[DataMember]
		public DirectoryIdentity MailboxIdentity { get; private set; }

		[DataMember]
		public DirectoryIdentity SourceDatabase { get; private set; }

		[DataMember]
		public DirectoryIdentity TargetDatabase { get; private set; }
	}
}
