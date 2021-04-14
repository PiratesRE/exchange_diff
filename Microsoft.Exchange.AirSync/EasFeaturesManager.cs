using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal class EasFeaturesManager : IEasFeaturesManager
	{
		public static IEasFeaturesManager Create(MiniRecipient recipient, Dictionary<EasFeature, bool> flightingOverrides)
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(recipient.GetContext(null), null, null);
			return new EasFeaturesManager(snapshot, flightingOverrides);
		}

		public EasFeaturesManager(VariantConfigurationSnapshot configurationSnapshot, Dictionary<EasFeature, bool> flightingOverrides)
		{
			this.configurationSnapshot = configurationSnapshot;
			this.flightingOverrides = flightingOverrides;
		}

		public VariantConfigurationSnapshot ConfigurationSnapshot
		{
			get
			{
				return this.configurationSnapshot;
			}
		}

		public VariantConfigurationSnapshot.ActiveSyncSettingsIni Settings
		{
			get
			{
				return this.ConfigurationSnapshot.ActiveSync;
			}
		}

		public bool IsEnabled(EasFeature featureId)
		{
			bool result;
			if (this.flightingOverrides.TryGetValue(featureId, out result))
			{
				return result;
			}
			IFeature @object = this.ConfigurationSnapshot.ActiveSync.GetObject<IFeature>(featureId.ToString());
			return @object != null && @object.Enabled;
		}

		public bool IsOverridden(EasFeature featureId)
		{
			return this.flightingOverrides.ContainsKey(featureId);
		}

		private readonly VariantConfigurationSnapshot configurationSnapshot;

		private Dictionary<EasFeature, bool> flightingOverrides;
	}
}
