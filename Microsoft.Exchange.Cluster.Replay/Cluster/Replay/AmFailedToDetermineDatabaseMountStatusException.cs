using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmFailedToDetermineDatabaseMountStatusException : AmDbActionException
	{
		public AmFailedToDetermineDatabaseMountStatusException(string serverName, string dbName) : base(ReplayStrings.AmFailedToDetermineDatabaseMountStatus(serverName, dbName))
		{
			this.serverName = serverName;
			this.dbName = dbName;
		}

		public AmFailedToDetermineDatabaseMountStatusException(string serverName, string dbName, Exception innerException) : base(ReplayStrings.AmFailedToDetermineDatabaseMountStatus(serverName, dbName), innerException)
		{
			this.serverName = serverName;
			this.dbName = dbName;
		}

		protected AmFailedToDetermineDatabaseMountStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
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
