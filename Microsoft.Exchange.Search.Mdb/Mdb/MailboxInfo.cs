using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class MailboxInfo
	{
		public Guid MailboxGuid { get; set; }

		public bool IsArchive { get; set; }

		public string DisplayName { get; set; }

		public int MailboxNumber { get; set; }

		public bool IsPublicFolderMailbox { get; set; }

		public OrganizationId OrganizationId { get; set; }

		public bool IsSharedMailbox { get; set; }

		public bool IsTeamSiteMailbox { get; set; }

		public bool IsModernGroupMailbox { get; set; }

		public override string ToString()
		{
			return string.Format("{0} ({1}, MailboxNumber={2})", this.DisplayName, this.MailboxGuid, this.MailboxNumber);
		}
	}
}
