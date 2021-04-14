using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderItemStatisticsSchema : MailMessageSchema
	{
		public static readonly XsoDriverPropertyDefinition EntryId = new XsoDriverPropertyDefinition(StoreObjectSchema.EntryId, "EntryId", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition LastModifiedTime = new XsoDriverPropertyDefinition(StoreObjectSchema.LastModifiedTime, "LastModifiedTime", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition CreationTime = new XsoDriverPropertyDefinition(StoreObjectSchema.CreationTime, "CreationTime", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition HasAttachment = new XsoDriverPropertyDefinition(ItemSchema.HasAttachment, "HasAttachment", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition ItemClass = new XsoDriverPropertyDefinition(StoreObjectSchema.ItemClass, "ItemClass", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition Size = new XsoDriverPropertyDefinition(ItemSchema.Size, "Size", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
