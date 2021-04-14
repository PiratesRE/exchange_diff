using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GroupMailboxProperties
	{
		[DataMember]
		public int SubscribersCount { get; set; }

		[DataMember]
		public bool CanUpdateAutoSubscribeFlag { get; set; }

		[DataMember]
		public int LanguageLCID { get; set; }
	}
}
