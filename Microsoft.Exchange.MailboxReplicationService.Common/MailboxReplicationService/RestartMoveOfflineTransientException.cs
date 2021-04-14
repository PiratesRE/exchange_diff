using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RestartMoveOfflineTransientException : MailboxReplicationTransientException
	{
		public RestartMoveOfflineTransientException(LocalizedString mbxId) : base(MrsStrings.ReportRemovingTargetMailboxDueToOfflineMoveFailure(mbxId))
		{
			this.mbxId = mbxId;
		}

		public RestartMoveOfflineTransientException(LocalizedString mbxId, Exception innerException) : base(MrsStrings.ReportRemovingTargetMailboxDueToOfflineMoveFailure(mbxId), innerException)
		{
			this.mbxId = mbxId;
		}

		protected RestartMoveOfflineTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mbxId = (LocalizedString)info.GetValue("mbxId", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mbxId", this.mbxId);
		}

		public LocalizedString MbxId
		{
			get
			{
				return this.mbxId;
			}
		}

		private readonly LocalizedString mbxId;
	}
}
