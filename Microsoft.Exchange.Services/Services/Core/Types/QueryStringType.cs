using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "QueryStringType")]
	[Serializable]
	public class QueryStringType
	{
		[XmlAttribute]
		[DataMember(IsRequired = false)]
		public bool ResetCache { get; set; }

		[DataMember(IsRequired = false)]
		[XmlAttribute]
		public bool ReturnHighlightTerms { get; set; }

		[XmlAttribute]
		[DataMember(IsRequired = false)]
		public bool ReturnDeletedItems { get; set; }

		[XmlAttribute]
		[DataMember(IsRequired = false)]
		public int MaxResultsCount { get; set; }

		[DataMember(IsRequired = false)]
		[XmlAttribute]
		public bool WaitForSearchComplete { get; set; }

		[DataMember(IsRequired = false)]
		[XmlAttribute]
		public bool OptimizedSearch { get; set; }

		[XmlText]
		[DataMember(IsRequired = true)]
		public string Value { get; set; }
	}
}
