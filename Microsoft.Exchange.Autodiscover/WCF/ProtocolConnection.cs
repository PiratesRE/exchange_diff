using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "ProtocolConnection", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class ProtocolConnection
	{
		[DataMember(Name = "EncryptionMethod", IsRequired = true, Order = 3)]
		public string EncryptionMethod { get; set; }

		[DataMember(Name = "Hostname", IsRequired = true, Order = 1)]
		public string Hostname { get; set; }

		[DataMember(Name = "Port", IsRequired = true, Order = 2)]
		public int Port { get; set; }
	}
}
