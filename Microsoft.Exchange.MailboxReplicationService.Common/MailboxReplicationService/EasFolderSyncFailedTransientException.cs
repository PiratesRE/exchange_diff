using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasFolderSyncFailedTransientException : MailboxReplicationTransientException
	{
		public EasFolderSyncFailedTransientException(string errorMessage) : base(MrsStrings.EasFolderSyncFailed(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public EasFolderSyncFailedTransientException(string errorMessage, Exception innerException) : base(MrsStrings.EasFolderSyncFailed(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected EasFolderSyncFailedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
