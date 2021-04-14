using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorMoveActiveCopyIsSeedingSourceException : LocalizedException
	{
		public ErrorMoveActiveCopyIsSeedingSourceException(string db, string server) : base(Strings.ErrorMoveActiveCopyIsSeedingSourceException(db, server))
		{
			this.db = db;
			this.server = server;
		}

		public ErrorMoveActiveCopyIsSeedingSourceException(string db, string server, Exception innerException) : base(Strings.ErrorMoveActiveCopyIsSeedingSourceException(db, server), innerException)
		{
			this.db = db;
			this.server = server;
		}

		protected ErrorMoveActiveCopyIsSeedingSourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.db = (string)info.GetValue("db", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("db", this.db);
			info.AddValue("server", this.server);
		}

		public string Db
		{
			get
			{
				return this.db;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string db;

		private readonly string server;
	}
}
