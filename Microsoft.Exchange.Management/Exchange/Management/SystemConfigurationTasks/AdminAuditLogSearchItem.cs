using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AdminAuditLogSearchItem : AuditLogSearchItemBase
	{
		public AdminAuditLogSearchItem(MailboxSession session, Folder folder) : base(session, folder)
		{
		}

		public AdminAuditLogSearchItem(MailboxSession session, VersionedId messageId) : base(session, messageId)
		{
		}

		protected override string ItemClass
		{
			get
			{
				return "IPM.AuditLogSearch.Admin";
			}
		}

		protected override PropertyDefinition[] PropertiesToLoad
		{
			get
			{
				return AdminAuditLogSearchItem.propertiesToLoad;
			}
		}

		public MultiValuedProperty<string> Cmdlets
		{
			get
			{
				return base.GetPropertiesPossiblyNotFound<string>(AdminAuditLogSearchItemSchema.Cmdlets);
			}
			set
			{
				base.Message[AdminAuditLogSearchItemSchema.Cmdlets] = value.ToArray();
			}
		}

		public MultiValuedProperty<string> Parameters
		{
			get
			{
				return base.GetPropertiesPossiblyNotFound<string>(AdminAuditLogSearchItemSchema.Parameters);
			}
			set
			{
				base.Message[AdminAuditLogSearchItemSchema.Parameters] = value.ToArray();
			}
		}

		public MultiValuedProperty<string> ObjectIds
		{
			get
			{
				return base.GetPropertiesPossiblyNotFound<string>(AdminAuditLogSearchItemSchema.ObjectIds);
			}
			set
			{
				base.Message[AdminAuditLogSearchItemSchema.ObjectIds] = value.ToArray();
			}
		}

		public MultiValuedProperty<string> RawUserIds
		{
			get
			{
				return base.GetPropertiesPossiblyNotFound<string>(AdminAuditLogSearchItemSchema.RawUserIds);
			}
			set
			{
				base.Message[AdminAuditLogSearchItemSchema.RawUserIds] = value.ToArray();
			}
		}

		public MultiValuedProperty<string> ResolvedUsers
		{
			get
			{
				return base.GetPropertiesPossiblyNotFound<string>(AdminAuditLogSearchItemSchema.ResolvedUsers);
			}
			set
			{
				base.Message[AdminAuditLogSearchItemSchema.ResolvedUsers] = value.ToArray();
			}
		}

		public bool RedactDatacenterAdmins
		{
			get
			{
				return base.Message.GetValueOrDefault<bool>(AdminAuditLogSearchItemSchema.RedactDatacenterAdmins);
			}
			set
			{
				base.Message[AdminAuditLogSearchItemSchema.RedactDatacenterAdmins] = value;
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
			AdminAuditLogSearchItemSchema.Cmdlets,
			AdminAuditLogSearchItemSchema.Parameters,
			AdminAuditLogSearchItemSchema.ObjectIds,
			AdminAuditLogSearchItemSchema.RawUserIds,
			AdminAuditLogSearchItemSchema.ResolvedUsers,
			AdminAuditLogSearchItemSchema.RedactDatacenterAdmins
		};
	}
}
