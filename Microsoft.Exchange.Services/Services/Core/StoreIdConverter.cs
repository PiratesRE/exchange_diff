using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class StoreIdConverter : SimpleIdConverterBase
	{
		internal override StoreObjectId ConvertStringToStoreObjectId(string idValue)
		{
			if (string.IsNullOrEmpty(idValue))
			{
				ExTraceGlobals.ConvertIdCallTracer.TraceDebug((long)this.GetHashCode(), "[StoreIdConverter::ConvertStringToStoreObjectId] string idValue passed in was either null or empty");
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U);
			}
			StoreObjectId result;
			try
			{
				result = StoreObjectId.Deserialize(idValue);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidStoreIdException((CoreResources.IDs)3107705007U, innerException);
			}
			return result;
		}

		internal override string ConvertStoreObjectIdToString(StoreObjectId storeObjectId)
		{
			return storeObjectId.ToBase64String();
		}

		internal override IdFormat IdFormat
		{
			get
			{
				return IdFormat.StoreId;
			}
		}
	}
}
