using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "WebClientUrl", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class WebClientUrl
	{
		[DataMember(Name = "AuthenticationMethods", IsRequired = true, Order = 1)]
		public string AuthenticationMethods { get; set; }

		[DataMember(Name = "Url", IsRequired = true, Order = 2)]
		public string Url { get; set; }
	}
}
