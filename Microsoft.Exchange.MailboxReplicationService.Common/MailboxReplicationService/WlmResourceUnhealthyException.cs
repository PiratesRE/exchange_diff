using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WlmResourceUnhealthyException : ResourceReservationException
	{
		public WlmResourceUnhealthyException(string resourceName, string resourceType, string wlmResourceKey, int wlmResourceMetricType, double reportedLoadRatio, string reportedLoadState, string metric) : base(MrsStrings.ErrorWlmResourceUnhealthy1(resourceName, resourceType, wlmResourceKey, wlmResourceMetricType, reportedLoadRatio, reportedLoadState, metric))
		{
			this.resourceName = resourceName;
			this.resourceType = resourceType;
			this.wlmResourceKey = wlmResourceKey;
			this.wlmResourceMetricType = wlmResourceMetricType;
			this.reportedLoadRatio = reportedLoadRatio;
			this.reportedLoadState = reportedLoadState;
			this.metric = metric;
		}

		public WlmResourceUnhealthyException(string resourceName, string resourceType, string wlmResourceKey, int wlmResourceMetricType, double reportedLoadRatio, string reportedLoadState, string metric, Exception innerException) : base(MrsStrings.ErrorWlmResourceUnhealthy1(resourceName, resourceType, wlmResourceKey, wlmResourceMetricType, reportedLoadRatio, reportedLoadState, metric), innerException)
		{
			this.resourceName = resourceName;
			this.resourceType = resourceType;
			this.wlmResourceKey = wlmResourceKey;
			this.wlmResourceMetricType = wlmResourceMetricType;
			this.reportedLoadRatio = reportedLoadRatio;
			this.reportedLoadState = reportedLoadState;
			this.metric = metric;
		}

		protected WlmResourceUnhealthyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
			this.resourceType = (string)info.GetValue("resourceType", typeof(string));
			this.wlmResourceKey = (string)info.GetValue("wlmResourceKey", typeof(string));
			this.wlmResourceMetricType = (int)info.GetValue("wlmResourceMetricType", typeof(int));
			this.reportedLoadRatio = (double)info.GetValue("reportedLoadRatio", typeof(double));
			this.reportedLoadState = (string)info.GetValue("reportedLoadState", typeof(string));
			this.metric = (string)info.GetValue("metric", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resourceName", this.resourceName);
			info.AddValue("resourceType", this.resourceType);
			info.AddValue("wlmResourceKey", this.wlmResourceKey);
			info.AddValue("wlmResourceMetricType", this.wlmResourceMetricType);
			info.AddValue("reportedLoadRatio", this.reportedLoadRatio);
			info.AddValue("reportedLoadState", this.reportedLoadState);
			info.AddValue("metric", this.metric);
		}

		public string ResourceName
		{
			get
			{
				return this.resourceName;
			}
		}

		public string ResourceType
		{
			get
			{
				return this.resourceType;
			}
		}

		public string WlmResourceKey
		{
			get
			{
				return this.wlmResourceKey;
			}
		}

		public int WlmResourceMetricType
		{
			get
			{
				return this.wlmResourceMetricType;
			}
		}

		public double ReportedLoadRatio
		{
			get
			{
				return this.reportedLoadRatio;
			}
		}

		public string ReportedLoadState
		{
			get
			{
				return this.reportedLoadState;
			}
		}

		public string Metric
		{
			get
			{
				return this.metric;
			}
		}

		private readonly string resourceName;

		private readonly string resourceType;

		private readonly string wlmResourceKey;

		private readonly int wlmResourceMetricType;

		private readonly double reportedLoadRatio;

		private readonly string reportedLoadState;

		private readonly string metric;
	}
}
