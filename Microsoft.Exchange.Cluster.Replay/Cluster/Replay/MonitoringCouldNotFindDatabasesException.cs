using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringCouldNotFindDatabasesException : MonitoringADConfigException
	{
		public MonitoringCouldNotFindDatabasesException(string serverName, string adError) : base(ReplayStrings.MonitoringCouldNotFindDatabasesException(serverName, adError))
		{
			this.serverName = serverName;
			this.adError = adError;
		}

		public MonitoringCouldNotFindDatabasesException(string serverName, string adError, Exception innerException) : base(ReplayStrings.MonitoringCouldNotFindDatabasesException(serverName, adError), innerException)
		{
			this.serverName = serverName;
			this.adError = adError;
		}

		protected MonitoringCouldNotFindDatabasesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.adError = (string)info.GetValue("adError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("adError", this.adError);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string AdError
		{
			get
			{
				return this.adError;
			}
		}

		private readonly string serverName;

		private readonly string adError;
	}
}
