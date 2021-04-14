using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseVersionUnsupportedPermanentException : MailboxReplicationPermanentException
	{
		public DatabaseVersionUnsupportedPermanentException(string dbName, string serverName, string serverVersion) : base(Strings.ErrorDatabaseVersionUnsupported(dbName, serverName, serverVersion))
		{
			this.dbName = dbName;
			this.serverName = serverName;
			this.serverVersion = serverVersion;
		}

		public DatabaseVersionUnsupportedPermanentException(string dbName, string serverName, string serverVersion, Exception innerException) : base(Strings.ErrorDatabaseVersionUnsupported(dbName, serverName, serverVersion), innerException)
		{
			this.dbName = dbName;
			this.serverName = serverName;
			this.serverVersion = serverVersion;
		}

		protected DatabaseVersionUnsupportedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.serverVersion = (string)info.GetValue("serverVersion", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("serverName", this.serverName);
			info.AddValue("serverVersion", this.serverVersion);
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

		public string ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		private readonly string dbName;

		private readonly string serverName;

		private readonly string serverVersion;
	}
}
