using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderDumpsterInfoSchema : SimpleProviderObjectSchema
	{
		public static readonly ProviderPropertyDefinition DumpsterHolderEntryId = new SimpleProviderPropertyDefinition("DumpsterHolderEntryId", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.PersistDefaultValue, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CountTotalFolders = new SimpleProviderPropertyDefinition("CountTotalFolders", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition HasDumpsterExtended = new SimpleProviderPropertyDefinition("HasDumpsterExtended", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CountLegacyDumpsters = new SimpleProviderPropertyDefinition("CountLegacyDumpsters", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CountContainerLevel1 = new SimpleProviderPropertyDefinition("CountContainerLevel1", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CountContainerLevel2 = new SimpleProviderPropertyDefinition("CountContainerLevel2", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CountDumpsters = new SimpleProviderPropertyDefinition("CountDumpsters", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CountDeletedFolders = new SimpleProviderPropertyDefinition("CountDeletedFolders", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
