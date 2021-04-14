using System;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal interface IInboundInspector
	{
		void ProcessInbound(ExchangeVersion requestVersion, Message request);
	}
}
