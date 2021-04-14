using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbLockConflictException : AmTransientException
	{
		public AmDbLockConflictException(Guid dbGuid, string reqReason, string ownerReason) : base(ReplayStrings.AmDbLockConflict(dbGuid, reqReason, ownerReason))
		{
			this.dbGuid = dbGuid;
			this.reqReason = reqReason;
			this.ownerReason = ownerReason;
		}

		public AmDbLockConflictException(Guid dbGuid, string reqReason, string ownerReason, Exception innerException) : base(ReplayStrings.AmDbLockConflict(dbGuid, reqReason, ownerReason), innerException)
		{
			this.dbGuid = dbGuid;
			this.reqReason = reqReason;
			this.ownerReason = ownerReason;
		}

		protected AmDbLockConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbGuid = (Guid)info.GetValue("dbGuid", typeof(Guid));
			this.reqReason = (string)info.GetValue("reqReason", typeof(string));
			this.ownerReason = (string)info.GetValue("ownerReason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbGuid", this.dbGuid);
			info.AddValue("reqReason", this.reqReason);
			info.AddValue("ownerReason", this.ownerReason);
		}

		public Guid DbGuid
		{
			get
			{
				return this.dbGuid;
			}
		}

		public string ReqReason
		{
			get
			{
				return this.reqReason;
			}
		}

		public string OwnerReason
		{
			get
			{
				return this.ownerReason;
			}
		}

		private readonly Guid dbGuid;

		private readonly string reqReason;

		private readonly string ownerReason;
	}
}
