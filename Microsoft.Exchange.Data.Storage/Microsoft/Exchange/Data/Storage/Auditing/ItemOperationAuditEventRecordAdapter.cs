using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit.Schema.Mailbox;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ItemOperationAuditEventRecordAdapter : AuditEventRecordAdapter
	{
		public ItemOperationAuditEventRecordAdapter(ExchangeMailboxAuditRecord record, string displayOrganizationId) : base(record, displayOrganizationId)
		{
			this.record = record;
		}

		protected override IEnumerable<KeyValuePair<string, string>> InternalGetEventDetails()
		{
			foreach (KeyValuePair<string, string> detail in base.InternalGetEventDetails())
			{
				yield return detail;
			}
			if (this.record.Item != null && this.record.Item.ParentFolder != null)
			{
				yield return base.MakePair("FolderId", this.record.Item.ParentFolder.Id);
				yield return base.MakePair("FolderPathName", this.record.Item.ParentFolder.PathName);
			}
			if (MailboxAuditOperations.FolderBind != base.AuditOperation)
			{
				if (this.record.Item != null)
				{
					KeyValuePair<string, string> p;
					if (base.TryMakePair("ItemId", this.record.Item.Id, out p))
					{
						yield return p;
					}
					if (base.TryMakePair("ItemSubject", this.record.Item.Subject, out p))
					{
						yield return p;
					}
				}
				if (MailboxAuditOperations.Update == base.AuditOperation)
				{
					List<string> dirtyProperties = this.record.ModifiedProperties ?? new List<string>(0);
					yield return base.MakePair("DirtyProperties", string.Join(", ", dirtyProperties.ToArray()));
				}
			}
			yield break;
		}

		private readonly ExchangeMailboxAuditRecord record;
	}
}
