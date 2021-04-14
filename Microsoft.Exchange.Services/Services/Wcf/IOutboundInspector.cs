using System;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal interface IOutboundInspector
	{
		void ProcessOutbound(ExchangeVersion requestVersion, Message reply);
	}
}
