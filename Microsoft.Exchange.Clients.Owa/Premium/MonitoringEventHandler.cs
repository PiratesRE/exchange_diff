using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Monitoring")]
	internal sealed class MonitoringEventHandler : OwaEventHandlerBase
	{
		[OwaEvent("Ping")]
		[OwaEventVerb(OwaEventVerb.Get)]
		public void Ping()
		{
		}

		public const string EventNamespace = "Monitoring";
	}
}
