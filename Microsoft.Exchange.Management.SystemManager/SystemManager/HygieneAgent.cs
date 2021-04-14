using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public enum HygieneAgent
	{
		[LocDescription(Strings.IDs.ContentFilteringFeatureName)]
		ContentFilter,
		[LocDescription(Strings.IDs.IPBlockProvidersFeatureName)]
		IPBlockListProviders,
		[LocDescription(Strings.IDs.IPBlockListFeatureName)]
		IPBlockList,
		[LocDescription(Strings.IDs.IPAllowProvidersFeatureName)]
		IPAllowListProviders,
		[LocDescription(Strings.IDs.IPAllowListFeatureName)]
		IPAllowList,
		[LocDescription(Strings.IDs.RecipientFilteringFeatureName)]
		RecipientFilter,
		[LocDescription(Strings.IDs.SenderFilteringFeatureName)]
		SenderFilter,
		[LocDescription(Strings.IDs.SenderIdFeatureName)]
		SenderId,
		[LocDescription(Strings.IDs.SenderReputationFeatureName)]
		SenderReputation
	}
}
