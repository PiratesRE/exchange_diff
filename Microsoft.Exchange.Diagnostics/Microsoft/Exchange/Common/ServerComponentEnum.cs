using System;

namespace Microsoft.Exchange.Common
{
	internal enum ServerComponentEnum
	{
		None,
		TestComponent,
		ServerWideOffline,
		ServerWideSettings,
		HubTransport,
		FrontendTransport,
		Monitoring,
		RecoveryActionsEnabled,
		AutoDiscoverProxy,
		ActiveSyncProxy,
		EcpProxy,
		EwsProxy,
		ImapProxy,
		OabProxy,
		OwaProxy,
		PopProxy,
		PushNotificationsProxy,
		RpsProxy,
		RwsProxy,
		RpcProxy,
		UMCallRouter,
		XropProxy,
		HttpProxyAvailabilityGroup,
		ForwardSyncDaemon,
		ProvisioningRps,
		MapiProxy,
		EdgeTransport,
		HighAvailability,
		SharedCache
	}
}
