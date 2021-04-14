using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class DeliverySettings : ItemPropertiesBase
	{
		[DataMember]
		public int AutoReplyModeInt { get; set; }

		public OlcAutoReplyMode AutoReplyMode
		{
			get
			{
				return (OlcAutoReplyMode)this.AutoReplyModeInt;
			}
			set
			{
				this.AutoReplyModeInt = (int)value;
			}
		}

		[DataMember]
		public DateTime? AutoReplyStartTime { get; set; }

		[DataMember]
		public DateTime? AutoReplyEndTime { get; set; }

		[DataMember]
		public string AutoReplyMessage { get; set; }

		[DataMember]
		public int AutoReplyMessageFormatInt { get; set; }

		public OlcAutoReplyFormat AutoReplyMessageFormat
		{
			get
			{
				return (OlcAutoReplyFormat)this.AutoReplyMessageFormatInt;
			}
			set
			{
				this.AutoReplyMessageFormatInt = (int)value;
			}
		}

		[DataMember]
		public bool BulkMailDeletion { get; set; }

		[DataMember]
		public uint BulkMailProtectionLevelUInt { get; set; }

		[DataMember]
		public int ForwardingModeInt { get; set; }

		public OlcForwardingMode ForwardingMode
		{
			get
			{
				return (OlcForwardingMode)this.ForwardingModeInt;
			}
			set
			{
				this.ForwardingModeInt = (int)value;
			}
		}

		[DataMember]
		public string[] ForwardingList { get; set; }

		[DataMember]
		public DateTime? MailDeliveryBlackoutExpiration { get; set; }
	}
}
