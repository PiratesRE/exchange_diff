using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ItemOperationAuditEvent : AuditEvent
	{
		public ItemOperationAuditEvent(MailboxSession session, MailboxAuditOperations operation, COWSettings settings, OperationResult result, LogonType logonType, bool externalAccess, StoreObjectId itemId, CoreItem item, ItemAuditInfo itemAuditInfo) : base(session, operation, settings, result, logonType, externalAccess)
		{
			if (MailboxAuditOperations.FolderBind != operation && MailboxAuditOperations.SendAs != operation && MailboxAuditOperations.SendOnBehalf != operation && MailboxAuditOperations.Create != operation)
			{
				Util.ThrowOnNullArgument(itemId, "itemId");
			}
			this.itemId = itemId;
			this.item = item;
			this.itemAuditInfo = itemAuditInfo;
		}

		private static string GetItemSubject(MailboxSession session, CoreItem item)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(item, "item");
			return item.PropertyBag.TryGetProperty(ItemSchema.Subject) as string;
		}

		protected override IEnumerable<KeyValuePair<string, string>> InternalGetEventDetails()
		{
			foreach (KeyValuePair<string, string> detail in base.InternalGetEventDetails())
			{
				yield return detail;
			}
			if (base.COWSettings.CurrentFolderId != null)
			{
				yield return new KeyValuePair<string, string>("FolderId", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					base.COWSettings.CurrentFolderId
				}));
				if (this.itemAuditInfo != null && this.itemAuditInfo.ParentFolderPathName != null)
				{
					yield return new KeyValuePair<string, string>("FolderPathName", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						this.itemAuditInfo.ParentFolderPathName
					}));
				}
				else
				{
					string folderPathName = base.GetCurrentFolderPathName();
					if (folderPathName != null)
					{
						yield return new KeyValuePair<string, string>("FolderPathName", folderPathName);
					}
				}
			}
			if (MailboxAuditOperations.FolderBind != base.AuditOperation)
			{
				if (MailboxAuditOperations.Update == base.AuditOperation || MailboxAuditOperations.Create == base.AuditOperation)
				{
					if (this.itemAuditInfo == null)
					{
						throw new InvalidOperationException("ItemOperationAuditEvent::itemAuditInfo should not be null for Update/Create operations.");
					}
					if (this.itemAuditInfo.Id != null)
					{
						yield return new KeyValuePair<string, string>("ItemId", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
						{
							this.itemAuditInfo.Id
						}));
					}
					yield return new KeyValuePair<string, string>("ItemSubject", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						this.itemAuditInfo.Subject
					}));
					if (MailboxAuditOperations.Update == base.AuditOperation)
					{
						List<string> dirtyProperties = this.itemAuditInfo.DirtyProperties;
						yield return new KeyValuePair<string, string>("DirtyProperties", string.Join(", ", dirtyProperties.ToArray()));
					}
				}
				else if (MailboxAuditOperations.SendAs == base.AuditOperation || MailboxAuditOperations.SendOnBehalf == base.AuditOperation)
				{
					if (this.item.Id != null && this.item.Id.ObjectId != null)
					{
						yield return new KeyValuePair<string, string>("ItemId", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
						{
							this.item.Id.ObjectId
						}));
					}
					string itemSubject = ItemOperationAuditEvent.GetItemSubject(base.MailboxSession, this.item);
					if (itemSubject != null)
					{
						yield return new KeyValuePair<string, string>("ItemSubject", this.item.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string);
					}
				}
				else if (this.item == null)
				{
					yield return new KeyValuePair<string, string>("ItemId", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						this.itemId
					}));
				}
				else
				{
					if (this.item.Id != null && this.item.Id.ObjectId != null)
					{
						yield return new KeyValuePair<string, string>("ItemId", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
						{
							this.item.Id.ObjectId
						}));
					}
					string itemSubject2 = ItemOperationAuditEvent.GetItemSubject(base.MailboxSession, this.item);
					if (itemSubject2 != null)
					{
						yield return new KeyValuePair<string, string>("ItemSubject", this.item.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string);
					}
				}
			}
			yield break;
		}

		private StoreObjectId itemId;

		private CoreItem item;

		private ItemAuditInfo itemAuditInfo;
	}
}
