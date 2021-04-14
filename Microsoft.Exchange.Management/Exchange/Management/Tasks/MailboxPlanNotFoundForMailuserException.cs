using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxPlanNotFoundForMailuserException : MailboxReplicationPermanentException
	{
		public MailboxPlanNotFoundForMailuserException(string user) : base(Strings.ErrorCouldNotLocateMailboxPlanForMailUser(user))
		{
			this.user = user;
		}

		public MailboxPlanNotFoundForMailuserException(string user, Exception innerException) : base(Strings.ErrorCouldNotLocateMailboxPlanForMailUser(user), innerException)
		{
			this.user = user;
		}

		protected MailboxPlanNotFoundForMailuserException(SerializationInfo info, StreamingContext context) : base(info, context)
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
