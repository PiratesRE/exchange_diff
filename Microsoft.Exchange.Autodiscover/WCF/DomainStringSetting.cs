using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "DomainStringSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public sealed class DomainStringSetting : DomainSetting
	{
		[DataMember]
		public string Value { get; set; }
	}
}
