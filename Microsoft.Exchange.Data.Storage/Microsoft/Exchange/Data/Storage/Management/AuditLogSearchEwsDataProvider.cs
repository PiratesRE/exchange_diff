using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditLogSearchEwsDataProvider : EwsStoreDataProvider
	{
		public AuditLogSearchEwsDataProvider(IExchangePrincipal primaryMailbox) : base(new LazilyInitialized<IExchangePrincipal>(() => primaryMailbox))
		{
			base.LogonType = new SpecialLogonType?(SpecialLogonType.Admin);
			base.BudgetType = OpenAsAdminOrSystemServiceBudgetTypeType.RunAsBackgroundLoad;
		}

		public IEnumerable<T> FindIds<T>(SearchFilter filter, bool identityPresent, int resultSize, ObjectId rootId) where T : IConfigurable, new()
		{
			Folder mailboxAuditFolder = base.GetOrCreateFolder("MailboxAuditLogSearch");
			Folder adminAuditFolder = base.GetOrCreateFolder("AdminAuditLogSearch");
			int itemsFound = 0;
			if (identityPresent)
			{
				foreach (AdminAuditLogSearch item in this.FindInFolder<AdminAuditLogSearch>(filter, adminAuditFolder.Id))
				{
					if (itemsFound >= resultSize)
					{
						yield break;
					}
					item.Type = "Admin";
					itemsFound++;
					yield return (T)((object)item);
				}
				foreach (MailboxAuditLogSearch item2 in this.FindInFolder<MailboxAuditLogSearch>(filter, mailboxAuditFolder.Id))
				{
					if (itemsFound >= resultSize)
					{
						yield break;
					}
					item2.Type = "Mailbox";
					itemsFound++;
					yield return (T)((object)item2);
				}
			}
			else
			{
				foreach (AuditLogSearchBase item3 in this.FindInFolder<AuditLogSearchBase>(filter, adminAuditFolder.Id))
				{
					if (itemsFound >= resultSize)
					{
						yield break;
					}
					item3.Type = "Admin";
					itemsFound++;
					yield return (T)((object)item3);
				}
				foreach (AuditLogSearchBase item4 in this.FindInFolder<AuditLogSearchBase>(filter, mailboxAuditFolder.Id))
				{
					if (itemsFound >= resultSize)
					{
						yield break;
					}
					item4.Type = "Mailbox";
					itemsFound++;
					yield return (T)((object)item4);
				}
			}
			yield break;
		}

		public void DeleteAuditLogSearch(AuditLogSearchBase search)
		{
			EwsStoreObject ewsStoreObject = new EwsStoreObject();
			ewsStoreObject.propertyBag.SetField(EwsStoreObjectSchema.Identity, search[AuditLogSearchBaseEwsSchema.EwsItemId]);
			base.Delete(ewsStoreObject);
			search.ResetChangeTracking();
			search.MarkAsDeleted();
		}
	}
}
