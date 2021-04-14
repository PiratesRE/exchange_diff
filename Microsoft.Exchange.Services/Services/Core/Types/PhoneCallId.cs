using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "PhoneCallIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PhoneCallId
	{
		public PhoneCallId()
		{
		}

		internal PhoneCallId(string id)
		{
			this.Id = id;
		}

		[DataMember(IsRequired = true)]
		[XmlAttribute("Id", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string Id { get; set; }
	}
}
