using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal enum RequestTarget
	{
		Unknown,
		Monitor,
		Owa,
		LocalClient,
		Akamai,
		Ecp,
		Rws,
		LiveIdConsumer,
		LiveIdBusiness,
		Hotmail,
		Adfs
	}
}
