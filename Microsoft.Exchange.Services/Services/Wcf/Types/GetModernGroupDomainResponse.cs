using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "GetModernGroupDomainResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetModernGroupDomainResponse : BaseJsonResponse
	{
		[DataMember(Name = "Domain", IsRequired = false)]
		public string Domain { get; set; }

		internal GetModernGroupDomainResponse(string domain)
		{
			this.Domain = domain;
		}
	}
}
