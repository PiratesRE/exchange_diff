using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetSendAddressResponse : OptionsResponseBase
	{
		public GetSendAddressResponse()
		{
			this.SendAddressCollection = new SendAddressCollection();
		}

		[DataMember(IsRequired = true)]
		public SendAddressCollection SendAddressCollection { get; set; }

		public override string ToString()
		{
			return string.Format("GetSendAddressResponse: {0}", this.SendAddressCollection);
		}
	}
}
