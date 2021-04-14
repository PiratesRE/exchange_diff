using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tracking
{
	[Serializable]
	public abstract class MessageTrackingConfigurableObject : ConfigurableObject
	{
		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public MessageTrackingConfigurableObject() : base(new SimpleProviderPropertyBag())
		{
		}
	}
}
