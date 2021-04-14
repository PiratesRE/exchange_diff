using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DbCopyAlreadyHostedOnServerException : LocalizedException
	{
		public DbCopyAlreadyHostedOnServerException(string database, string hostServer) : base(Strings.DbCopyAlreadyHostedOnServerException(database, hostServer))
		{
			this.database = database;
			this.hostServer = hostServer;
		}

		public DbCopyAlreadyHostedOnServerException(string database, string hostServer, Exception innerException) : base(Strings.DbCopyAlreadyHostedOnServerException(database, hostServer), innerException)
		{
			this.database = database;
			this.hostServer = hostServer;
		}

		protected DbCopyAlreadyHostedOnServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.hostServer = (string)info.GetValue("hostServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("hostServer", this.hostServer);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string HostServer
		{
			get
			{
				return this.hostServer;
			}
		}

		private readonly string database;

		private readonly string hostServer;
	}
}
