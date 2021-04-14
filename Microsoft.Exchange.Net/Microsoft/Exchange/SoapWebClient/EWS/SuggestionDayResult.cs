using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class SuggestionDayResult
	{
		public DateTime Date;

		public SuggestionQuality DayQuality;

		[XmlArrayItem(IsNullable = false)]
		public Suggestion[] SuggestionArray;
	}
}
