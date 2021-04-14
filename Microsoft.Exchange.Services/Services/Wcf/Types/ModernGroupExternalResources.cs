using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ModernGroupExternalResources
	{
		[DataMember]
		public string SharePointUrl { get; set; }

		[DataMember]
		public string DocumentsUrl { get; set; }
	}
}
