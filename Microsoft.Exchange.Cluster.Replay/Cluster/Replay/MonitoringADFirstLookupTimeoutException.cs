using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringADFirstLookupTimeoutException : MonitoringADConfigException
	{
		public MonitoringADFirstLookupTimeoutException(int timeoutMs) : base(ReplayStrings.MonitoringADFirstLookupTimeoutException(timeoutMs))
		{
			this.timeoutMs = timeoutMs;
		}

		public MonitoringADFirstLookupTimeoutException(int timeoutMs, Exception innerException) : base(ReplayStrings.MonitoringADFirstLookupTimeoutException(timeoutMs), innerException)
		{
			this.timeoutMs = timeoutMs;
		}

		protected MonitoringADFirstLookupTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
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
