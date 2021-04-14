using System;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	internal enum TransportSyncNotificationEvent
	{
		RegistryAccessDenied,
		DeltaSyncPartnerAuthenticationFailed,
		DeltaSyncServiceEndpointsLoadFailed,
		DeltaSyncEndpointUnreachable
	}
}
