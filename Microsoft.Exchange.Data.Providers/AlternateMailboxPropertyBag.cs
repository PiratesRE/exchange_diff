using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Providers
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class AlternateMailboxPropertyBag : PropertyBag
	{
		public AlternateMailboxPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public AlternateMailboxPropertyBag()
		{
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return AlternateMailboxSchema.Identity;
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
