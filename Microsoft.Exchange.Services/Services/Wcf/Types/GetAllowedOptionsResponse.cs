using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetAllowedOptionsResponse : BaseJsonResponse
	{
		[DataMember(IsRequired = true)]
		public string[] AllowedOptions { get; set; }
	}
}
