using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MoveRequestInfo
	{
		public MoveRequestInfo()
		{
			this.Message = LocalizedString.Empty;
		}

		[DataMember(IsRequired = true)]
		public Guid MailboxGuid { get; set; }

		[DataMember(IsRequired = true)]
		public SyncStage SyncStage { get; set; }

		[DataMember(IsRequired = true)]
		public int PercentComplete { get; set; }

		[DataMember]
		public ulong ItemsTransfered { get; set; }

		[DataMember]
		public ulong BytesTransfered { get; set; }

		[DataMember]
		public ulong BytesPerMinute { get; set; }

		[DataMember]
		public TransferProgressTracker ProgressTracker { get; set; }

		[DataMember]
		public int BadItemsEncountered { get; set; }

		[DataMember]
		public int LargeItemsEncountered { get; set; }

		[IgnoreDataMember]
		public LocalizedString Message
		{
			get
			{
				return CommonUtils.ByteDeserialize(this.MessageData);
			}
			set
			{
				this.MessageData = CommonUtils.ByteSerialize(value);
			}
		}

		[DataMember(Name = "Message")]
		public byte[] MessageData { get; set; }
	}
}
