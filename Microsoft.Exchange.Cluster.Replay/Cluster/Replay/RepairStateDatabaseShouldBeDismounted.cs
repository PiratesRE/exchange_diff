using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RepairStateDatabaseShouldBeDismounted : RepairStateException
	{
		public RepairStateDatabaseShouldBeDismounted(string dbName, string mountedServer) : base(ReplayStrings.RepairStateDatabaseShouldBeDismounted(dbName, mountedServer))
		{
			this.dbName = dbName;
			this.mountedServer = mountedServer;
		}

		public RepairStateDatabaseShouldBeDismounted(string dbName, string mountedServer, Exception innerException) : base(ReplayStrings.RepairStateDatabaseShouldBeDismounted(dbName, mountedServer), innerException)
		{
			this.dbName = dbName;
			this.mountedServer = mountedServer;
		}

		protected RepairStateDatabaseShouldBeDismounted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.mountedServer = (string)info.GetValue("mountedServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("mountedServer", this.mountedServer);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string MountedServer
		{
			get
			{
				return this.mountedServer;
			}
		}

		private readonly string dbName;

		private readonly string mountedServer;
	}
}
