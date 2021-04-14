using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AddEventToMyCalendarRequest
	{
		[XmlElement("RecurringMasterItemId", typeof(RecurringMasterItemId))]
		[DataMember(Name = "ItemId", IsRequired = true)]
		[XmlElement("OccurrenceItemId", typeof(OccurrenceItemId))]
		[XmlElement("ItemId", typeof(ItemId))]
		public BaseItemId ItemId { get; set; }
	}
}
