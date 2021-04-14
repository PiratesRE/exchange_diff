using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxCreationException : LocalizedException
	{
		public MailboxCreationException(string user, string errorMessage) : base(Strings.ErrorMailboxCreationFailure(user, errorMessage))
		{
			this.user = user;
			this.errorMessage = errorMessage;
		}

		public MailboxCreationException(string user, string errorMessage, Exception innerException) : base(Strings.ErrorMailboxCreationFailure(user, errorMessage), innerException)
		{
			this.user = user;
			this.errorMessage = errorMessage;
		}

		protected MailboxCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string user;

		private readonly string errorMessage;
	}
}
