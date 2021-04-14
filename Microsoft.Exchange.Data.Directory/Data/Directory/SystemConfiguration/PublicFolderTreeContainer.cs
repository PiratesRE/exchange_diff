using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class PublicFolderTreeContainer : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return PublicFolderTreeContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return PublicFolderTreeContainer.mostDerivedClass;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(ADConfigurationObjectSchema.AdminDisplayName))
			{
				base.AdminDisplayName = PublicFolderTreeContainer.DefaultName;
			}
			if (!base.IsModified(ADConfigurationObjectSchema.SystemFlags))
			{
				this[ADConfigurationObjectSchema.SystemFlags] = SystemFlagsEnum.None;
			}
			base.StampPersistableDefaultValues();
		}

		public static readonly string DefaultName = "Folder Hierarchies";

		private static PublicFolderTreeContainerSchema schema = ObjectSchema.GetInstance<PublicFolderTreeContainerSchema>();

		private static string mostDerivedClass = "msExchContainer";
	}
}
