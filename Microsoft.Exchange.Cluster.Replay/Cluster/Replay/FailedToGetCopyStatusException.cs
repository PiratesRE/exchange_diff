using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToGetCopyStatusException : SeedPrepareException
	{
		public FailedToGetCopyStatusException(string server, string db) : base(ReplayStrings.FailedToGetCopyStatus(server, db))
		{
			this.server = server;
			this.db = db;
		}

		public FailedToGetCopyStatusException(string server, string db, Exception innerException) : base(ReplayStrings.FailedToGetCopyStatus(server, db), innerException)
		{
			this.server = server;
			this.db = db;
		}

		protected FailedToGetCopyStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
			this.db = (string)info.GetValue("db", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
			info.AddValue("db", this.db);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string Db
		{
			get
			{
				return this.db;
			}
		}

		private readonly string server;

		private readonly string db;
	}
}
