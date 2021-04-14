using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	internal class ServiceStatusPropertyBag : PropertyBag
	{
		public ServiceStatusPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		public ServiceStatusPropertyBag() : base(false, 16)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return ServiceStatusSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return ServiceStatusSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return ServiceStatusSchema.Identity;
			}
		}
	}
}
