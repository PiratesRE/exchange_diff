using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class SyncNowNotification
	{
		[DataMember(IsRequired = true)]
		public Guid MailboxGuid { get; set; }

		[DataMember(IsRequired = true)]
		public Guid MdbGuid { get; set; }

		[DataMember(IsRequired = true)]
		public int Flags { get; set; }

		public SyncNowNotificationFlags SyncNowNotificationFlags
		{
			get
			{
				return (SyncNowNotificationFlags)this.Flags;
			}
			set
			{
				this.Flags = (int)value;
			}
		}
	}
}
