using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal enum FailingComponent
	{
		Owa = 1,
		OwaUnknownRootCause,
		LiveIdConsumer,
		LiveIdBusiness,
		Mailbox,
		ActiveDirectory,
		Networking,
		Ecp,
		EcpDependency,
		MServ,
		Rws,
		DataMart,
		Akamai,
		Hotmail,
		Places,
		UnifiedMessaging,
		ActiveMonitoring,
		Adfs,
		E4e
	}
}
