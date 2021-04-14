using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UserNotUmEnabledException : LocalizedException
	{
		public UserNotUmEnabledException(string user) : base(Strings.ExceptionUserNotUmEnabled(user))
		{
			this.user = user;
		}

		public UserNotUmEnabledException(string user, Exception innerException) : base(Strings.ExceptionUserNotUmEnabled(user), innerException)
		{
			this.user = user;
		}

		protected UserNotUmEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
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
