using System;
using System.Runtime.Serialization;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Exchange.Security.Authentication
{
	[DataContract(Name = "LiveIdBasicAuth", Namespace = "http://microsoft.com/exoanalytics")]
	public class LiveIdBasicAuthRawData : InsightRawData
	{
		[DataMember(Name = "RangeStart")]
		public DateTime RangeStart { get; set; }

		[DataMember(Name = "RangeEnd")]
		public DateTime RangeEnd { get; set; }

		[DataMember(Name = "ResultType")]
		public string ResultType { get; set; }

		[DataMember(Name = "ApplicationName")]
		public string ApplicationName { get; set; }

		[DataMember(Name = "ServerName")]
		public string ServerName { get; set; }

		[DataMember(Name = "CountDuringInterval")]
		public int CountDuringInterval
		{
			get
			{
				return this.countDuringInterval;
			}
			set
			{
				this.countDuringInterval = value;
			}
		}

		private int countDuringInterval = 1;
	}
}
