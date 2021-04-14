using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "ExchangeVersion", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public enum ExchangeVersion
	{
		[EnumMember]
		Exchange2010,
		[EnumMember]
		Exchange2010_SP1,
		[EnumMember]
		Exchange2010_SP2,
		[EnumMember]
		Exchange2012,
		[EnumMember]
		Exchange2013,
		[EnumMember]
		Exchange2013_SP1
	}
}
