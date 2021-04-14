using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum ProxyControlFlags
	{
		None = 0,
		DoNotApplyProxyThrottling = 1,
		DoNotCompress = 2,
		DoNotBuffer = 4,
		StripLargeRulesForDownlevelTargets = 8,
		DoNotAddIdentifyingCafeHeaders = 16,
		SkipWLMThrottling = 32,
		ResolveServerName = 64,
		UseTcp = 128,
		UseCertificateToAuthenticate = 256,
		Olc = 512
	}
}
