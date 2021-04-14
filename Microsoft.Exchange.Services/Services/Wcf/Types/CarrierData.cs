using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CarrierData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string CarrierId { get; set; }

		[DataMember]
		public string CarrierName { get; set; }

		[DataMember]
		public bool HasSmtpGateway { get; set; }

		[DataMember]
		public UnifiedMessagingInfo UnifiedMessagingInfo;
	}
}
