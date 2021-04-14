using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Supervision
{
	[Serializable]
	internal class SupervisionPolicyPropertyBag : PropertyBag
	{
		public SupervisionPolicyPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		public SupervisionPolicyPropertyBag() : base(false, 16)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return SupervisionPolicySchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return SupervisionPolicySchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return SupervisionPolicySchema.Identity;
			}
		}
	}
}
