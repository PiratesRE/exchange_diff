using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	internal class PhysicalAvailabilityReportPropertyBag : PropertyBag
	{
		public PhysicalAvailabilityReportPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		public PhysicalAvailabilityReportPropertyBag() : base(false, 16)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return PhysicalAvailabilityReportSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return PhysicalAvailabilityReportSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return PhysicalAvailabilityReportSchema.Identity;
			}
		}
	}
}
