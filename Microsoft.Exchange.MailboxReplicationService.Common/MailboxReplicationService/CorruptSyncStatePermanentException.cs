using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptSyncStatePermanentException : MailboxReplicationPermanentException
	{
		public CorruptSyncStatePermanentException(string mbxId) : base(MrsStrings.ReportFailingMoveBecauseSyncStateIssue(mbxId))
		{
			this.mbxId = mbxId;
		}

		public CorruptSyncStatePermanentException(string mbxId, Exception innerException) : base(MrsStrings.ReportFailingMoveBecauseSyncStateIssue(mbxId), innerException)
		{
			this.mbxId = mbxId;
		}

		protected CorruptSyncStatePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mbxId = (string)info.GetValue("mbxId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mbxId", this.mbxId);
		}

		public string MbxId
		{
			get
			{
				return this.mbxId;
			}
		}

		private readonly string mbxId;
	}
}
