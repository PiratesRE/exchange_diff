using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RestartMoveSourceCorruptionTransientException : MailboxReplicationTransientException
	{
		public RestartMoveSourceCorruptionTransientException() : base(MrsStrings.ReportMoveRestartedDueToSourceCorruption)
		{
		}

		public RestartMoveSourceCorruptionTransientException(Exception innerException) : base(MrsStrings.ReportMoveRestartedDueToSourceCorruption, innerException)
		{
		}

		protected RestartMoveSourceCorruptionTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
