using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetMessageTrackingReportRequestType : BaseRequestType
	{
		public string Scope;

		public MessageTrackingReportTemplateType ReportTemplate;

		public EmailAddressType RecipientFilter;

		public string MessageTrackingReportId;

		public bool ReturnQueueEvents;

		[XmlIgnore]
		public bool ReturnQueueEventsSpecified;

		public string DiagnosticsLevel;

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
