using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ConflictResultsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ConflictResults
	{
		[XmlElement("Count")]
		[DataMember(Name = "Count", EmitDefaultValue = true, Order = 0)]
		public int Count { get; set; }
	}
}
