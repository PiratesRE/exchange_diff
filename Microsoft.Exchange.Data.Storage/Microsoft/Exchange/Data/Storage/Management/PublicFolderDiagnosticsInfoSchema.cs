using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderDiagnosticsInfoSchema : SimpleProviderObjectSchema
	{
		public static readonly ProviderPropertyDefinition DisplayName = new SimpleProviderPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition AssistantInfo = new SimpleProviderPropertyDefinition("AssistantInfo", ExchangeObjectVersion.Exchange2010, typeof(PublicFolderMailboxAssistantInfo), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SyncInfo = new SimpleProviderPropertyDefinition("SyncInfo", ExchangeObjectVersion.Exchange2010, typeof(PublicFolderMailboxSynchronizerInfo), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition DumpsterInfo = new SimpleProviderPropertyDefinition("DumpsterInfo", ExchangeObjectVersion.Exchange2012, typeof(PublicFolderMailboxDumpsterInfo), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition HierarchyInfo = new SimpleProviderPropertyDefinition("HierarchyInfo", ExchangeObjectVersion.Exchange2012, typeof(PublicFolderMailboxHierarchyInfo), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
