using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public enum SpamDataBlobDataID : byte
	{
		IPList = 1,
		IPListNoReplication,
		OutboundAlertRule,
		OutboundConfig,
		CountryAddressMap,
		SpamRules,
		UriRules,
		IPReputationResult,
		OutboundIPAddress,
		UriRulesW14,
		UnifiedUriRules,
		UnifiedSpamRules,
		IPStats,
		IPReputationStats,
		MetaIPList,
		URLExclusionList,
		OutboundSpamExclusionList,
		SpamClassifiers,
		SpamEngineExecutionPackageHub,
		SpamEngineExecutionPackageFrontEnd
	}
}
