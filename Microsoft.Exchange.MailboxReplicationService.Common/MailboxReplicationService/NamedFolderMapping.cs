using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class NamedFolderMapping : WellKnownFolderMapping
	{
		public NamedFolderMapping(WellKnownFolderType wkft, WellKnownFolderType parentType, string folderName)
		{
			base.WKFType = wkft;
			this.ParentType = parentType;
			this.FolderName = folderName;
		}

		public WellKnownFolderType ParentType { get; protected set; }

		public string FolderName { get; protected set; }
	}
}
