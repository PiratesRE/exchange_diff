using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class MailboxAuditLogSourceFolder
	{
		private MailboxAuditLogSourceFolder()
		{
		}

		public string SourceFolderId { get; private set; }

		public string SourceFolderPathName { get; private set; }

		public static MailboxAuditLogSourceFolder Parse(string folderId, string folderPathName)
		{
			return new MailboxAuditLogSourceFolder
			{
				SourceFolderId = folderId,
				SourceFolderPathName = folderPathName
			};
		}

		public override int GetHashCode()
		{
			if (this.SourceFolderId != null)
			{
				return this.SourceFolderId.ToUpperInvariant().GetHashCode();
			}
			return string.Empty.GetHashCode();
		}

		public override string ToString()
		{
			return this.SourceFolderId;
		}
	}
}
