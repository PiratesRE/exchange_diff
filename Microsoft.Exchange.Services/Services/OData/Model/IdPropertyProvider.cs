using System;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class IdPropertyProvider : AggregatedPropertyProvider
	{
		public override PropertyProvider SelectProvider(EntitySchema schema)
		{
			if (schema is ItemSchema)
			{
				return IdPropertyProvider.ItemIdProvider;
			}
			if (schema is FolderSchema || schema is ContactFolderSchema)
			{
				return IdPropertyProvider.FolderIdProvider;
			}
			if (schema is AttachmentSchema)
			{
				return IdPropertyProvider.AttachmentIdProvider;
			}
			return null;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static IdPropertyProvider()
		{
			SimpleEwsPropertyProvider simpleEwsPropertyProvider = new SimpleEwsPropertyProvider(ItemSchema.ItemId);
			simpleEwsPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e.Id = EwsIdConverter.EwsIdToODataId((s[sp] as ItemId).Id);
			};
			simpleEwsPropertyProvider.QueryConstantBuilder = ((object o) => EwsIdConverter.ODataIdToEwsId((string)o));
			IdPropertyProvider.ItemIdProvider = simpleEwsPropertyProvider;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider2 = new SimpleEwsPropertyProvider(BaseFolderSchema.FolderId);
			simpleEwsPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e.Id = EwsIdConverter.EwsIdToODataId((s[sp] as FolderId).Id);
			};
			simpleEwsPropertyProvider2.QueryConstantBuilder = ((object o) => EwsIdConverter.ODataIdToEwsId((string)o));
			IdPropertyProvider.FolderIdProvider = simpleEwsPropertyProvider2;
			GenericPropertyProvider<AttachmentType> genericPropertyProvider = new GenericPropertyProvider<AttachmentType>();
			genericPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				e.Id = EwsIdConverter.EwsIdToODataId(a.AttachmentId.Id);
			};
			IdPropertyProvider.AttachmentIdProvider = genericPropertyProvider;
		}

		private static readonly SimpleEwsPropertyProvider ItemIdProvider;

		private static readonly SimpleEwsPropertyProvider FolderIdProvider;

		private static readonly GenericPropertyProvider<AttachmentType> AttachmentIdProvider;
	}
}
