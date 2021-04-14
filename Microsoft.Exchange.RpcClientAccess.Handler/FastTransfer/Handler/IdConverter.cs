using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal abstract class IdConverter
	{
		protected IdConverter(PropertyTag clientProperty, PropertyDefinition serverProperty)
		{
			this.ClientProperty = clientProperty;
			this.ServerProperty = serverProperty;
		}

		internal bool GetClientId(StoreSession session, ICorePropertyBag propertyBag, PropertyTag property, out PropertyValue clientValue)
		{
			clientValue = default(PropertyValue);
			if (property != this.ClientProperty)
			{
				return false;
			}
			StoreId storeId = propertyBag.TryGetProperty(this.ServerProperty) as StoreId;
			if (storeId == null)
			{
				clientValue = PropertyValue.Error(this.ClientProperty.PropertyId, (ErrorCode)2147746063U);
			}
			else
			{
				clientValue = new PropertyValue(this.ClientProperty, this.CreateClientId(session, storeId));
			}
			return true;
		}

		protected abstract long CreateClientId(StoreSession session, StoreId id);

		protected readonly PropertyTag ClientProperty;

		protected readonly PropertyDefinition ServerProperty;
	}
}
