using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class ChangeHighlightsType
	{
		public bool HasLocationChanged;

		[XmlIgnore]
		public bool HasLocationChangedSpecified;

		public string Location;

		public bool HasStartTimeChanged;

		[XmlIgnore]
		public bool HasStartTimeChangedSpecified;

		public DateTime Start;

		[XmlIgnore]
		public bool StartSpecified;

		public bool HasEndTimeChanged;

		[XmlIgnore]
		public bool HasEndTimeChangedSpecified;

		public DateTime End;

		[XmlIgnore]
		public bool EndSpecified;
	}
}
