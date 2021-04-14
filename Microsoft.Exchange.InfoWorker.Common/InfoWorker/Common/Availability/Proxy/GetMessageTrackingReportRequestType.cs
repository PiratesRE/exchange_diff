using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.3038")]
	[Serializable]
	public class GetMessageTrackingReportRequestType : BaseRequestType
	{
		public string Scope
		{
			get
			{
				return this.scopeField;
			}
			set
			{
				this.scopeField = value;
			}
		}

		public MessageTrackingReportTemplateType ReportTemplate
		{
			get
			{
				return this.reportTemplateField;
			}
			set
			{
				this.reportTemplateField = value;
			}
		}

		public EmailAddressType RecipientFilter
		{
			get
			{
				return this.recipientFilterField;
			}
			set
			{
				this.recipientFilterField = value;
			}
		}

		public string MessageTrackingReportId
		{
			get
			{
				return this.messageTrackingReportIdField;
			}
			set
			{
				this.messageTrackingReportIdField = value;
			}
		}

		public bool ReturnQueueEvents
		{
			get
			{
				return this.returnQueueEventsField;
			}
			set
			{
				this.returnQueueEventsField = value;
			}
		}

		[XmlIgnore]
		public bool ReturnQueueEventsSpecified
		{
			get
			{
				return this.returnQueueEventsFieldSpecified;
			}
			set
			{
				this.returnQueueEventsFieldSpecified = value;
			}
		}

		public string DiagnosticsLevel
		{
			get
			{
				return this.diagnosticsLevelField;
			}
			set
			{
				this.diagnosticsLevelField = value;
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

		private string scopeField;

		private MessageTrackingReportTemplateType reportTemplateField;

		private EmailAddressType recipientFilterField;

		private string messageTrackingReportIdField;

		private bool returnQueueEventsField;

		private bool returnQueueEventsFieldSpecified;

		private string diagnosticsLevelField;

		private TrackingPropertyType[] propertiesField;
	}
}
