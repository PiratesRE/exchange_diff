using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "ReminderItemAction", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ReminderItemActionType
	{
		[DataMember]
		[XmlElement("ActionType", Order = 1)]
		public ReminderActionType ActionType { get; set; }

		[DataMember]
		[XmlElement("ItemId", Order = 2)]
		public ItemId ItemId { get; set; }

		[XmlElement("NewReminderTime", Order = 3)]
		[DataMember]
		public string NewReminderTime { get; set; }
	}
}
