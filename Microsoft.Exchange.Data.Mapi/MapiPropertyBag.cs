using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	internal class MapiPropertyBag : PropertyBag
	{
		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return MapiObjectSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return MapiObjectSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return MapiObjectSchema.Identity;
			}
		}

		internal bool IsChangedOrInitialized(ProviderPropertyDefinition key)
		{
			if (!base.IsModified(key))
			{
				return false;
			}
			if (base.IsChanged(key))
			{
				return true;
			}
			object obj = null;
			base.TryGetField(key, ref obj);
			return key.DefaultValue == obj;
		}

		internal override MultiValuedPropertyBase CreateMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage)
		{
			return ADValueConvertor.CreateGenericMultiValuedProperty(propertyDefinition, createAsReadOnly, values, invalidValues, readOnlyErrorMessage);
		}
	}
}
