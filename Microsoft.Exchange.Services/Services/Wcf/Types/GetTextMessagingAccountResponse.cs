using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetTextMessagingAccountResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public TextMessagingAccount Data { get; set; }

		public override string ToString()
		{
			return string.Format("GetTextMessagingAccountResponse: {0}", this.Data);
		}
	}
}
