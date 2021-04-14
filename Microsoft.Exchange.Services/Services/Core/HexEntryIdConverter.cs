using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class HexEntryIdConverter : SimpleIdConverterBase
	{
		internal override StoreObjectId ConvertStringToStoreObjectId(string idValue)
		{
			if (string.IsNullOrEmpty(idValue))
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U);
			}
			StoreObjectId result;
			try
			{
				ByteArray byteArray = ByteArray.Parse(idValue);
				result = StoreObjectId.FromProviderSpecificId(byteArray.Bytes);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U, innerException);
			}
			return result;
		}

		internal override string ConvertStoreObjectIdToString(StoreObjectId storeObjectId)
		{
			byte[] providerLevelItemId = storeObjectId.ProviderLevelItemId;
			ByteArray byteArray = new ByteArray(providerLevelItemId);
			return byteArray.ToString();
		}

		internal override IdFormat IdFormat
		{
			get
			{
				return IdFormat.HexEntryId;
			}
		}
	}
}
