using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseCopySuspendException : LocalizedException
	{
		public DatabaseCopySuspendException(string dbName, string server, string msg) : base(ReplayStrings.DatabaseCopySuspendException(dbName, server, msg))
		{
			this.dbName = dbName;
			this.server = server;
			this.msg = msg;
		}

		public DatabaseCopySuspendException(string dbName, string server, string msg, Exception innerException) : base(ReplayStrings.DatabaseCopySuspendException(dbName, server, msg), innerException)
		{
			this.dbName = dbName;
			this.server = server;
			this.msg = msg;
		}

		protected DatabaseCopySuspendException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("server", this.server);
			info.AddValue("msg", this.msg);
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

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string dbName;

		private readonly string server;

		private readonly string msg;
	}
}
