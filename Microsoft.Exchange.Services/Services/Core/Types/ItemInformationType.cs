using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ItemInformationType
	{
		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlIgnore]
		public string Start { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		[DateTimeString]
		[XmlIgnore]
		public string End { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 3)]
		public EnhancedLocationType Location { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 4)]
		[XmlIgnore]
		public SingleRecipientType Organizer { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 5)]
		public bool IsResponseRequested { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 6)]
		public string Subject { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 7)]
		public ItemId ConversationId { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 8)]
		public RecurrenceType Recurrence { get; set; }
	}
}
