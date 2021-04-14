using System;

namespace Microsoft.Exchange.Diagnostics
{
	public enum ServiceLatencyMetadata
	{
		AuthModuleLatency,
		CallerADLatency,
		CallContextInitLatency,
		CheckAccessCoreLatency,
		CoreExecutionLatency,
		ExchangePrincipalLatency,
		HttpPipelineLatency,
		PreExecutionLatency,
		RecipientLookupLatency,
		RequestedUserADLatency,
		DetailedExchangePrincipalLatency,
		EPCacheGetAdSessionSettingsForOrg
	}
}
