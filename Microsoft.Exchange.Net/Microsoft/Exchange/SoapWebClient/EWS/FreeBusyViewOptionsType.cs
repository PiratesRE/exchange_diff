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
	public class FreeBusyViewOptionsType
	{
		public Duration TimeWindow;

		public int MergedFreeBusyIntervalInMinutes;

		[XmlIgnore]
		public bool MergedFreeBusyIntervalInMinutesSpecified;

		public FreeBusyViewType RequestedView;

		[XmlIgnore]
		public bool RequestedViewSpecified;
	}
}
