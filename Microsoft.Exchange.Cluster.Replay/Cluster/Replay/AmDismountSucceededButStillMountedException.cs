using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDismountSucceededButStillMountedException : AmDbActionException
	{
		public AmDismountSucceededButStillMountedException(string serverName, string dbName) : base(ReplayStrings.AmDismountSucceededButStillMounted(serverName, dbName))
		{
			this.serverName = serverName;
			this.dbName = dbName;
		}

		public AmDismountSucceededButStillMountedException(string serverName, string dbName, Exception innerException) : base(ReplayStrings.AmDismountSucceededButStillMounted(serverName, dbName), innerException)
		{
			this.serverName = serverName;
			this.dbName = dbName;
		}

		protected AmDismountSucceededButStillMountedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.dbName = (string)info.GetValue("dbName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("dbName", this.dbName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		private readonly string serverName;

		private readonly string dbName;
	}
}
