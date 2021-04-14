using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseConnectionTransientException : MailboxReplicationTransientException
	{
		public DatabaseConnectionTransientException(string mdb) : base(Strings.ErrorCannotConnectToMailboxDatabase(mdb))
		{
			this.mdb = mdb;
		}

		public DatabaseConnectionTransientException(string mdb, Exception innerException) : base(Strings.ErrorCannotConnectToMailboxDatabase(mdb), innerException)
		{
			this.mdb = mdb;
		}

		protected DatabaseConnectionTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mdb = (string)info.GetValue("mdb", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mdb", this.mdb);
		}

		public string Mdb
		{
			get
			{
				return this.mdb;
			}
		}

		private readonly string mdb;
	}
}
