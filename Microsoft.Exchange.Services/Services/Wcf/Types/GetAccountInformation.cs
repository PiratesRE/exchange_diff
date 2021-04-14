using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetAccountInformation : SetUserData
	{
		[DataMember]
		public MyAccountStatisticsData Statistics { get; set; }

		[DataMember]
		public MyAccountMailboxData Mailbox { get; set; }

		[DataMember]
		public CountryData[] CountryList { get; set; }
	}
}
