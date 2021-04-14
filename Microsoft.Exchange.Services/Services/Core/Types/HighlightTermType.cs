using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Term")]
	[Serializable]
	public class HighlightTermType
	{
		[DataMember(Order = 1)]
		public string Scope { get; set; }

		[DataMember(Order = 2)]
		public string Value { get; set; }
	}
}
