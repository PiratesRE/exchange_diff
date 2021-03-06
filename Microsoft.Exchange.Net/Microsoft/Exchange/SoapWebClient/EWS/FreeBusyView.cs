using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class FreeBusyView
	{
		public FreeBusyViewType FreeBusyViewType;

		public string MergedFreeBusy;

		[XmlArrayItem(IsNullable = false)]
		public CalendarEvent[] CalendarEventArray;

		public WorkingHours WorkingHours;
	}
}
