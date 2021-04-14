using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmPreMountCallbackFailedNoReplicaInstanceException : AmServerException
	{
		public AmPreMountCallbackFailedNoReplicaInstanceException(string dbName, string server) : base(ReplayStrings.AmPreMountCallbackFailedNoReplicaInstanceException(dbName, server))
		{
			this.dbName = dbName;
			this.server = server;
		}

		public AmPreMountCallbackFailedNoReplicaInstanceException(string dbName, string server, Exception innerException) : base(ReplayStrings.AmPreMountCallbackFailedNoReplicaInstanceException(dbName, server), innerException)
		{
			this.dbName = dbName;
			this.server = server;
		}

		protected AmPreMountCallbackFailedNoReplicaInstanceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("server", this.server);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string dbName;

		private readonly string server;
	}
}
