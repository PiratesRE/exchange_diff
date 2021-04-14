using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "DomainResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class DomainResponse : AutodiscoverResponse
	{
		public DomainResponse()
		{
			this.DomainSettings = new DomainSettingCollection();
			this.DomainSettingErrors = new DomainSettingErrorCollection();
		}

		[DataMember(Name = "RedirectTarget", IsRequired = false)]
		public string RedirectTarget { get; set; }

		[DataMember(Name = "DomainSettings", IsRequired = false)]
		public DomainSettingCollection DomainSettings { get; set; }

		[DataMember(Name = "DomainSettingErrors", IsRequired = false)]
		public DomainSettingErrorCollection DomainSettingErrors { get; set; }
	}
}
