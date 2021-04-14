using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbAcllErrorNoReplicaInstance : AmServerException
	{
		public AmDbAcllErrorNoReplicaInstance(string database, string server) : base(ServerStrings.AmDbAcllErrorNoReplicaInstance(database, server))
		{
			this.database = database;
			this.server = server;
		}

		public AmDbAcllErrorNoReplicaInstance(string database, string server, Exception innerException) : base(ServerStrings.AmDbAcllErrorNoReplicaInstance(database, server), innerException)
		{
			this.database = database;
			this.server = server;
		}

		protected AmDbAcllErrorNoReplicaInstance(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("server", this.server);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string database;

		private readonly string server;
	}
}
