using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class OccurrencesRangeType
	{
		[XmlAttribute]
		public DateTime Start;

		[XmlIgnore]
		public bool StartSpecified;

		[XmlAttribute]
		public DateTime End;

		[XmlIgnore]
		public bool EndSpecified;

		[XmlAttribute]
		public int Count;

		[XmlIgnore]
		public bool CountSpecified;

		[XmlAttribute]
		public bool CompareOriginalStartTime;

		[XmlIgnore]
		public bool CompareOriginalStartTimeSpecified;
	}
}
