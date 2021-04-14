using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class AttributionConverter : BaseConverter
	{
		public override object ConvertToObject(string propertyString)
		{
			return null;
		}

		public override string ConvertToString(object propertyValue)
		{
			return string.Empty;
		}

		public override object ConvertToServiceObjectValue(object propertyValue, IdConverterWithCommandSettings idConverterWithCommandSettings)
		{
			if (propertyValue == null)
			{
				return null;
			}
			Attribution attribution = (Attribution)propertyValue;
			ItemId itemId = null;
			if (attribution.SourceId != null)
			{
				if (attribution.SourceId.IsStoreId)
				{
					itemId = idConverterWithCommandSettings.PersonaIdFromStoreId(attribution.SourceId.StoreId);
				}
				else if (attribution.SourceId.IsADObjectIdGuid)
				{
					itemId = IdConverter.PersonaIdFromADObjectId(attribution.SourceId.ADObjectIdGuid);
				}
			}
			if (itemId == null)
			{
				itemId = new ItemId
				{
					Id = string.Empty,
					ChangeKey = string.Empty
				};
			}
			FolderId folderId = null;
			if (attribution.FolderId != null)
			{
				ConcatenatedIdAndChangeKey concatenatedId = idConverterWithCommandSettings.GetConcatenatedId(attribution.FolderId);
				folderId = new FolderId
				{
					Id = concatenatedId.Id,
					ChangeKey = concatenatedId.ChangeKey
				};
			}
			return new Attribution
			{
				Id = attribution.Id,
				SourceId = itemId,
				DisplayName = attribution.DisplayName,
				IsWritable = attribution.IsWritable,
				IsQuickContact = attribution.IsQuickContact,
				IsHidden = attribution.IsHidden,
				FolderId = folderId,
				FolderName = attribution.FolderName
			};
		}
	}
}
