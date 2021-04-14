using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "ProtocolConnectionCollectionSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class ProtocolConnectionCollectionSetting : UserSetting
	{
		[DataMember(Name = "ProtocolConnections", IsRequired = true)]
		public ProtocolConnectionCollection ProtocolConnections { get; set; }
	}
}
