using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasFetchFailedTransientException : MailboxReplicationTransientException
	{
		public EasFetchFailedTransientException(string errorMessage) : base(MrsStrings.EasFetchFailed(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public EasFetchFailedTransientException(string errorMessage, Exception innerException) : base(MrsStrings.EasFetchFailed(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected EasFetchFailedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string errorMessage;
	}
}
