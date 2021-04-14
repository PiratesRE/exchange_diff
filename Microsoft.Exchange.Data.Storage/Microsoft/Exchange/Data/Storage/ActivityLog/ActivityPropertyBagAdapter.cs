using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ActivityPropertyBagAdapter : IPropertyBag
	{
		public ActivityPropertyBagAdapter(MemoryPropertyBag propertyBag)
		{
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			this.propertyBag = propertyBag;
		}

		public AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag)
		{
			if (propertyTag == PropertyTag.Mid)
			{
				return new AnnotatedPropertyValue(propertyTag, new PropertyValue(propertyTag, 0L), null);
			}
			object obj = this.propertyBag.TryGetProperty(ActivityPropertyBagAdapter.PropDefFromPropTag(propertyTag));
			PropertyValue propertyValue = PropertyError.IsPropertyError(obj) ? PropertyValue.Error(propertyTag.PropertyId, (ErrorCode)2147746063U) : new PropertyValue(propertyTag, obj);
			return new AnnotatedPropertyValue(propertyTag, propertyValue, null);
		}

		public IEnumerable<AnnotatedPropertyValue> GetAnnotatedProperties()
		{
			foreach (KeyValuePair<PropertyDefinition, object> prop in ((IEnumerable<KeyValuePair<PropertyDefinition, object>>)this.propertyBag))
			{
				KeyValuePair<PropertyDefinition, object> keyValuePair = prop;
				if (!PropertyError.IsPropertyError(keyValuePair.Value))
				{
					KeyValuePair<PropertyDefinition, object> keyValuePair2 = prop;
					PropertyTag propertyTag = new PropertyTag(((PropertyTagPropertyDefinition)keyValuePair2.Key).PropertyTag);
					PropertyTag propertyTag2 = propertyTag;
					PropertyTag propertyTag3 = propertyTag;
					KeyValuePair<PropertyDefinition, object> keyValuePair3 = prop;
					yield return new AnnotatedPropertyValue(propertyTag2, new PropertyValue(propertyTag3, keyValuePair3.Value), null);
				}
			}
			yield break;
		}

		public void SetProperty(PropertyValue propertyValue)
		{
			this.propertyBag.SetOrDeleteProperty(ActivityPropertyBagAdapter.PropDefFromPropTag(propertyValue.PropertyTag), propertyValue.Value);
		}

		public void Delete(PropertyTag property)
		{
			this.propertyBag.Delete(ActivityPropertyBagAdapter.PropDefFromPropTag(property));
		}

		public Stream GetPropertyStream(PropertyTag property)
		{
			throw new NotSupportedException("Reading properties as streams is not supported for activities.");
		}

		public Stream SetPropertyStream(PropertyTag property, long dataSizeEstimate)
		{
			if (property.PropertyType != PropertyType.Binary && property.PropertyType != PropertyType.Unicode)
			{
				throw new NotSupportedException("Writing properties as streams is only supported for binary and unicode properties for activities.");
			}
			return new PropertyBagStream(this.propertyBag, ActivityPropertyBagAdapter.PropDefFromPropTag(property), property.PropertyType, (int)dataSizeEstimate);
		}

		public ISession Session
		{
			get
			{
				throw new NotSupportedException("Named properties are not supported for activities.");
			}
		}

		private static PropertyTagPropertyDefinition PropDefFromPropTag(PropertyTag property)
		{
			return PropertyTagPropertyDefinition.InternalCreateCustom(string.Empty, (PropTag)property, PropertyFlags.None, NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck);
		}

		private readonly MemoryPropertyBag propertyBag;
	}
}
