using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoMailboxForUserException : LocalizedException
	{
		public NoMailboxForUserException(string user) : base(Strings.ExceptionNoMailboxForUser(user))
		{
			this.user = user;
		}

		public NoMailboxForUserException(string user, Exception innerException) : base(Strings.ExceptionNoMailboxForUser(user), innerException)
		{
			this.user = user;
		}

		protected NoMailboxForUserException(SerializationInfo info, StreamingContext context) : base(info, context)
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
