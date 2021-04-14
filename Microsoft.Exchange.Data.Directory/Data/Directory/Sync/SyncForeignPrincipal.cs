using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncForeignPrincipal : SyncObject
	{
		public SyncForeignPrincipal(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public override SyncObjectSchema Schema
		{
			get
			{
				return SyncForeignPrincipal.schema;
			}
		}

		internal override DirectoryObjectClass ObjectClass
		{
			get
			{
				return DirectoryObjectClass.ForeignPrincipal;
			}
		}

		protected override DirectoryObject CreateDirectoryObject()
		{
			return new ForeignPrincipal();
		}

		public SyncProperty<string> DisplayName
		{
			get
			{
				return (SyncProperty<string>)base[SyncForeignPrincipalSchema.DisplayName];
			}
			set
			{
				base[SyncForeignPrincipalSchema.DisplayName] = value;
			}
		}

		public SyncProperty<string> Description
		{
			get
			{
				return (SyncProperty<string>)base[SyncForeignPrincipalSchema.Description];
			}
			set
			{
				base[SyncForeignPrincipalSchema.Description] = value;
			}
		}

		internal SyncProperty<string> LinkedPartnerGroupId
		{
			get
			{
				return (SyncProperty<string>)base[SyncForeignPrincipalSchema.LinkedPartnerGroupId];
			}
			set
			{
				base[SyncForeignPrincipalSchema.LinkedPartnerGroupId] = value;
			}
		}

		internal SyncProperty<string> LinkedPartnerOrganizationId
		{
			get
			{
				return (SyncProperty<string>)base[SyncForeignPrincipalSchema.LinkedPartnerOrganizationId];
			}
			set
			{
				base[SyncForeignPrincipalSchema.LinkedPartnerOrganizationId] = value;
			}
		}

		private static readonly SyncForeignPrincipalSchema schema = ObjectSchema.GetInstance<SyncForeignPrincipalSchema>();
	}
}
