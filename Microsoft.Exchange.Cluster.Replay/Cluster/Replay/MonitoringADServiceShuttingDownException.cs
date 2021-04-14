using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringADServiceShuttingDownException : MonitoringADConfigException
	{
		public MonitoringADServiceShuttingDownException() : base(ReplayStrings.MonitoringADServiceShuttingDownException)
		{
		}

		public MonitoringADServiceShuttingDownException(Exception innerException) : base(ReplayStrings.MonitoringADServiceShuttingDownException, innerException)
		{
		}

		protected MonitoringADServiceShuttingDownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
