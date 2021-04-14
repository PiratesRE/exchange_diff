using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	internal class CasTransactionPropertyBag : PropertyBag
	{
		public CasTransactionPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public CasTransactionPropertyBag() : base(false, 16)
		{
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return CasTransationObjectSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return CasTransationObjectSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return CasTransationObjectSchema.Identity;
			}
		}
	}
}
