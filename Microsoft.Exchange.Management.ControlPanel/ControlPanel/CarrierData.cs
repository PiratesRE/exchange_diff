using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public sealed class CarrierData
	{
		public string ID { get; set; }

		public string Name
		{
			get
			{
				return SmsServiceProviders.GetLocalizedName(this.LocalizedNames);
			}
		}

		public bool HasSmtpGateway { get; set; }

		public UnifiedMessagingInfo UnifiedMessagingInfo { get; set; }

		public Dictionary<string, string> LocalizedNames { get; set; }
	}
}
