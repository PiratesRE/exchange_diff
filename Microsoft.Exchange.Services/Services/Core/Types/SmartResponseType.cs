using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(CancelCalendarItemType))]
	[KnownType(typeof(ReplyAllToItemType))]
	[KnownType(typeof(ReplyToItemType))]
	[XmlInclude(typeof(CancelCalendarItemType))]
	[XmlInclude(typeof(ForwardItemType))]
	[XmlInclude(typeof(ReplyAllToItemType))]
	[XmlInclude(typeof(ReplyToItemType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(ForwardItemType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SmartResponseType : SmartResponseBaseType
	{
		[DataMember(EmitDefaultValue = false)]
		public BodyContentType NewBodyContent { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false)]
		public ItemId UpdateResponseItemId { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false)]
		public int ReferenceItemDocumentId { get; set; }
	}
}
