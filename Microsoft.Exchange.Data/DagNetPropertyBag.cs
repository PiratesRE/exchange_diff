using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DagNetPropertyBag : PropertyBag
	{
		public DagNetPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public DagNetPropertyBag()
		{
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return DatabaseAvailabilityGroupNetworkSchema.Identity;
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
