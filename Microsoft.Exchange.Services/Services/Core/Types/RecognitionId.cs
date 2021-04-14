using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "RecognitionIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RecognitionId
	{
		public RecognitionId()
		{
		}

		internal RecognitionId(Guid requestId)
		{
			this.RequestId = requestId;
		}

		[DataMember(IsRequired = true)]
		[XmlAttribute("RequestId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public Guid RequestId { get; set; }
	}
}
