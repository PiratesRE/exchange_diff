using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotCreateMessageIdException : MailboxReplicationPermanentException
	{
		public CannotCreateMessageIdException(long uid, string folderName) : base(MrsStrings.CannotCreateMessageId(uid, folderName))
		{
			this.uid = uid;
			this.folderName = folderName;
		}

		public CannotCreateMessageIdException(long uid, string folderName, Exception innerException) : base(MrsStrings.CannotCreateMessageId(uid, folderName), innerException)
		{
			this.uid = uid;
			this.folderName = folderName;
		}

		protected CannotCreateMessageIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.uid = (long)info.GetValue("uid", typeof(long));
			this.folderName = (string)info.GetValue("folderName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("uid", this.uid);
			info.AddValue("folderName", this.folderName);
		}

		public long Uid
		{
			get
			{
				return this.uid;
			}
		}

		public string FolderName
		{
			get
			{
				return this.folderName;
			}
		}

		private readonly long uid;

		private readonly string folderName;
	}
}
