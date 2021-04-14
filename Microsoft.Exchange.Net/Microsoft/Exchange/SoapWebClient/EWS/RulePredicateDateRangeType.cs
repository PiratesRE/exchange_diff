using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class RulePredicateDateRangeType
	{
		public DateTime StartDateTime;

		[XmlIgnore]
		public bool StartDateTimeSpecified;

		public DateTime EndDateTime;

		[XmlIgnore]
		public bool EndDateTimeSpecified;
	}
}
