using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class SimpleProviderPropertyBag : PropertyBag
	{
		public SimpleProviderPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public SimpleProviderPropertyBag()
		{
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return SimpleProviderObjectSchema.Identity;
			}
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return SimpleProviderObjectSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return SimpleProviderObjectSchema.ObjectState;
			}
		}
	}
}
