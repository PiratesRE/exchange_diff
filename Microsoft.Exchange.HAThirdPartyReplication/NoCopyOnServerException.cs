using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoCopyOnServerException : ThirdPartyReplicationException
	{
		public NoCopyOnServerException(Guid dbId, string dbName, string serverName) : base(ThirdPartyReplication.NoCopyOnServer(dbId, dbName, serverName))
		{
			this.dbId = dbId;
			this.dbName = dbName;
			this.serverName = serverName;
		}

		public NoCopyOnServerException(Guid dbId, string dbName, string serverName, Exception innerException) : base(ThirdPartyReplication.NoCopyOnServer(dbId, dbName, serverName), innerException)
		{
			this.dbId = dbId;
			this.dbName = dbName;
			this.serverName = serverName;
		}

		protected NoCopyOnServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbId = (Guid)info.GetValue("dbId", typeof(Guid));
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbId", this.dbId);
			info.AddValue("dbName", this.dbName);
			info.AddValue("serverName", this.serverName);
		}

		public Guid DbId
		{
			get
			{
				return this.dbId;
			}
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly Guid dbId;

		private readonly string dbName;

		private readonly string serverName;
	}
}
