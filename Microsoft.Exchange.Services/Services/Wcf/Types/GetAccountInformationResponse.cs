using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetAccountInformationResponse : OptionsResponseBase
	{
		public GetAccountInformationResponse()
		{
			this.AccountInfo = new GetAccountInformation();
		}

		[DataMember(IsRequired = true)]
		public GetAccountInformation AccountInfo { get; set; }

		public override string ToString()
		{
			return string.Format("GetAccountInformationResponse: {0}", this.AccountInfo);
		}
	}
}
