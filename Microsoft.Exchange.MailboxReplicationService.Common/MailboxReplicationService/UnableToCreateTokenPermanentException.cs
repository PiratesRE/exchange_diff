using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToCreateTokenPermanentException : MailboxReplicationPermanentException
	{
		public UnableToCreateTokenPermanentException(string user) : base(MrsStrings.UnableToCreateToken(user))
		{
			this.user = user;
		}

		public UnableToCreateTokenPermanentException(string user, Exception innerException) : base(MrsStrings.UnableToCreateToken(user), innerException)
		{
			this.user = user;
		}

		protected UnableToCreateTokenPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		private readonly string user;
	}
}
