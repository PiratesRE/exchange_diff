using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "StringSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public sealed class StringSetting : UserSetting
	{
		[DataMember]
		public string Value { get; set; }
	}
}
