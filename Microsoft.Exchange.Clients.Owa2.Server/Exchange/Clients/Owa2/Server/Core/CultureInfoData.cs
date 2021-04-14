using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "CultureInfoData", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CultureInfoData
	{
		[DataMember(Name = "Name", IsRequired = false)]
		public string Name { get; set; }

		[DataMember(Name = "NativeName", IsRequired = false)]
		public string NativeName { get; set; }

		[DataMember(Name = "LCID", IsRequired = false)]
		public int LCID { get; set; }
	}
}
