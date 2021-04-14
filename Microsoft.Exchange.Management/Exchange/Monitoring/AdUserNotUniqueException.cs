using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdUserNotUniqueException : LocalizedException
	{
		public AdUserNotUniqueException(string user) : base(Strings.ErrorAdUserNotUnique(user))
		{
			this.user = user;
		}

		public AdUserNotUniqueException(string user, Exception innerException) : base(Strings.ErrorAdUserNotUnique(user), innerException)
		{
			this.user = user;
		}

		protected AdUserNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
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
