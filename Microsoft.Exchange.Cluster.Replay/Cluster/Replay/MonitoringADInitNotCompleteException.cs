using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringADInitNotCompleteException : MonitoringADConfigException
	{
		public MonitoringADInitNotCompleteException() : base(ReplayStrings.MonitoringADInitNotCompleteException)
		{
		}

		public MonitoringADInitNotCompleteException(Exception innerException) : base(ReplayStrings.MonitoringADInitNotCompleteException, innerException)
		{
		}

		protected MonitoringADInitNotCompleteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
