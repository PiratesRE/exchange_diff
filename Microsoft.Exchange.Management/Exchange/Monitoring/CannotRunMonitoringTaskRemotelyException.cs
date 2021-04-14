using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRunMonitoringTaskRemotelyException : LocalizedException
	{
		public CannotRunMonitoringTaskRemotelyException(string remoteServerName) : base(Strings.CannotRunMonitoringTaskRemotelyException(remoteServerName))
		{
			this.remoteServerName = remoteServerName;
		}

		public CannotRunMonitoringTaskRemotelyException(string remoteServerName, Exception innerException) : base(Strings.CannotRunMonitoringTaskRemotelyException(remoteServerName), innerException)
		{
			this.remoteServerName = remoteServerName;
		}

		protected CannotRunMonitoringTaskRemotelyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.remoteServerName = (string)info.GetValue("remoteServerName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("remoteServerName", this.remoteServerName);
		}

		public string RemoteServerName
		{
			get
			{
				return this.remoteServerName;
			}
		}

		private readonly string remoteServerName;
	}
}
