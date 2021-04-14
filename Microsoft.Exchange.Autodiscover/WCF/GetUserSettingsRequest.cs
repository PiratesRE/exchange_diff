using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class GetUserSettingsRequest : AutodiscoverRequest
	{
		[DataMember(Name = "Users", IsRequired = true, Order = 1)]
		public UserCollection Users { get; set; }

		[DataMember(Name = "RequestedSettings", IsRequired = true, Order = 2)]
		public RequestedSettingCollection RequestedSettings { get; set; }

		[DataMember(Name = "RequestedVersion", IsRequired = false, Order = 3)]
		public ExchangeVersion? RequestedVersion { get; set; }
	}
}
