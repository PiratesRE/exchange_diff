using System;
using System.Runtime.Serialization;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[DataContract(Name = "ProbeResult", Namespace = "http://microsoft.com/exoanalytics")]
	public sealed class ProbeResultRawData : InsightRawData
	{
		[DataMember(Name = "AMInstanceName")]
		public string AMInstanceName { get; set; }

		[DataMember(Name = "Latency")]
		public double Latency { get; set; }

		[DataMember(Name = "Error")]
		public string Error { get; set; }

		[DataMember(Name = "Exception")]
		public string Exception { get; set; }

		[DataMember(Name = "FailureCategory")]
		public int FailureCategory { get; set; }

		[DataMember(Name = "StateAttribute1")]
		public string StateAttribute1 { get; set; }

		[DataMember(Name = "StateAttribute2")]
		public string StateAttribute2 { get; set; }

		[DataMember(Name = "StateAttribute3")]
		public string StateAttribute3 { get; set; }

		[DataMember(Name = "StateAttribute4")]
		public string StateAttribute4 { get; set; }

		[DataMember(Name = "StateAttribute5")]
		public string StateAttribute5 { get; set; }

		[DataMember(Name = "StateAttribute6")]
		public double StateAttribute6 { get; set; }

		[DataMember(Name = "StateAttribute7")]
		public double StateAttribute7 { get; set; }

		[DataMember(Name = "StateAttribute8")]
		public double StateAttribute8 { get; set; }

		[DataMember(Name = "StateAttribute9")]
		public double StateAttribute9 { get; set; }

		[DataMember(Name = "StateAttribute10")]
		public double StateAttribute10 { get; set; }

		[DataMember(Name = "StateAttribute11")]
		public string StateAttribute11 { get; set; }

		[DataMember(Name = "StateAttribute12")]
		public string StateAttribute12 { get; set; }

		[DataMember(Name = "StateAttribute13")]
		public string StateAttribute13 { get; set; }

		[DataMember(Name = "StateAttribute14")]
		public string StateAttribute14 { get; set; }

		[DataMember(Name = "StateAttribute15")]
		public string StateAttribute15 { get; set; }

		[DataMember(Name = "StateAttribute18")]
		public double StateAttribute18 { get; set; }

		[DataMember(Name = "StateAttribute21")]
		public string StateAttribute21 { get; set; }

		[DataMember(Name = "StateAttribute22")]
		public string StateAttribute22 { get; set; }

		[DataMember(Name = "StateAttribute23")]
		public string StateAttribute23 { get; set; }

		[DataMember(Name = "ExecutionContext")]
		public string ExecutionContext { get; set; }

		[DataMember(Name = "FailureContext")]
		public string FailureContext { get; set; }

		[DataMember(Name = "ResultId")]
		public int ResultId { get; set; }

		[DataMember(Name = "IsCortex")]
		public bool IsCortex { get; set; }

		[DataMember(Name = "DataPartition")]
		public string DataPartition { get; set; }
	}
}
