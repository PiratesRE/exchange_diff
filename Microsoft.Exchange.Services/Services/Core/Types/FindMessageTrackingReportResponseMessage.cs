using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("FindMessageTrackingReportResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FindMessageTrackingReportResponseMessage : ResponseMessage
	{
		public FindMessageTrackingReportResponseMessage()
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.FindMessageTrackingReportResponseMessage;
		}

		internal FindMessageTrackingReportResponseMessage(ServiceResultCode code, ServiceError error, FindMessageTrackingSearchResultType[] messageTrackingSearchResults, string[] diagnostics, string executedSearchScope, ArrayOfTrackingPropertiesType[] trackingErrors, TrackingPropertyType[] properties) : base(code, error)
		{
			this.MessageTrackingSearchResults = messageTrackingSearchResults;
			this.Diagnostics = diagnostics;
			this.ExecutedSearchScope = executedSearchScope;
			this.Errors = trackingErrors;
			this.Properties = properties;
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Diagnostics;

		[XmlArrayItem("MessageTrackingSearchResult", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public FindMessageTrackingSearchResultType[] MessageTrackingSearchResults;

		public string ExecutedSearchScope;

		[XmlArrayItem("ArrayOfTrackingPropertiesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ArrayOfTrackingPropertiesType[] Errors;

		[XmlArrayItem("TrackingPropertyType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
