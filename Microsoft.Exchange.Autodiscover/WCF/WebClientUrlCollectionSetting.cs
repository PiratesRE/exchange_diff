using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "WebClientUrlCollectionSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class WebClientUrlCollectionSetting : UserSetting
	{
		[DataMember(Name = "WebClientUrls", IsRequired = true)]
		public WebClientUrlCollection WebClientUrls { get; set; }
	}
}
