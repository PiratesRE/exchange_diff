using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasFolderDeleteFailedPermanentException : MailboxReplicationPermanentException
	{
		public EasFolderDeleteFailedPermanentException(string errorMessage) : base(MrsStrings.EasFolderDeleteFailed(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public EasFolderDeleteFailedPermanentException(string errorMessage, Exception innerException) : base(MrsStrings.EasFolderDeleteFailed(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected EasFolderDeleteFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
