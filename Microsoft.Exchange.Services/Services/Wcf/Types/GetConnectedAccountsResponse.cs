using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetConnectedAccountsResponse : OptionsResponseBase
	{
		public GetConnectedAccountsResponse()
		{
			this.ConnectedAccountsInformation = new ConnectedAccountsInformation();
		}

		[DataMember(IsRequired = true)]
		public ConnectedAccountsInformation ConnectedAccountsInformation { get; set; }

		public override string ToString()
		{
			return string.Format("GetConnectedAccountsResponse: {0}", this.ConnectedAccountsInformation);
		}
	}
}
