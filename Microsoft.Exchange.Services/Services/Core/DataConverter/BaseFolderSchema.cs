using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class BaseFolderSchema : Schema
	{
		static BaseFolderSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				BaseFolderSchema.FolderId,
				BaseFolderSchema.ParentFolderId,
				BaseFolderSchema.FolderClass,
				BaseFolderSchema.DisplayName,
				BaseFolderSchema.TotalCount,
				BaseFolderSchema.ChildFolderCount,
				BaseFolderSchema.ExtendedProperty,
				BaseFolderSchema.ManagedFolderInformation,
				BaseFolderSchema.EffectiveRights,
				BaseFolderSchema.DistinguishedFolderId,
				BaseFolderSchema.PolicyTag,
				BaseFolderSchema.ArchiveTag,
				BaseFolderSchema.UnClutteredViewFolderEntryId,
				BaseFolderSchema.ClutteredViewFolderEntryId,
				BaseFolderSchema.ClutterCount,
				BaseFolderSchema.UnreadClutterCount,
				BaseFolderSchema.ReplicaList
			};
			BaseFolderSchema.schema = new BaseFolderSchema(xmlElements, BaseFolderSchema.FolderId);
		}

		private BaseFolderSchema(XmlElementInformation[] xmlElements, PropertyInformation idPropertyInformation) : base(xmlElements, idPropertyInformation)
		{
			IList<PropertyInformation> propertyInformationListByShapeEnum = base.GetPropertyInformationListByShapeEnum(ShapeEnum.AllProperties);
			propertyInformationListByShapeEnum.Remove(BaseFolderSchema.ExtendedProperty);
			propertyInformationListByShapeEnum.Remove(BaseFolderSchema.DistinguishedFolderId);
			propertyInformationListByShapeEnum.Remove(BaseFolderSchema.UnClutteredViewFolderEntryId);
			propertyInformationListByShapeEnum.Remove(BaseFolderSchema.ClutteredViewFolderEntryId);
			propertyInformationListByShapeEnum.Remove(BaseFolderSchema.ClutterCount);
			propertyInformationListByShapeEnum.Remove(BaseFolderSchema.UnreadClutterCount);
			propertyInformationListByShapeEnum.Remove(BaseFolderSchema.ReplicaList);
		}

		public static Schema GetSchema()
		{
			return BaseFolderSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation FolderId = new PropertyInformation("FolderId", ExchangeVersion.Exchange2007, FolderSchema.Id, new PropertyUri(PropertyUriEnum.FolderId), new PropertyCommand.CreatePropertyCommand(FolderIdProperty.CreateCommand));

		public static readonly PropertyInformation FolderClass = new PropertyInformation("FolderClass", ExchangeVersion.Exchange2007, StoreObjectSchema.ContainerClass, new PropertyUri(PropertyUriEnum.FolderClass), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation ParentFolderId = new PropertyInformation("ParentFolderId", ExchangeVersion.Exchange2007, StoreObjectSchema.ParentItemId, new PropertyUri(PropertyUriEnum.ParentFolderId), new PropertyCommand.CreatePropertyCommand(ParentFolderIdProperty.CreateCommand));

		public static readonly PropertyInformation DisplayName = new PropertyInformation("DisplayName", ExchangeVersion.Exchange2007, FolderSchema.DisplayName, new PropertyUri(PropertyUriEnum.FolderDisplayName), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation TotalCount = new PropertyInformation("TotalCount", ExchangeVersion.Exchange2007, FolderSchema.ItemCount, new PropertyUri(PropertyUriEnum.TotalCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ChildFolderCount = new PropertyInformation("ChildFolderCount", ExchangeVersion.Exchange2007, FolderSchema.ChildCount, new PropertyUri(PropertyUriEnum.ChildFolderCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ExtendedProperty = new ExtendedPropertyInformation();

		public static readonly PropertyInformation ManagedFolderInformation = new ManagedFolderPropertyInformation();

		public static readonly PropertyInformation EffectiveRights = new PropertyInformation("EffectiveRights", ExchangeVersion.Exchange2007SP1, StoreObjectSchema.EffectiveRights, new PropertyUri(PropertyUriEnum.FolderEffectiveRights), new PropertyCommand.CreatePropertyCommand(EffectiveRightsProperty.CreateCommand));

		public static readonly PropertyInformation DistinguishedFolderId = new PropertyInformation("DistinguishedFolderId", ExchangeVersion.Exchange2012, FolderSchema.Id, new PropertyUri(PropertyUriEnum.DistinguishedFolderId), new PropertyCommand.CreatePropertyCommand(DistinguishedFolderIdProperty.CreateCommand));

		public static readonly PropertyInformation PolicyTag = new PropertyInformation("PolicyTag", ServiceXml.GetFullyQualifiedName("PolicyTag"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2012, new PropertyDefinition[]
		{
			StoreObjectSchema.PolicyTag,
			StoreObjectSchema.RetentionPeriod,
			StoreObjectSchema.RetentionFlags
		}, new PropertyUri(PropertyUriEnum.FolderPolicyTag), new PropertyCommand.CreatePropertyCommand(FolderPolicyTagProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation ArchiveTag = new PropertyInformation("ArchiveTag", ServiceXml.GetFullyQualifiedName("ArchiveTag"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2012, new PropertyDefinition[]
		{
			StoreObjectSchema.ArchiveTag,
			StoreObjectSchema.ArchivePeriod,
			StoreObjectSchema.RetentionFlags
		}, new PropertyUri(PropertyUriEnum.FolderArchiveTag), new PropertyCommand.CreatePropertyCommand(FolderArchiveTagProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation UnClutteredViewFolderEntryId = new PropertyInformation("UnClutteredViewFolderEntryId", ExchangeVersion.Exchange2012, FolderSchema.UnClutteredViewFolderEntryId, new PropertyUri(PropertyUriEnum.UnClutteredViewFolderEntryId), new PropertyCommand.CreatePropertyCommand(LinkedFolderIdProperty.Create), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ClutteredViewFolderEntryId = new PropertyInformation("ClutteredViewFolderEntryId", ExchangeVersion.Exchange2012, FolderSchema.ClutteredViewFolderEntryId, new PropertyUri(PropertyUriEnum.ClutteredViewFolderEntryId), new PropertyCommand.CreatePropertyCommand(LinkedFolderIdProperty.Create), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ClutterCount = new PropertyInformation("ClutterCount", ServiceXml.GetFullyQualifiedName("ClutterCount"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2012, new PropertyDefinition[]
		{
			FolderSchema.ClutteredViewFolderEntryId,
			FolderSchema.ItemCount
		}, new PropertyUri(PropertyUriEnum.ClutterCount), new PropertyCommand.CreatePropertyCommand(ClutterCountProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation UnreadClutterCount = new PropertyInformation("UnreadClutterCount", ServiceXml.GetFullyQualifiedName("UnreadClutterCount"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2012, new PropertyDefinition[]
		{
			FolderSchema.ClutteredViewFolderEntryId,
			FolderSchema.UnreadCount
		}, new PropertyUri(PropertyUriEnum.UnreadClutterCount), new PropertyCommand.CreatePropertyCommand(ClutterCountProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ReplicaList = new ArrayPropertyInformation("ReplicaList", ExchangeVersion.Exchange2013_SP1, "String", FolderSchema.ReplicaList, new PropertyUri(PropertyUriEnum.ReplicaList), new PropertyCommand.CreatePropertyCommand(ReplicaListProperty.CreateCommand));
	}
}
