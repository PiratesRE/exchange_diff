using System;
using System.Runtime.Serialization;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[DataContract(Name = "ScopeNotification", Namespace = "http://microsoft.com/exoanalytics")]
	public sealed class ScopeNotificationRawData : InsightRawData
	{
		[DataMember(Name = "NotificationName")]
		public string NotificationName { get; set; }

		[DataMember(Name = "ScopeName")]
		public string ScopeName { get; set; }

		[DataMember(Name = "ScopeType")]
		public string ScopeType { get; set; }

		[DataMember(Name = "HealthSetName")]
		public string HealthSetName { get; set; }

		[DataMember(Name = "HealthState")]
		public int HealthState { get; set; }

		[DataMember(Name = "MachineName")]
		public string MachineName { get; set; }

		[DataMember(Name = "SourceInstanceName")]
		public string SourceInstanceName { get; set; }

		[DataMember(Name = "SourceInstanceType")]
		public string SourceInstanceType { get; set; }

		[DataMember(Name = "IsMultiSourceInstance")]
		public bool IsMultiSourceInstance { get; set; }

		[DataMember(Name = "ExecutionStartTime")]
		public DateTime ExecutionStartTime { get; set; }

		[DataMember(Name = "ExecutionEndTime")]
		public DateTime ExecutionEndTime { get; set; }

		[DataMember(Name = "Error")]
		public string Error { get; set; }

		[DataMember(Name = "Exception")]
		public string Exception { get; set; }

		[DataMember(Name = "ExecutionContext")]
		public string ExecutionContext { get; set; }

		[DataMember(Name = "FailureContext")]
		public string FailureContext { get; set; }

		[DataMember(Name = "Data")]
		public string Data { get; set; }
	}
}
