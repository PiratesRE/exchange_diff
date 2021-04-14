using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringServerSiteIsNullException : MonitoringADConfigException
	{
		public MonitoringServerSiteIsNullException(string serverName) : base(ReplayStrings.MonitoringServerSiteIsNullException(serverName))
		{
			this.serverName = serverName;
		}

		public MonitoringServerSiteIsNullException(string serverName, Exception innerException) : base(ReplayStrings.MonitoringServerSiteIsNullException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected MonitoringServerSiteIsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
