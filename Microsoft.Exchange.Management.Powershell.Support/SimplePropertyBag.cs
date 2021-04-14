using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	internal class SimplePropertyBag : PropertyBag
	{
		public SimplePropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public SimplePropertyBag() : base(false, 16)
		{
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return InMemoryObjectSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return InMemoryObjectSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return InMemoryObjectSchema.Identity;
			}
		}
	}
}
