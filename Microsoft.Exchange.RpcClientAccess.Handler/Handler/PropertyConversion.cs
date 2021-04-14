using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal abstract class PropertyConversion
	{
		protected PropertyConversion(PropertyTag clientPropertyTag, PropertyTag serverPropertyTag)
		{
			this.ClientPropertyTag = clientPropertyTag;
			this.ServerPropertyTag = serverPropertyTag;
		}

		internal bool TryConvertPropertyTagFromClient(PropertyTag clientPropertyTag, out PropertyTag serverPropertyTag)
		{
			if (clientPropertyTag == this.ClientPropertyTag)
			{
				serverPropertyTag = this.ServerPropertyTag;
				return true;
			}
			if (clientPropertyTag.PropertyId == this.ClientPropertyTag.PropertyId && clientPropertyTag.PropertyType == PropertyType.Unspecified)
			{
				serverPropertyTag = new PropertyTag(this.ServerPropertyTag.PropertyId, PropertyType.Unspecified);
				return true;
			}
			serverPropertyTag = clientPropertyTag;
			return false;
		}

		internal bool TryConvertPropertyTagToClient(PropertyTag serverPropertyTag, PropertyTag? originalClientPropertyTag, out PropertyTag clientPropertyTag)
		{
			if (this.CanConvertPropertyTagToClient(serverPropertyTag, originalClientPropertyTag))
			{
				if (serverPropertyTag == this.ServerPropertyTag)
				{
					clientPropertyTag = this.ClientPropertyTag;
				}
				else
				{
					clientPropertyTag = new PropertyTag(this.ClientPropertyTag.PropertyId, this.ServerPropertyTag.PropertyType);
				}
				return true;
			}
			clientPropertyTag = serverPropertyTag;
			return false;
		}

		internal bool TryConvertPropertyValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, ref PropertyValue propertyValue)
		{
			if (propertyValue.PropertyTag == this.ClientPropertyTag)
			{
				propertyValue = new PropertyValue(this.ServerPropertyTag, this.ConvertValueFromClient(session, storageObjectProperties, propertyValue.Value));
				return true;
			}
			return false;
		}

		internal bool TryConvertPropertyValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, PropertyTag? originalClientPropertyTag, ref PropertyValue propertyValue)
		{
			if (this.CanConvertPropertyTagToClient(propertyValue.PropertyTag, originalClientPropertyTag))
			{
				if (propertyValue.PropertyTag == this.ServerPropertyTag)
				{
					propertyValue = new PropertyValue(this.ClientPropertyTag, this.ConvertValueToClient(session, storageObjectProperties, propertyValue.Value));
				}
				else
				{
					propertyValue = propertyValue.CloneAs(this.ClientPropertyTag.PropertyId);
				}
				return true;
			}
			return false;
		}

		protected abstract object ConvertValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue);

		protected abstract object ConvertValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, object propertyValue);

		private bool CanConvertPropertyTagToClient(PropertyTag serverPropertyTag, PropertyTag? originalClientPropertyTag)
		{
			return (serverPropertyTag == this.ServerPropertyTag || (serverPropertyTag.PropertyId == this.ServerPropertyTag.PropertyId && serverPropertyTag.PropertyType == PropertyType.Error)) && (originalClientPropertyTag == null || originalClientPropertyTag.Value == this.ClientPropertyTag || (originalClientPropertyTag.Value.PropertyId == this.ClientPropertyTag.PropertyId && originalClientPropertyTag.Value.PropertyType == PropertyType.Unspecified));
		}

		internal readonly PropertyTag ClientPropertyTag;

		internal readonly PropertyTag ServerPropertyTag;
	}
}
