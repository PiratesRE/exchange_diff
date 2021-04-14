using System;
using System.Runtime.Serialization;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Exchange.Autodiscover
{
	[DataContract(Name = "AutoDiscover", Namespace = "http://microsoft.com/exoanalytics")]
	internal class AutoDiscoverRawData : InsightRawData
	{
		[DataMember(Name = "RequestTime")]
		public string RequestTime { get; set; }

		[DataMember(Name = "UserAgent")]
		public string UserAgent { get; set; }

		[DataMember(Name = "SoapAction")]
		public string SoapAction { get; set; }

		[DataMember(Name = "HttpStatus")]
		public string HttpStatus { get; set; }

		[DataMember(Name = "ErrorCode")]
		public string ErrorCode { get; set; }

		[DataMember(Name = "GenericErrors")]
		public string GenericErrors { get; set; }
	}
}
