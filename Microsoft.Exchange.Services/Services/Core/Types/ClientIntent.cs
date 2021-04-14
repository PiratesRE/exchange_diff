using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ClientIntentType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public sealed class ClientIntent
	{
		[DataMember(IsRequired = true, Order = 1)]
		[XmlElement]
		public ItemId ItemId { get; set; }

		[DataMember(IsRequired = false, Order = 2)]
		[XmlElement]
		public int? Intent { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false, Order = 3)]
		public int? ItemVersion { get; set; }

		[DataMember(IsRequired = false, Order = 4)]
		[XmlElement]
		public bool WouldRepair { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false, Order = 5)]
		public ClientIntentMeetingInquiryAction PredictedAction { get; set; }
	}
}
