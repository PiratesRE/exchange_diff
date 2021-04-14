using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class OABVariantConfigurationSettings
	{
		public static bool IsMultitenancyEnabled
		{
			get
			{
				return OABVariantConfigurationSettings.isMultitenancyEnabled.Member;
			}
		}

		public static bool IsSharedTemplateFilesEnabled
		{
			get
			{
				return OABVariantConfigurationSettings.isSharedTemplateFilesEnabled.Member;
			}
		}

		public static bool IsGenerateRequestedOABsOnlyEnabled
		{
			get
			{
				return OABVariantConfigurationSettings.isGenerateRequestedOABsOnlyEnabled.Member;
			}
		}

		public static bool IsLinkedOABGenMailboxesEnabled
		{
			get
			{
				return OABVariantConfigurationSettings.isLinkedOABGenMailboxesEnabled.Member;
			}
		}

		public static bool IsEnforceManifestVersionMatchEnabled
		{
			get
			{
				return OABVariantConfigurationSettings.isEnforceManifestVersionMatchEnabled.Member;
			}
		}

		public static bool OabHttpClientAccessRulesEnabled
		{
			get
			{
				return OABVariantConfigurationSettings.oabHttpClientAccessRulesEnabled.Member;
			}
		}

		public static bool IsSkipServiceTopologyDiscoveryEnabled
		{
			get
			{
				return OABVariantConfigurationSettings.skipServiceTopologyDiscovery.Member;
			}
		}

		private static LazyMember<bool> isMultitenancyEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled);

		private static LazyMember<bool> isSharedTemplateFilesEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.OAB.SharedTemplateFiles.Enabled);

		private static LazyMember<bool> isGenerateRequestedOABsOnlyEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.OAB.GenerateRequestedOABsOnly.Enabled);

		private static LazyMember<bool> isLinkedOABGenMailboxesEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.OAB.LinkedOABGenMailboxes.Enabled);

		private static LazyMember<bool> isEnforceManifestVersionMatchEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.OAB.EnforceManifestVersionMatch.Enabled);

		private static LazyMember<bool> oabHttpClientAccessRulesEnabled = new LazyMember<bool>(() => VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OAB.OabHttpClientAccessRulesEnabled.Enabled);

		private static LazyMember<bool> skipServiceTopologyDiscovery = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.OAB.SkipServiceTopologyDiscovery.Enabled);
	}
}
