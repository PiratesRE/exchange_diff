using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(ModifiedEventType))]
	[KnownType(typeof(ModifiedEventType))]
	[XmlInclude(typeof(MovedCopiedEventType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(MovedCopiedEventType))]
	[DataContract(Name = "BaseObjectChangedEvent", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class BaseObjectChangedEventType : BaseNotificationEventType
	{
		public BaseObjectChangedEventType()
		{
		}

		public BaseObjectChangedEventType(NotificationTypeEnum notificationType) : base(notificationType)
		{
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string TimeStamp { get; set; }

		[IgnoreDataMember]
		[XmlElement("ItemId", typeof(ItemId))]
		[XmlElement("FolderId", typeof(FolderId))]
		public object ChangedObject
		{
			get
			{
				return this.changedObject;
			}
			set
			{
				this.changedObject = value;
			}
		}

		[DataMember(Name = "FolderId", EmitDefaultValue = false, IsRequired = false, Order = 2)]
		[XmlIgnore]
		public FolderId FolderId
		{
			get
			{
				return this.changedObject as FolderId;
			}
			set
			{
				this.changedObject = value;
			}
		}

		[DataMember(Name = "ItemId", EmitDefaultValue = false, Order = 3)]
		[XmlIgnore]
		public ItemId ItemId
		{
			get
			{
				return this.changedObject as ItemId;
			}
			set
			{
				this.changedObject = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public FolderId ParentFolderId { get; set; }

		private object changedObject;
	}
}
