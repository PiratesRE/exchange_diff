using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class SubscriptionResponseData
	{
		public SubscriptionResponseData(string subscrptionId, bool successfullyCreated)
		{
			this.SubscriptionId = subscrptionId;
			this.SuccessfullyCreated = successfullyCreated;
		}

		[DataMember]
		public string SubscriptionId { get; set; }

		[DataMember]
		public bool SuccessfullyCreated { get; set; }

		[DataMember]
		public string ErrorInfo { get; set; }

		[DataMember]
		public bool SubscriptionExists { get; set; }
	}
}
