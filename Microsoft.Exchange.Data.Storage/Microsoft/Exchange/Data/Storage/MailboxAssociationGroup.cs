using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAssociationGroup : MailboxAssociationBaseItem, IMailboxAssociationGroup, IMailboxAssociationBaseItem, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal MailboxAssociationGroup(ICoreItem coreItem) : base(coreItem)
		{
		}

		public ExDateTime PinDate
		{
			get
			{
				this.CheckDisposed("PinDate::get");
				return base.GetValueOrDefault<ExDateTime>(MailboxAssociationGroupSchema.PinDate, default(ExDateTime));
			}
			set
			{
				this.CheckDisposed("PinDate::set");
				this[MailboxAssociationGroupSchema.PinDate] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MailboxAssociationGroupSchema.Instance;
			}
		}

		public override string AssociationItemClass
		{
			get
			{
				return "IPM.MailboxAssociation.Group";
			}
		}

		public static MailboxAssociationGroup Create(StoreSession session, StoreId folderId)
		{
			MailboxAssociationGroup mailboxAssociationGroup = ItemBuilder.CreateNewItem<MailboxAssociationGroup>(session, folderId, ItemCreateInfo.MailboxAssociationGroupInfo);
			mailboxAssociationGroup[StoreObjectSchema.ItemClass] = "IPM.MailboxAssociation.Group";
			return mailboxAssociationGroup;
		}

		public new static MailboxAssociationGroup Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<MailboxAssociationGroup>(session, storeId, MailboxAssociationGroupSchema.Instance, propsToReturn);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			base.AppendDescriptionTo(stringBuilder);
			stringBuilder.Append(", PinDate=");
			stringBuilder.Append(this.PinDate);
			return stringBuilder.ToString();
		}
	}
}
