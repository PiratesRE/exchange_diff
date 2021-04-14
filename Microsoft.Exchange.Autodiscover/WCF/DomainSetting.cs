using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "DomainSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[KnownType(typeof(DomainStringSetting))]
	public abstract class DomainSetting
	{
		public DomainSetting()
		{
		}

		[DataMember(IsRequired = true)]
		public string Name { get; set; }
	}
}
