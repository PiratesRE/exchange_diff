using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "ModifiedEvent", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ModifiedEventType : BaseObjectChangedEventType
	{
		public ModifiedEventType() : base(NotificationTypeEnum.ModifiedEvent)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public int UnreadCount
		{
			get
			{
				return this.unreadCount;
			}
			set
			{
				this.UnreadCountSpecified = true;
				this.unreadCount = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool UnreadCountSpecified { get; set; }

		private int unreadCount;
	}
}
