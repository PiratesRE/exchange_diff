using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToReadADUserException : MailboxReplicationPermanentException
	{
		public UnableToReadADUserException(string userId) : base(MrsStrings.UnableToReadADUser(userId))
		{
			this.userId = userId;
		}

		public UnableToReadADUserException(string userId, Exception innerException) : base(MrsStrings.UnableToReadADUser(userId), innerException)
		{
			this.userId = userId;
		}

		protected UnableToReadADUserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userId = (string)info.GetValue("userId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userId", this.userId);
		}

		public string UserId
		{
			get
			{
				return this.userId;
			}
		}

		private readonly string userId;
	}
}
