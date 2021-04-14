using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class DelegatedPropertyBag : ADPropertyBag
	{
		public DelegatedPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public DelegatedPropertyBag() : base(false, 16)
		{
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return DelegatedObjectSchema.Identity;
			}
		}
	}
}
