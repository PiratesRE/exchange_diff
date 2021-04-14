using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmServiceMonitorSystemShutdownException : LocalizedException
	{
		public AmServiceMonitorSystemShutdownException(string serviceName) : base(ReplayStrings.AmServiceMonitorSystemShutdownException(serviceName))
		{
			this.serviceName = serviceName;
		}

		public AmServiceMonitorSystemShutdownException(string serviceName, Exception innerException) : base(ReplayStrings.AmServiceMonitorSystemShutdownException(serviceName), innerException)
		{
			this.serviceName = serviceName;
		}

		protected AmServiceMonitorSystemShutdownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceName = (string)info.GetValue("serviceName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceName", this.serviceName);
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		private readonly string serviceName;
	}
}
