using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SetArchivePermissionException : LocalizedException
	{
		public SetArchivePermissionException(string user, Exception exception) : base(Strings.SetArchivePermissionException(user, exception))
		{
			this.user = user;
			this.exception = exception;
		}

		public SetArchivePermissionException(string user, Exception exception, Exception innerException) : base(Strings.SetArchivePermissionException(user, exception), innerException)
		{
			this.user = user;
			this.exception = exception;
		}

		protected SetArchivePermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
			this.exception = (Exception)info.GetValue("exception", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
			info.AddValue("exception", this.exception);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		private readonly string user;

		private readonly Exception exception;
	}
}
