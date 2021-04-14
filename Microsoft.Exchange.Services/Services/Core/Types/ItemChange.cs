using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "ItemChange", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ItemChangeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class ItemChange : StoreObjectChangeBase
	{
		[XmlElement("RecurringMasterItemId", typeof(RecurringMasterItemId))]
		[XmlElement("ItemId", typeof(ItemId))]
		[DataMember(Name = "ItemId", IsRequired = true)]
		[XmlElement("OccurrenceItemId", typeof(OccurrenceItemId))]
		public BaseItemId ItemId { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ChangesAlreadyProcessed { get; set; }
	}
}
