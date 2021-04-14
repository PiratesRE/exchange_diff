using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxAuditLogSearchItem : AuditLogSearchItemBase
	{
		public MailboxAuditLogSearchItem(MailboxSession session, Folder folder) : base(session, folder)
		{
		}

		public MailboxAuditLogSearchItem(MailboxSession session, VersionedId messageId) : base(session, messageId)
		{
		}

		protected override string ItemClass
		{
			get
			{
				return "IPM.AuditLogSearch.Mailbox";
			}
		}

		protected override PropertyDefinition[] PropertiesToLoad
		{
			get
			{
				return MailboxAuditLogSearchItem.propertiesToLoad;
			}
		}

		public MultiValuedProperty<ADObjectId> MailboxIds
		{
			get
			{
				MultiValuedProperty<byte[]> propertiesPossiblyNotFound = base.GetPropertiesPossiblyNotFound<byte[]>(MailboxAuditLogSearchItemSchema.MailboxIds);
				MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
				foreach (byte[] bytes in propertiesPossiblyNotFound)
				{
					multiValuedProperty.Add(new ADObjectId(bytes));
				}
				return multiValuedProperty;
			}
			set
			{
				base.Message[MailboxAuditLogSearchItemSchema.MailboxIds] = (from adObjectId in value
				select adObjectId.GetBytes()).ToArray<byte[]>();
			}
		}

		public MultiValuedProperty<string> LogonTypeStrings
		{
			get
			{
				return base.GetPropertiesPossiblyNotFound<string>(MailboxAuditLogSearchItemSchema.LogonTypeStrings);
			}
			set
			{
				base.Message[MailboxAuditLogSearchItemSchema.LogonTypeStrings] = value.ToArray();
			}
		}

		public MultiValuedProperty<string> Operations
		{
			get
			{
				return base.GetPropertiesPossiblyNotFound<string>(MailboxAuditLogSearchItemSchema.Operations);
			}
			set
			{
				base.Message[MailboxAuditLogSearchItemSchema.Operations] = value.ToArray();
			}
		}

		public bool ShowDetails
		{
			get
			{
				return (bool)base.Message[MailboxAuditLogSearchItemSchema.ShowDetails];
			}
			set
			{
				base.Message[MailboxAuditLogSearchItemSchema.ShowDetails] = value;
			}
		}

		private static PropertyDefinition[] propertiesToLoad = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.CreationTime,
			AuditLogSearchItemSchema.Identity,
			AuditLogSearchItemSchema.Name,
			AuditLogSearchItemSchema.StartDate,
			AuditLogSearchItemSchema.EndDate,
			AuditLogSearchItemSchema.StatusMailRecipients,
			AuditLogSearchItemSchema.CreatedBy,
			AuditLogSearchItemSchema.CreatedByEx,
			AuditLogSearchItemSchema.ExternalAccess,
			MailboxAuditLogSearchItemSchema.MailboxIds,
			MailboxAuditLogSearchItemSchema.LogonTypeStrings,
			MailboxAuditLogSearchItemSchema.Operations,
			MailboxAuditLogSearchItemSchema.ShowDetails
		};
	}
}
