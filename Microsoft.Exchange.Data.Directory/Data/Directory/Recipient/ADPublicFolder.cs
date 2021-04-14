using System;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[ProvisioningObjectTag("ADPublicFolder")]
	[Serializable]
	public sealed class ADPublicFolder : ADRecipient
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADPublicFolder.schema;
			}
		}

		internal override string ObjectCategoryName
		{
			get
			{
				return ADPublicFolder.ObjectCategoryNameInternal;
			}
		}

		public override string ObjectCategoryCN
		{
			get
			{
				return ADPublicFolder.ObjectCategoryCNInternal;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADPublicFolder.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal ADPublicFolder(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal ADPublicFolder(IRecipientSession session, string commonName, ADObjectId containerId)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId("CN", commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public ADPublicFolder()
		{
		}

		public MultiValuedProperty<ADObjectId> Contacts
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADPublicFolderSchema.Contacts];
			}
			internal set
			{
				this[ADPublicFolderSchema.Contacts] = value;
			}
		}

		public ADObjectId ContentMailbox
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.DefaultPublicFolderMailbox];
			}
			internal set
			{
				this[ADRecipientSchema.DefaultPublicFolderMailbox] = value;
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[ADPublicFolderSchema.Database];
			}
			set
			{
				this[ADPublicFolderSchema.Database] = value;
			}
		}

		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[ADPublicFolderSchema.DeliverToMailboxAndForward];
			}
			set
			{
				this[ADPublicFolderSchema.DeliverToMailboxAndForward] = value;
			}
		}

		public string EntryId
		{
			get
			{
				return (string)this[ADPublicFolderSchema.EntryId];
			}
			internal set
			{
				this[ADPublicFolderSchema.EntryId] = value;
			}
		}

		private static readonly ADPublicFolderSchema schema = ObjectSchema.GetInstance<ADPublicFolderSchema>();

		internal static string MostDerivedClass = "publicFolder";

		internal static string ObjectCategoryNameInternal = "publicFolder";

		internal static string ObjectCategoryCNInternal = "ms-Exch-Public-Folder";
	}
}
