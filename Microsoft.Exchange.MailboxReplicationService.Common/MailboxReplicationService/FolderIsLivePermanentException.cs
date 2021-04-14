using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderIsLivePermanentException : MailboxReplicationPermanentException
	{
		public FolderIsLivePermanentException(string folderName) : base(MrsStrings.FolderIsLive(folderName))
		{
			this.folderName = folderName;
		}

		public FolderIsLivePermanentException(string folderName, Exception innerException) : base(MrsStrings.FolderIsLive(folderName), innerException)
		{
			this.folderName = folderName;
		}

		protected FolderIsLivePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
