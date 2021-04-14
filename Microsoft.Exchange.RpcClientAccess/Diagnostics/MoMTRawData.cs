using System;
using System.Runtime.Serialization;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	[DataContract(Name = "MoMT", Namespace = "http://microsoft.com/exoanalytics")]
	internal class MoMTRawData : InsightRawData
	{
		[DataMember(Name = "DateTimeUtc")]
		public string DateTimeUtc { get; set; }

		[DataMember(Name = "ClientName")]
		public string ClientName { get; set; }

		[DataMember(Name = "OrganizationInfo")]
		public string OrganizationInfo { get; set; }

		[DataMember(Name = "Failures")]
		public string Failures { get; set; }
	}
}
