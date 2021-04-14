using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class TimeChangeType
	{
		[XmlElement(DataType = "duration")]
		public string Offset;

		[XmlElement("RelativeYearlyRecurrence", typeof(RelativeYearlyRecurrencePatternType))]
		[XmlElement("AbsoluteDate", typeof(DateTime), DataType = "date")]
		public object Item;

		[XmlElement(DataType = "time")]
		public DateTime Time;

		[XmlAttribute]
		public string TimeZoneName;
	}
}
