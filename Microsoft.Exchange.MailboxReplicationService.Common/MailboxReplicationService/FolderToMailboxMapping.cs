using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class FolderToMailboxMapping : XMLSerializableBase
	{
		private FolderToMailboxMapping()
		{
		}

		public FolderToMailboxMapping(string folderName, Guid mailboxGuid)
		{
			this.FolderName = folderName;
			this.MailboxGuid = mailboxGuid;
		}

		[XmlElement(ElementName = "FolderName")]
		public string FolderName { get; set; }

		[XmlElement(ElementName = "MailboxGuid")]
		public Guid MailboxGuid { get; set; }
	}
}
