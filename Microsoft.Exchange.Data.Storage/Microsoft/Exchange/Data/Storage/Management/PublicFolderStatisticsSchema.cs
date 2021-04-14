using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderStatisticsSchema : XsoMailboxConfigurationObjectSchema
	{
		public static readonly XsoDriverPropertyDefinition AssociatedItemCount = new XsoDriverPropertyDefinition(FolderSchema.AssociatedItemCount, "AssociatedItemCount", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition CreationTime = new XsoDriverPropertyDefinition(StoreObjectSchema.CreationTime, "CreationTime", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition EntryId = new XsoDriverPropertyDefinition(FolderSchema.Id, "Id", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition FolderPath = new XsoDriverPropertyDefinition(FolderSchema.FolderPathName, "FolderPath", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition ItemCount = new XsoDriverPropertyDefinition(FolderSchema.ItemCount, "ItemCount", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition LastModificationTime = new XsoDriverPropertyDefinition(StoreObjectSchema.LastModifiedTime, "LastModifiedTime", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition Name = new XsoDriverPropertyDefinition(FolderSchema.DisplayName, "DisplayName", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition TotalAssociatedItemSize = new XsoDriverPropertyDefinition(InternalSchema.ExtendedAssociatedItemSize, "TotalAssociatedItemSize", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition TotalItemSize = new XsoDriverPropertyDefinition(FolderSchema.ExtendedSize, "ExtendedSize", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
