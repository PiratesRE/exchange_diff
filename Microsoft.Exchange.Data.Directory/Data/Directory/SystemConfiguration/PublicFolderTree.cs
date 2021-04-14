using System;
using System.Security.AccessControl;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class PublicFolderTree : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return PublicFolderTree.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return PublicFolderTree.MostDerivedClass;
			}
		}

		public PublicFolderTreeType PublicFolderTreeType
		{
			get
			{
				return (PublicFolderTreeType)this[PublicFolderTreeSchema.PublicFolderTreeType];
			}
			internal set
			{
				this[PublicFolderTreeSchema.PublicFolderTreeType] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> PublicDatabases
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[PublicFolderTreeSchema.PublicDatabases];
			}
		}

		internal void SetPublicFolderDefaultAdminAcl(RawSecurityDescriptor rawSecurityDescriptor)
		{
			this[PublicFolderTreeSchema.PublicFolderDefaultAdminAcl] = rawSecurityDescriptor;
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(ADConfigurationObjectSchema.AdminDisplayName))
			{
				base.AdminDisplayName = PublicFolderTree.DefaultName;
			}
			if (!base.IsModified(PublicFolderTreeSchema.PublicFolderTreeType))
			{
				this.PublicFolderTreeType = PublicFolderTreeType.Mapi;
			}
			if (!base.IsModified(ADConfigurationObjectSchema.SystemFlags))
			{
				this[ADConfigurationObjectSchema.SystemFlags] = (SystemFlagsEnum.Movable | SystemFlagsEnum.Renamable);
			}
			base.StampPersistableDefaultValues();
		}

		public static readonly string DefaultName = "Public Folders";

		private static PublicFolderTreeSchema schema = ObjectSchema.GetInstance<PublicFolderTreeSchema>();

		internal static readonly string MostDerivedClass = "msExchPFTree";
	}
}
