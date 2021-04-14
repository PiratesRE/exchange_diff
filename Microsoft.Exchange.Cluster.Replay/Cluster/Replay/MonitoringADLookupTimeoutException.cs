using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringADLookupTimeoutException : MonitoringADConfigException
	{
		public MonitoringADLookupTimeoutException(int timeoutMs) : base(ReplayStrings.MonitoringADLookupTimeoutException(timeoutMs))
		{
			this.timeoutMs = timeoutMs;
		}

		public MonitoringADLookupTimeoutException(int timeoutMs, Exception innerException) : base(ReplayStrings.MonitoringADLookupTimeoutException(timeoutMs), innerException)
		{
			this.timeoutMs = timeoutMs;
		}

		protected MonitoringADLookupTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.timeoutMs = (int)info.GetValue("timeoutMs", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("timeoutMs", this.timeoutMs);
		}

		public int TimeoutMs
		{
			get
			{
				return this.timeoutMs;
			}
		}

		private readonly int timeoutMs;
	}
}
