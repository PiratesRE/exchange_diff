using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "AlternateMailbox", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class AlternateMailbox
	{
		[DataMember(Name = "Type", IsRequired = true, Order = 1)]
		public string Type { get; set; }

		[DataMember(Name = "DisplayName", IsRequired = true, Order = 2)]
		public string DisplayName { get; set; }

		[DataMember(Name = "LegacyDN", IsRequired = true, Order = 3)]
		public string LegacyDN { get; set; }

		[DataMember(Name = "Server", IsRequired = true, Order = 4)]
		public string Server { get; set; }

		[DataMember(Name = "SmtpAddress", IsRequired = true, Order = 5)]
		public string SmtpAddress { get; set; }

		[DataMember(Name = "OwnerSmtpAddress", IsRequired = true, Order = 6)]
		public string OwnerSmtpAddress { get; set; }
	}
}
