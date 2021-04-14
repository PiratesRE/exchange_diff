using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[Serializable]
	public class QueryStringType
	{
		[XmlAttribute]
		public bool ResetCache;

		[XmlIgnore]
		public bool ResetCacheSpecified;

		[XmlAttribute]
		public bool ReturnHighlightTerms;

		[XmlIgnore]
		public bool ReturnHighlightTermsSpecified;

		[XmlAttribute]
		public bool ReturnDeletedItems;

		[XmlIgnore]
		public bool ReturnDeletedItemsSpecified;

		[XmlText]
		public string Value;
	}
}
