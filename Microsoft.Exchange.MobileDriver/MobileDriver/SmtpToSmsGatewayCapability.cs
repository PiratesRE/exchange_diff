using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ApplicationLogic.TextMessaging;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class SmtpToSmsGatewayCapability : MobileServiceCapability
	{
		internal SmtpToSmsGatewayCapability(PartType supportedPartType, int segmentPerPart, IList<CodingSupportability> codingSupportabilities, FeatureSupportability featureSupportabilities, TextMessagingHostingDataServicesServiceSmtpToSmsGateway gatewayParameters) : base(supportedPartType, segmentPerPart, codingSupportabilities, featureSupportabilities)
		{
			this.Parameters = gatewayParameters;
		}

		public TextMessagingHostingDataServicesServiceSmtpToSmsGateway Parameters { get; private set; }
	}
}
