using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoSuchDatabaseException : ThirdPartyReplicationException
	{
		public NoSuchDatabaseException(Guid dbId) : base(ThirdPartyReplication.NoSuchDatabase(dbId))
		{
			this.dbId = dbId;
		}

		public NoSuchDatabaseException(Guid dbId, Exception innerException) : base(ThirdPartyReplication.NoSuchDatabase(dbId), innerException)
		{
			this.dbId = dbId;
		}

		protected NoSuchDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbId = (Guid)info.GetValue("dbId", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbId", this.dbId);
		}

		public Guid DbId
		{
			get
			{
				return this.dbId;
			}
		}

		private readonly Guid dbId;
	}
}
