using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RestartMoveCorruptSyncStateTransientException : MailboxReplicationTransientException
	{
		public RestartMoveCorruptSyncStateTransientException(string mbxId) : base(MrsStrings.ReportRestartingMoveBecauseSyncStateDoesNotExist(mbxId))
		{
			this.mbxId = mbxId;
		}

		public RestartMoveCorruptSyncStateTransientException(string mbxId, Exception innerException) : base(MrsStrings.ReportRestartingMoveBecauseSyncStateDoesNotExist(mbxId), innerException)
		{
			this.mbxId = mbxId;
		}

		protected RestartMoveCorruptSyncStateTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
