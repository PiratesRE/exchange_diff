using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogCopierFailedToGetSuspendLockException : TransientException
	{
		public LogCopierFailedToGetSuspendLockException() : base(ReplayStrings.LogCopierFailedToGetSuspendLock)
		{
		}

		public LogCopierFailedToGetSuspendLockException(Exception innerException) : base(ReplayStrings.LogCopierFailedToGetSuspendLock, innerException)
		{
		}

		protected LogCopierFailedToGetSuspendLockException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
