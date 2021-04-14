using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxFolderSchema : XsoMailboxConfigurationObjectSchema
	{
		public static readonly XsoDriverPropertyDefinition Name = new XsoDriverPropertyDefinition(FolderSchema.DisplayName, "Name", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, string.Empty, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition InternalFolderIdentity = new XsoDriverPropertyDefinition(FolderSchema.Id, "InternalFolderIdentity", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition InternalParentFolderIdentity = new XsoDriverPropertyDefinition(StoreObjectSchema.ParentItemId, "InternalParentFolderIdentity", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition FolderSize = new XsoDriverPropertyDefinition(FolderSchema.ExtendedSize, "FolderSize", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.ReadOnly, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition HasSubfolders = new XsoDriverPropertyDefinition(FolderSchema.HasChildren, "HasSubfolders", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.ReadOnly, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition FolderClass = new XsoDriverPropertyDefinition(StoreObjectSchema.ContainerClass, "FolderClass", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, string.Empty, "IPF.Note", PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition IsHidden = new XsoDriverPropertyDefinition(FolderSchema.IsHidden, "IsHidden", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition ExtendedFolderFlags = new XsoDriverPropertyDefinition(FolderSchema.ExtendedFolderFlags, "ExtendedFolderFlags", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DefaultFolderType = new SimpleProviderPropertyDefinition("DefaultFolderType", ExchangeObjectVersion.Exchange2003, typeof(DefaultFolderType?), PropertyDefinitionFlags.Mandatory, null, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DefaultFolderType))
		}, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition FolderPath = new SimpleProviderPropertyDefinition("FolderPath", ExchangeObjectVersion.Exchange2003, typeof(MapiFolderPath), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2003, typeof(MailboxFolderId), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated | PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxFolderSchema.FolderPath,
			MailboxFolderSchema.InternalFolderIdentity,
			XsoMailboxConfigurationObjectSchema.MailboxOwnerId
		}, null, new GetterDelegate(MailboxFolder.IdentityGetter), null);

		public static readonly SimpleProviderPropertyDefinition ParentFolder = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2003, typeof(MailboxFolderId), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxFolderSchema.FolderPath,
			MailboxFolderSchema.InternalFolderIdentity,
			MailboxFolderSchema.InternalParentFolderIdentity,
			XsoMailboxConfigurationObjectSchema.MailboxOwnerId
		}, null, new GetterDelegate(MailboxFolder.ParentFolderGetter), null);

		public static readonly SimpleProviderPropertyDefinition FolderStoreObjectId = new SimpleProviderPropertyDefinition("FolderStoreObjectId", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxFolderSchema.InternalFolderIdentity
		}, null, new GetterDelegate(MailboxFolder.FolderStoreObjectIdGetter), null);
	}
}
