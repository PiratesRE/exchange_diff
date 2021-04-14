using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class TextMessagingAccount : SetTextMessagingAccountData
	{
		[DataMember]
		public CarrierData[] CarrierList { get; set; }

		[DataMember]
		public E164NumberData E164NotificationPhoneNumber { get; set; }

		[DataMember]
		public bool EasEnabled { get; set; }

		[DataMember]
		public bool NotificationPhoneNumberVerified { get; set; }

		[DataMember]
		public RegionData[] RegionList { get; set; }

		[DataMember]
		public CarrierData[] VoiceMailCarrierList { get; set; }
	}
}
