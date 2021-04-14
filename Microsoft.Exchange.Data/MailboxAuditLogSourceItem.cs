using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class MailboxAuditLogSourceItem
	{
		private MailboxAuditLogSourceItem()
		{
		}

		public string SourceItemId { get; private set; }

		public string SourceItemSubject { get; private set; }

		public string SourceItemFolderPathName { get; private set; }

		public static MailboxAuditLogSourceItem Parse(string itemId, string itemSubject, string itemFolderPathName)
		{
			return new MailboxAuditLogSourceItem
			{
				SourceItemId = itemId,
				SourceItemSubject = itemSubject,
				SourceItemFolderPathName = itemFolderPathName
			};
		}

		public override int GetHashCode()
		{
			if (this.SourceItemId != null)
			{
				return this.SourceItemId.ToUpperInvariant().GetHashCode();
			}
			return string.Empty.GetHashCode();
		}

		public override string ToString()
		{
			return this.SourceItemId;
		}
	}
}
