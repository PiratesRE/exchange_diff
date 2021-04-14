using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NextHopSolutionKeyPropertyBag : PropertyBag
	{
		public NextHopSolutionKeyPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public NextHopSolutionKeyPropertyBag() : base(false, 16)
		{
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return NextHopSolutionKeyObjectSchema.Version;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return NextHopSolutionKeyObjectSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return NextHopSolutionKeyObjectSchema.Id;
			}
		}
	}
}
