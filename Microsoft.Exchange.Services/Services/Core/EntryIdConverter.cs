using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class EntryIdConverter : SimpleIdConverterBase
	{
		internal override StoreObjectId ConvertStringToStoreObjectId(string idValue)
		{
			if (string.IsNullOrEmpty(idValue))
			{
				ExTraceGlobals.ConvertIdCallTracer.TraceDebug((long)this.GetHashCode(), "[EntryIdConverter::ConvertStringToStoreObjectId] string idValue passed in was either null or empty");
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U);
			}
			StoreObjectId result;
			try
			{
				byte[] entryId = Convert.FromBase64String(idValue);
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(entryId);
				result = storeObjectId;
			}
			catch (FormatException innerException)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U, innerException);
			}
			catch (ArgumentException innerException2)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U, innerException2);
			}
			return result;
		}

		internal override string ConvertStoreObjectIdToString(StoreObjectId storeObjectId)
		{
			return Convert.ToBase64String(storeObjectId.ProviderLevelItemId);
		}

		internal override IdFormat IdFormat
		{
			get
			{
				return IdFormat.EntryId;
			}
		}
	}
}
