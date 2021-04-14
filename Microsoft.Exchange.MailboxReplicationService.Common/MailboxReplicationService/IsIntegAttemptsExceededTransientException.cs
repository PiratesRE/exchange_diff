using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IsIntegAttemptsExceededTransientException : MailboxReplicationTransientException
	{
		public IsIntegAttemptsExceededTransientException(short attempts) : base(MrsStrings.IsIntegAttemptsExceededError(attempts))
		{
			this.attempts = attempts;
		}

		public IsIntegAttemptsExceededTransientException(short attempts, Exception innerException) : base(MrsStrings.IsIntegAttemptsExceededError(attempts), innerException)
		{
			this.attempts = attempts;
		}

		protected IsIntegAttemptsExceededTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.attempts = (short)info.GetValue("attempts", typeof(short));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("attempts", this.attempts);
		}

		public short Attempts
		{
			get
			{
				return this.attempts;
			}
		}

		private readonly short attempts;
	}
}
