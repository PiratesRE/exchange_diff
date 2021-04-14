using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderIsMissingPermanentException : MailboxReplicationPermanentException
	{
		public FolderIsMissingPermanentException(string folderPath) : base(MrsStrings.FolderIsMissing(folderPath))
		{
			this.folderPath = folderPath;
		}

		public FolderIsMissingPermanentException(string folderPath, Exception innerException) : base(MrsStrings.FolderIsMissing(folderPath), innerException)
		{
			this.folderPath = folderPath;
		}

		protected FolderIsMissingPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderPath = (string)info.GetValue("folderPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderPath", this.folderPath);
		}

		public string FolderPath
		{
			get
			{
				return this.folderPath;
			}
		}

		private readonly string folderPath;
	}
}
