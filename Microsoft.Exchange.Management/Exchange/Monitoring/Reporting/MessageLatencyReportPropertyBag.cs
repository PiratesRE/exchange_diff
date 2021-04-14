using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	[Serializable]
	internal class MessageLatencyReportPropertyBag : PropertyBag
	{
		public MessageLatencyReportPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public MessageLatencyReportPropertyBag() : base(false, 16)
		{
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return TransportReportSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return TransportReportSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return TransportReportSchema.Identity;
			}
		}
	}
}
