using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WindowsLiveIDAddressIsMissingPermanentException : MailboxReplicationPermanentException
	{
		public WindowsLiveIDAddressIsMissingPermanentException(string user) : base(MrsStrings.WindowsLiveIDAddressIsMissing(user))
		{
			this.user = user;
		}

		public WindowsLiveIDAddressIsMissingPermanentException(string user, Exception innerException) : base(MrsStrings.WindowsLiveIDAddressIsMissing(user), innerException)
		{
			this.user = user;
		}

		protected WindowsLiveIDAddressIsMissingPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
