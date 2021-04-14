using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderCopyFailedPermanentException : MailboxReplicationPermanentException
	{
		public FolderCopyFailedPermanentException(string folderName) : base(MrsStrings.FolderCopyFailed(folderName))
		{
			this.folderName = folderName;
		}

		public FolderCopyFailedPermanentException(string folderName, Exception innerException) : base(MrsStrings.FolderCopyFailed(folderName), innerException)
		{
			this.folderName = folderName;
		}

		protected FolderCopyFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderName = (string)info.GetValue("folderName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderName", this.folderName);
		}

		public string FolderName
		{
			get
			{
				return this.folderName;
			}
		}

		private readonly string folderName;
	}
}
