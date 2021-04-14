using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncContact : SyncOrgPerson
	{
		public SyncContact(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public override SyncObjectSchema Schema
		{
			get
			{
				return SyncContact.schema;
			}
		}

		internal override DirectoryObjectClass ObjectClass
		{
			get
			{
				return DirectoryObjectClass.Contact;
			}
		}

		protected override DirectoryObject CreateDirectoryObject()
		{
			return new Contact();
		}

		public override SyncRecipient CreatePlaceHolder()
		{
			SyncContact syncContact = (SyncContact)base.CreatePlaceHolder();
			if (base.ExternalEmailAddress.HasValue)
			{
				syncContact.ExternalEmailAddress = base.ExternalEmailAddress.Value;
			}
			return syncContact;
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> Manager
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncContactSchema.Manager];
			}
			set
			{
				base[SyncContactSchema.Manager] = value;
			}
		}

		private static readonly SyncContactSchema schema = ObjectSchema.GetInstance<SyncContactSchema>();
	}
}
