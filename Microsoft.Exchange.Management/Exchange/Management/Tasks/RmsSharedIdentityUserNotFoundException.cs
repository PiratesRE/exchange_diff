using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsSharedIdentityUserNotFoundException : LocalizedException
	{
		public RmsSharedIdentityUserNotFoundException(string userCn) : base(Strings.RmsSharedIdentityUserNotFound(userCn))
		{
			this.userCn = userCn;
		}

		public RmsSharedIdentityUserNotFoundException(string userCn, Exception innerException) : base(Strings.RmsSharedIdentityUserNotFound(userCn), innerException)
		{
			this.userCn = userCn;
		}

		protected RmsSharedIdentityUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userCn = (string)info.GetValue("userCn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userCn", this.userCn);
		}

		public string UserCn
		{
			get
			{
				return this.userCn;
			}
		}

		private readonly string userCn;
	}
}
