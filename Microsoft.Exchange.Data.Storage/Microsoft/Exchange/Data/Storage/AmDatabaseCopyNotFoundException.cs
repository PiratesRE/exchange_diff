using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDatabaseCopyNotFoundException : AmServerException
	{
		public AmDatabaseCopyNotFoundException(string dbName, string serverName) : base(ServerStrings.AmDatabaseCopyNotFoundException(dbName, serverName))
		{
			this.dbName = dbName;
			this.serverName = serverName;
		}

		public AmDatabaseCopyNotFoundException(string dbName, string serverName, Exception innerException) : base(ServerStrings.AmDatabaseCopyNotFoundException(dbName, serverName), innerException)
		{
			this.dbName = dbName;
			this.serverName = serverName;
		}

		protected AmDatabaseCopyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("serverName", this.serverName);
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

		private readonly string dbName;

		private readonly string serverName;
	}
}
