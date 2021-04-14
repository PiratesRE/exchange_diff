using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ImmediateDismountMailboxDatabaseException : ThirdPartyReplicationException
	{
		public ImmediateDismountMailboxDatabaseException(Guid dbId, string reason) : base(ThirdPartyReplication.ImmediateDismountMailboxDatabaseFailed(dbId, reason))
		{
			this.dbId = dbId;
			this.reason = reason;
		}

		public ImmediateDismountMailboxDatabaseException(Guid dbId, string reason, Exception innerException) : base(ThirdPartyReplication.ImmediateDismountMailboxDatabaseFailed(dbId, reason), innerException)
		{
			this.dbId = dbId;
			this.reason = reason;
		}

		protected ImmediateDismountMailboxDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbId = (Guid)info.GetValue("dbId", typeof(Guid));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbId", this.dbId);
			info.AddValue("reason", this.reason);
		}

		public Guid DbId
		{
			get
			{
				return this.dbId;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly Guid dbId;

		private readonly string reason;
	}
}
