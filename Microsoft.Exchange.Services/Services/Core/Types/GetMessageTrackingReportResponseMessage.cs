using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetMessageTrackingReportResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetMessageTrackingReportResponseMessage : ResponseMessage
	{
		public GetMessageTrackingReportResponseMessage()
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetMessageTrackingReportResponseMessage;
		}

		internal GetMessageTrackingReportResponseMessage(ServiceResultCode code, ServiceError error, MessageTrackingReport messageTrackingReport, string[] diagnostics, ArrayOfTrackingPropertiesType[] trackingErrors, TrackingPropertyType[] properties) : base(code, error)
		{
			this.MessageTrackingReport = messageTrackingReport;
			this.Diagnostics = diagnostics;
			this.Errors = trackingErrors;
			this.Properties = properties;
		}

		public MessageTrackingReport MessageTrackingReport;

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Diagnostics;

		[XmlArrayItem("ArrayOfTrackingPropertiesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ArrayOfTrackingPropertiesType[] Errors;

		[XmlArrayItem("TrackingPropertyType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
