using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncAccount : SyncObject
	{
		public SyncAccount(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public override SyncObjectSchema Schema
		{
			get
			{
				return SyncAccount.schema;
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
			return new Account();
		}

		public SyncProperty<string> DisplayName
		{
			get
			{
				return (SyncProperty<string>)base[SyncAccountSchema.DisplayName];
			}
			set
			{
				base[SyncAccountSchema.DisplayName] = value;
			}
		}

		private static readonly SyncAccountSchema schema = ObjectSchema.GetInstance<SyncAccountSchema>();
	}
}
