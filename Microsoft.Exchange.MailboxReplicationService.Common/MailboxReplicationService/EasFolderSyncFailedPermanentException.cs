using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasFolderSyncFailedPermanentException : MailboxReplicationPermanentException
	{
		public EasFolderSyncFailedPermanentException(string errorMessage) : base(MrsStrings.EasFolderSyncFailed(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public EasFolderSyncFailedPermanentException(string errorMessage, Exception innerException) : base(MrsStrings.EasFolderSyncFailed(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected EasFolderSyncFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
