using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCertsInfoRequestWrapper
	{
		[DataMember(Name = "certRawData")]
		public string CertRawData { get; set; }

		[DataMember(Name = "isSend")]
		public bool IsSend { get; set; }
	}
}
