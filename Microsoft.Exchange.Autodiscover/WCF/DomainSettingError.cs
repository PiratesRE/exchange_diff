using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "DomainSettingError", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class DomainSettingError
	{
		[DataMember(IsRequired = true)]
		public string SettingName { get; set; }

		[DataMember(IsRequired = true)]
		public ErrorCode ErrorCode { get; set; }

		[DataMember(IsRequired = true)]
		public string ErrorMessage { get; set; }
	}
}
