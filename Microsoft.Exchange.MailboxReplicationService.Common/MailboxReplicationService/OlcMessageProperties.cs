using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class OlcMessageProperties : ItemPropertiesBase
	{
		private static ExDateTime ConvertToExDateTime(DateTime dateTime)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, dateTime);
		}

		public override void Apply(MailboxSession session, Item item)
		{
			PersistablePropertyBag propertyBag = item.PropertyBag;
			propertyBag[MessageItemSchema.IsRead] = this.IsRead;
			propertyBag[MessageItemSchema.WasEverRead] = this.IsRead;
			propertyBag[MessageItemSchema.IsDraft] = this.IsDraft;
			propertyBag[ItemSchema.ReceivedTime] = OlcMessageProperties.ConvertToExDateTime(this.ReceivedDate);
			propertyBag[StoreObjectSchema.LastModifiedTime] = OlcMessageProperties.ConvertToExDateTime(this.LastModifiedDate);
			IconIndex iconIndex = IconIndex.Default;
			OlcMessageType messageType = this.MessageType;
			switch (messageType)
			{
			case OlcMessageType.CALENDAR_REQUEST:
				iconIndex = IconIndex.AppointmentMeet;
				break;
			case OlcMessageType.CALENDAR_CANCEL:
				iconIndex = IconIndex.AppointmentMeetCancel;
				break;
			case OlcMessageType.CALENDAR_ACCEPTED:
				iconIndex = IconIndex.AppointmentMeetYes;
				break;
			case OlcMessageType.CALENDAR_TENTATIVE:
				iconIndex = IconIndex.AppointmentMeetMaybe;
				break;
			case OlcMessageType.CALENDAR_DECLINED:
				iconIndex = IconIndex.AppointmentMeetNo;
				break;
			default:
				if (messageType == OlcMessageType.SMS || messageType == OlcMessageType.MMS)
				{
					iconIndex = IconIndex.SmsDelivered;
				}
				break;
			}
			if (this.IsReplied && iconIndex == IconIndex.Default)
			{
				iconIndex = IconIndex.MailReplied;
			}
			if (this.IsForwarded && iconIndex == IconIndex.Default)
			{
				iconIndex = IconIndex.MailForwarded;
			}
			propertyBag[ItemSchema.IconIndex] = iconIndex;
			propertyBag[MessageItemSchema.MessageAnswered] = (this.IsReplied || this.IsForwarded);
			propertyBag[ItemSchema.Importance] = this.Importance;
			propertyBag[ItemSchema.Sensitivity] = this.Sensitivity;
			if (this.MapiFlagStatus == FlagStatus.Flagged)
			{
				propertyBag[ItemSchema.FlagStatus] = 2;
				propertyBag[ItemSchema.IsToDoItem] = true;
				propertyBag[ItemSchema.ItemColor] = 6;
				propertyBag[ItemSchema.PercentComplete] = 0.0;
			}
		}

		[DataMember]
		public bool IsRead { get; set; }

		[DataMember]
		public bool IsSent { get; set; }

		[DataMember]
		public bool IsDraft { get; set; }

		[DataMember]
		public int OlcMessageTypeInt { get; set; }

		public OlcMessageType MessageType
		{
			get
			{
				return (OlcMessageType)this.OlcMessageTypeInt;
			}
			set
			{
				this.OlcMessageTypeInt = (int)value;
			}
		}

		[DataMember]
		public DateTime ReceivedDate { get; set; }

		[DataMember]
		public DateTime LastModifiedDate { get; set; }

		[DataMember]
		public bool IsReplied { get; set; }

		[DataMember]
		public bool IsForwarded { get; set; }

		[DataMember]
		public bool HasAttachments { get; set; }

		[DataMember]
		public int? ImapUid { get; set; }

		[DataMember]
		public string LegacyPopId { get; set; }

		[DataMember]
		public int MessageSize { get; set; }

		[DataMember]
		public bool IsConfirmedAsJunk { get; set; }

		[DataMember]
		public int SensitivityInt { get; set; }

		public Sensitivity Sensitivity
		{
			get
			{
				return (Sensitivity)this.SensitivityInt;
			}
			set
			{
				this.SensitivityInt = (int)value;
			}
		}

		[DataMember]
		public int ImportanceInt { get; set; }

		public Importance Importance
		{
			get
			{
				return (Importance)this.ImportanceInt;
			}
			set
			{
				this.ImportanceInt = (int)value;
			}
		}

		[DataMember]
		public int SenderIdStatusInt { get; set; }

		public SenderIdStatus SenderIdStatus
		{
			get
			{
				return (SenderIdStatus)this.SenderIdStatusInt;
			}
			set
			{
				this.SenderIdStatusInt = (int)value;
			}
		}

		[DataMember]
		public int? ItemColorInt { get; set; }

		public ItemColor? ItemColor
		{
			get
			{
				int? itemColorInt = this.ItemColorInt;
				if (itemColorInt == null)
				{
					return null;
				}
				return new ItemColor?((ItemColor)itemColorInt.GetValueOrDefault());
			}
			set
			{
				ItemColor? itemColor = value;
				this.ItemColorInt = ((itemColor != null) ? new int?((int)itemColor.GetValueOrDefault()) : null);
			}
		}

		[DataMember]
		public int? MapiFlagStatusInt { get; set; }

		public FlagStatus? MapiFlagStatus
		{
			get
			{
				int? mapiFlagStatusInt = this.MapiFlagStatusInt;
				if (mapiFlagStatusInt == null)
				{
					return null;
				}
				return new FlagStatus?((FlagStatus)mapiFlagStatusInt.GetValueOrDefault());
			}
			set
			{
				FlagStatus? flagStatus = value;
				this.MapiFlagStatusInt = ((flagStatus != null) ? new int?((int)flagStatus.GetValueOrDefault()) : null);
			}
		}

		[DataMember]
		public DateTime? FlagDueDate { get; set; }

		[DataMember]
		public string FlagSubject { get; set; }

		[DataMember]
		public CategorySettings[] CategoryList { get; set; }

		[DataMember]
		public int OriginalFolderReasonInt { get; set; }

		[DataMember]
		public int SenderClassInt { get; set; }

		[DataMember]
		public int AuthenticationTypeInt { get; set; }

		[DataMember]
		public int TrustedSourceInt { get; set; }

		[DataMember]
		public int SenderAuthResultInt { get; set; }

		[DataMember]
		public int PhishingInt { get; set; }

		[DataMember]
		public int WarningInfoInt { get; set; }

		[DataMember]
		public bool SenderIsSafe { get; set; }

		[DataMember]
		public bool IsPraEmailPresent { get; set; }

		[DataMember]
		public string PraEmail { get; set; }

		[DataMember]
		public bool Unsubscribe { get; set; }

		[DataMember]
		public short ActionsTakenInt { get; set; }

		[DataMember]
		public OlcLegacyMessageProperties LegacyProperties { get; set; }
	}
}
