using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetMessageTrackingReportResponseMessageType : ResponseMessage
	{
		public MessageTrackingReportType MessageTrackingReport
		{
			get
			{
				return this.messageTrackingReportField;
			}
			set
			{
				this.messageTrackingReportField = value;
			}
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Diagnostics
		{
			get
			{
				return this.diagnosticsField;
			}
			set
			{
				this.diagnosticsField = value;
			}
		}

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ArrayOfTrackingPropertiesType[] Errors
		{
			get
			{
				return this.errorsField;
			}
			set
			{
				this.errorsField = value;
			}
		}

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public TrackingPropertyType[] Properties
		{
			get
			{
				return this.propertiesField;
			}
			set
			{
				this.propertiesField = value;
			}
		}

		private MessageTrackingReportType messageTrackingReportField;

		private string[] diagnosticsField;

		private ArrayOfTrackingPropertiesType[] errorsField;

		private TrackingPropertyType[] propertiesField;
	}
}
