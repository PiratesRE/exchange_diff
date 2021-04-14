using System;

namespace Microsoft.Exchange.Security
{
	internal enum ContextAttribute
	{
		Sizes,
		Names,
		Lifespan,
		DceInfo,
		StreamSizes,
		Authority = 6,
		SessionKey = 9,
		PackageInfo,
		NegotiationInfo = 12,
		UniqueBindings = 25,
		EndpointBindings,
		ClientSpecifiedTarget,
		RemoteCertificate = 83,
		LocalCertificate,
		RootStore,
		IssuerListInfoEx = 89,
		ConnectionInfo,
		EapKeyBlock
	}
}
