using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal static class IdConverters
	{
		internal static bool GetClientId(StoreSession propertyMappingReference, ICorePropertyBag propertyBag, PropertyTag property, out PropertyValue clientValue)
		{
			foreach (IdConverter idConverter in IdConverters.converters)
			{
				if (idConverter.GetClientId(propertyMappingReference, propertyBag, property, out clientValue))
				{
					return true;
				}
			}
			clientValue = default(PropertyValue);
			return false;
		}

		private static readonly IdConverter[] converters = new IdConverter[]
		{
			new FolderIdConverter(),
			new MessageIdConverter()
		};
	}
}
