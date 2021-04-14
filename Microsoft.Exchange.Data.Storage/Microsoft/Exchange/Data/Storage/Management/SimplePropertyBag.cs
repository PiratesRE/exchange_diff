using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SimplePropertyBag : PropertyBag
	{
		public SimplePropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public SimplePropertyBag(ProviderPropertyDefinition idPropDef, ProviderPropertyDefinition statePropDef, ProviderPropertyDefinition verPropDef, bool readOnly, int initialSize) : this(readOnly, initialSize)
		{
			this.objectIdentityPropertyDefinition = idPropDef;
			this.objectStatePropertyDefinition = statePropDef;
			this.objectVersionPropertyDefinition = verPropDef;
		}

		public SimplePropertyBag()
		{
		}

		public SimplePropertyBag(ProviderPropertyDefinition idPropDef, ProviderPropertyDefinition statePropDef, ProviderPropertyDefinition verPropDef) : this()
		{
			this.objectIdentityPropertyDefinition = idPropDef;
			this.objectStatePropertyDefinition = statePropDef;
			this.objectVersionPropertyDefinition = verPropDef;
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return this.objectVersionPropertyDefinition;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return this.objectStatePropertyDefinition;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return this.objectIdentityPropertyDefinition;
			}
		}

		public void SetObjectIdentityPropertyDefinition(ProviderPropertyDefinition idPd)
		{
			this.objectIdentityPropertyDefinition = idPd;
		}

		public void SetObjectStatePropertyDefinition(ProviderPropertyDefinition statePd)
		{
			this.objectStatePropertyDefinition = statePd;
		}

		public void SetObjectVersionPropertyDefinition(ProviderPropertyDefinition verPd)
		{
			this.objectVersionPropertyDefinition = verPd;
		}

		internal override MultiValuedPropertyBase CreateMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage)
		{
			return StoreValueConverter.CreateGenericMultiValuedProperty(propertyDefinition, createAsReadOnly, values, invalidValues, readOnlyErrorMessage);
		}

		private ProviderPropertyDefinition objectIdentityPropertyDefinition;

		private ProviderPropertyDefinition objectStatePropertyDefinition;

		private ProviderPropertyDefinition objectVersionPropertyDefinition;
	}
}
