using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAssociationUser : MailboxAssociationBaseItem, IMailboxAssociationUser, IMailboxAssociationBaseItem, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal MailboxAssociationUser(ICoreItem coreItem) : base(coreItem)
		{
		}

		public string JoinedBy
		{
			get
			{
				this.CheckDisposed("JoinedBy::get");
				return base.GetValueOrDefault<string>(MailboxAssociationUserSchema.JoinedBy, null);
			}
			set
			{
				this.CheckDisposed("JoinedBy::set");
				this[MailboxAssociationUserSchema.JoinedBy] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MailboxAssociationUserSchema.Instance;
			}
		}

		public override string AssociationItemClass
		{
			get
			{
				return "IPM.MailboxAssociation.User";
			}
		}

		public ExDateTime LastVisitedDate
		{
			get
			{
				this.CheckDisposed("LastVisitedDate::get");
				return base.GetValueOrDefault<ExDateTime>(MailboxAssociationUserSchema.LastVisitedDate, default(ExDateTime));
			}
			set
			{
				this.CheckDisposed("LastVisitedDate::set");
				this[MailboxAssociationUserSchema.LastVisitedDate] = value;
			}
		}

		public static MailboxAssociationUser Create(StoreSession session, StoreId folderId)
		{
			MailboxAssociationUser mailboxAssociationUser = ItemBuilder.CreateNewItem<MailboxAssociationUser>(session, folderId, ItemCreateInfo.MailboxAssociationUserInfo);
			mailboxAssociationUser[StoreObjectSchema.ItemClass] = "IPM.MailboxAssociation.User";
			return mailboxAssociationUser;
		}

		public new static MailboxAssociationUser Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<MailboxAssociationUser>(session, storeId, MailboxAssociationUserSchema.Instance, propsToReturn);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			base.AppendDescriptionTo(stringBuilder);
			stringBuilder.Append(", JoinedBy=");
			stringBuilder.Append(this.JoinedBy);
			stringBuilder.Append(", LastVisitedDate=");
			stringBuilder.Append(this.LastVisitedDate);
			return stringBuilder.ToString();
		}
	}
}
