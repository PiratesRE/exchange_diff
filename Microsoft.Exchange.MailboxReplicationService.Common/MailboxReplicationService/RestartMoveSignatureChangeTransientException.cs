using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RestartMoveSignatureChangeTransientException : MailboxReplicationTransientException
	{
		public RestartMoveSignatureChangeTransientException() : base(MrsStrings.ReportMoveRestartedDueToSignatureChange)
		{
		}

		public RestartMoveSignatureChangeTransientException(Exception innerException) : base(MrsStrings.ReportMoveRestartedDueToSignatureChange, innerException)
		{
		}

		protected RestartMoveSignatureChangeTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
