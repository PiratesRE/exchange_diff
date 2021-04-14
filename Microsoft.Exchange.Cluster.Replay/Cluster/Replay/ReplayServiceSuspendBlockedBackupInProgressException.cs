using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceSuspendBlockedBackupInProgressException : TaskServerException
	{
		public ReplayServiceSuspendBlockedBackupInProgressException() : base(ReplayStrings.ReplayServiceSuspendBlockedBackupInProgressException)
		{
		}

		public ReplayServiceSuspendBlockedBackupInProgressException(Exception innerException) : base(ReplayStrings.ReplayServiceSuspendBlockedBackupInProgressException, innerException)
		{
		}

		protected ReplayServiceSuspendBlockedBackupInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
