using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseExcludedFromProvisioningException : MailboxReplicationPermanentException
	{
		public DatabaseExcludedFromProvisioningException(string db) : base(Strings.ErrorDatabaseExcludedFromProvisioning(db))
		{
			this.db = db;
		}

		public DatabaseExcludedFromProvisioningException(string db, Exception innerException) : base(Strings.ErrorDatabaseExcludedFromProvisioning(db), innerException)
		{
			this.db = db;
		}

		protected DatabaseExcludedFromProvisioningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.db = (string)info.GetValue("db", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("db", this.db);
		}

		public string Db
		{
			get
			{
				return this.db;
			}
		}

		private readonly string db;
	}
}
