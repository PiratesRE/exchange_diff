using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "MovedCopiedEvent", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class MovedCopiedEventType : BaseObjectChangedEventType
	{
		public MovedCopiedEventType()
		{
		}

		public MovedCopiedEventType(NotificationTypeEnum notificationType) : base(notificationType)
		{
		}

		[XmlElement("OldFolderId", typeof(FolderId))]
		[XmlElement("OldItemId", typeof(ItemId))]
		[IgnoreDataMember]
		public object OldObject
		{
			get
			{
				return this.oldObject;
			}
			set
			{
				this.oldObject = value;
			}
		}

		[DataMember(Name = "OldFolderId", EmitDefaultValue = false, IsRequired = false, Order = 1)]
		[XmlIgnore]
		public FolderId OldFolderId
		{
			get
			{
				return this.oldObject as FolderId;
			}
			set
			{
				this.oldObject = value;
			}
		}

		[DataMember(Name = "OldItemId", EmitDefaultValue = false, Order = 2)]
		[XmlIgnore]
		public ItemId OldItemId
		{
			get
			{
				return this.oldObject as ItemId;
			}
			set
			{
				this.oldObject = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public FolderId OldParentFolderId { get; set; }

		private object oldObject;
	}
}
