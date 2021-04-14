using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ADPropertyBag : PropertyBag
	{
		public ADPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public ADPropertyBag() : base(false, 16)
		{
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return ADObjectSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return ADObjectSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return ADObjectSchema.Id;
			}
		}

		public override object this[PropertyDefinition key]
		{
			get
			{
				return base[key];
			}
			set
			{
				base[key] = value;
				if (key != ADObjectSchema.RawName)
				{
					if (key == ADObjectSchema.Id)
					{
						ADObjectId adobjectId = (ADObjectId)this[ADObjectSchema.Id];
						base.SetField(ADObjectSchema.RawName, adobjectId.Rdn.UnescapedName);
					}
					return;
				}
				ADObjectId adobjectId2 = (ADObjectId)this[ADObjectSchema.Id];
				if (adobjectId2 == null)
				{
					return;
				}
				string prefix = adobjectId2.Rdn.Prefix;
				ADObjectId childId = adobjectId2.Parent.GetChildId(prefix, (string)value);
				base.SetField(ADObjectSchema.Id, new ADObjectId(childId.DistinguishedName, adobjectId2.ObjectGuid));
			}
		}

		internal override MultiValuedPropertyBase CreateMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage)
		{
			return ADValueConvertor.CreateGenericMultiValuedProperty(propertyDefinition, createAsReadOnly, values, invalidValues, readOnlyErrorMessage);
		}

		internal override object SerializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			return ADValueConvertor.SerializeData(propertyDefinition, input);
		}

		internal override object DeserializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			return ADValueConvertor.DeserializeData(propertyDefinition, input);
		}
	}
}
