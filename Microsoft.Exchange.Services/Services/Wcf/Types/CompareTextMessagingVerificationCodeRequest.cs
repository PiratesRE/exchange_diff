using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CompareTextMessagingVerificationCodeRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public string VerificationCode { get; set; }

		public override string ToString()
		{
			return string.Format("CompareTextMessagingVerificationCodeRequest: VerificationCode = {0}", this.VerificationCode);
		}
	}
}
