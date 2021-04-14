using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit.Schema.Mailbox;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupOperationAuditEventRecordAdapter : AuditEventRecordAdapter
	{
		public GroupOperationAuditEventRecordAdapter(ExchangeMailboxAuditGroupRecord record, string displayOrganizationId) : base(record, displayOrganizationId)
		{
			this.record = record;
		}

		protected override IEnumerable<KeyValuePair<string, string>> InternalGetEventDetails()
		{
			foreach (KeyValuePair<string, string> detail in base.InternalGetEventDetails())
			{
				yield return detail;
			}
			if (this.record.Folder != null)
			{
				yield return base.MakePair("FolderId", this.record.Folder.Id);
				yield return base.MakePair("FolderPathName", this.record.Folder.PathName);
			}
			bool crossMailbox = this.record.CrossMailboxOperation ?? false;
			yield return base.MakePair("CrossMailboxOperation", crossMailbox);
			if (crossMailbox && this.record.DestMailboxGuid != null)
			{
				yield return base.MakePair("DestMailboxGuid", this.record.DestMailboxGuid);
				yield return base.MakePair("DestMailboxOwnerUPN", this.record.DestMailboxOwnerUPN);
				if (!string.IsNullOrEmpty(this.record.DestMailboxOwnerSid))
				{
					yield return base.MakePair("DestMailboxOwnerSid", this.record.DestMailboxOwnerSid);
					KeyValuePair<string, string> p;
					if (base.TryMakePair("DestMailboxOwnerMasterAccountSid", this.record.DestMailboxOwnerMasterAccountSid, out p))
					{
						yield return p;
					}
				}
			}
			if (this.record.DestFolder != null && !string.IsNullOrEmpty(this.record.DestFolder.Id))
			{
				yield return base.MakePair("DestFolderId", this.record.DestFolder.Id);
				KeyValuePair<string, string> p;
				if (base.TryMakePair("DestFolderPathName", this.record.DestFolder.PathName, out p))
				{
					yield return p;
				}
			}
			if (this.record.Folders != null)
			{
				int folderCount = 0;
				foreach (ExchangeFolder folder in this.record.Folders)
				{
					if (this.record.Folder == null || !this.record.Folder.Id.Equals(folder.Id))
					{
						yield return base.MakePair(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
						{
							folderCount,
							".SourceFolderId"
						}), folder.Id);
						KeyValuePair<string, string> p;
						if (base.TryMakePair(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
						{
							folderCount,
							".SourceFolderPathName"
						}), folder.PathName, out p))
						{
							yield return p;
						}
						folderCount++;
					}
				}
			}
			if (this.record.SourceItems != null)
			{
				int itemCount = 0;
				foreach (ExchangeItem item in this.record.SourceItems)
				{
					yield return base.MakePair(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
					{
						itemCount,
						".SourceItemId"
					}), item.Id);
					KeyValuePair<string, string> p;
					if (base.TryMakePair(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
					{
						itemCount,
						".SourceItemSubject"
					}), item.Subject, out p))
					{
						yield return p;
					}
					if (item.ParentFolder != null && base.TryMakePair(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
					{
						itemCount,
						".SourceItemFolderPathName"
					}), item.ParentFolder.PathName, out p))
					{
						yield return p;
					}
					itemCount++;
				}
			}
			yield break;
		}

		private readonly ExchangeMailboxAuditGroupRecord record;
	}
}
