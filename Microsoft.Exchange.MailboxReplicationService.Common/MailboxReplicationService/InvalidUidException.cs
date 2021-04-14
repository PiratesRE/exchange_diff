using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidUidException : MailboxReplicationPermanentException
	{
		public InvalidUidException(string uid) : base(MrsStrings.InvalidUid(uid))
		{
			this.uid = uid;
		}

		public InvalidUidException(string uid, Exception innerException) : base(MrsStrings.InvalidUid(uid), innerException)
		{
			this.uid = uid;
		}

		protected InvalidUidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.uid = (string)info.GetValue("uid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("uid", this.uid);
		}

		public string Uid
		{
			get
			{
				return this.uid;
			}
		}

		private readonly string uid;
	}
}
