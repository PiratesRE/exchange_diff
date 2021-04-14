using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseNotFoundByGuidPermanentException : MailboxReplicationPermanentException
	{
		public DatabaseNotFoundByGuidPermanentException(Guid dbGuid) : base(MrsStrings.MailboxDatabaseNotFoundByGuid(dbGuid))
		{
			this.dbGuid = dbGuid;
		}

		public DatabaseNotFoundByGuidPermanentException(Guid dbGuid, Exception innerException) : base(MrsStrings.MailboxDatabaseNotFoundByGuid(dbGuid), innerException)
		{
			this.dbGuid = dbGuid;
		}

		protected DatabaseNotFoundByGuidPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbGuid = (Guid)info.GetValue("dbGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbGuid", this.dbGuid);
		}

		public Guid DbGuid
		{
			get
			{
				return this.dbGuid;
			}
		}

		private readonly Guid dbGuid;
	}
}
