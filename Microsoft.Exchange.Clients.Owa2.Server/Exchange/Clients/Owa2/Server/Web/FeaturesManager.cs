using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class FeaturesManager : IFeaturesManager
	{
		public static FeaturesManager Create(VariantConfigurationSnapshot configurationSnapshot, IConfigurationContext context, Func<VariantConfigurationSnapshot, IFeaturesStateOverride> featuresStateOverrideFactory)
		{
			IFeaturesStateOverride featureStateOverride = featuresStateOverrideFactory(configurationSnapshot);
			Func<IFeature, bool> isEnabled = (IFeature f) => (featureStateOverride == null || featureStateOverride.IsFeatureEnabled(f.Name)) && f.Enabled;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (object obj in Enum.GetValues(typeof(Feature)))
			{
				if (context.IsFeatureEnabled((Feature)obj))
				{
					hashSet.Add(obj.ToString());
				}
			}
			foreach (KeyValuePair<string, IFeature> keyValuePair in configurationSnapshot.OwaClient.GetObjectsOfType<IFeature>())
			{
				if (isEnabled(keyValuePair.Value))
				{
					hashSet.Add(keyValuePair.Value.Name);
				}
			}
			foreach (KeyValuePair<string, IFeature> keyValuePair2 in configurationSnapshot.OwaClientServer.GetObjectsOfType<IFeature>())
			{
				if (isEnabled(keyValuePair2.Value))
				{
					hashSet.Add(keyValuePair2.Value.Name);
				}
			}
			FeaturesManager result;
			try
			{
				IDictionary<string, IFeature> objectsOfType = configurationSnapshot.OwaClient.GetObjectsOfType<IFeature>();
				HashSet<string> enabledClientOnlyFeature = new HashSet<string>(from f in objectsOfType.Values
				where isEnabled(f)
				select f.Name);
				IDictionary<string, IFeature> objectsOfType2 = configurationSnapshot.OwaServer.GetObjectsOfType<IFeature>();
				HashSet<string> enabledServerOnlyFeature = new HashSet<string>(from f in objectsOfType2.Values
				where isEnabled(f)
				select f.Name);
				IDictionary<string, IFeature> objectsOfType3 = configurationSnapshot.OwaClientServer.GetObjectsOfType<IFeature>();
				HashSet<string> enabledClientServerOnlyFeature = new HashSet<string>(from f in objectsOfType3.Values
				where isEnabled(f)
				select f.Name);
				result = new FeaturesManager(configurationSnapshot, hashSet, enabledClientOnlyFeature, enabledServerOnlyFeature, enabledClientServerOnlyFeature);
			}
			catch (KeyNotFoundException ex)
			{
				string message = string.Format("VariantConfigurationSnapshot could not find OWA component settings. Exception {0} {1}.", ex.GetType(), ex.Message);
				ExTraceGlobals.CoreTracer.TraceError(0L, message);
				throw new FlightConfigurationException(message);
			}
			return result;
		}

		public FeaturesManager(VariantConfigurationSnapshot configurationSnapshot, HashSet<string> clientFeatures) : this(configurationSnapshot, clientFeatures, new HashSet<string>(), new HashSet<string>(), new HashSet<string>())
		{
		}

		public FeaturesManager(VariantConfigurationSnapshot configurationSnapshot, HashSet<string> clientFeatures, HashSet<string> enabledClientOnlyFeature, HashSet<string> enabledServerOnlyFeature, HashSet<string> enabledClientServerOnlyFeature)
		{
			this.configurationSnapshot = configurationSnapshot;
			this.allEnabledClientFeatures = clientFeatures;
			this.enabledClientOnlyFeatures = enabledClientOnlyFeature;
			this.enabledServerOnlyFeatures = enabledServerOnlyFeature;
			this.enabledClientServerFeatures = enabledClientServerOnlyFeature;
		}

		public VariantConfigurationSnapshot ConfigurationSnapshot
		{
			get
			{
				return this.configurationSnapshot;
			}
		}

		public VariantConfigurationSnapshot.OwaClientServerSettingsIni ClientServerSettings
		{
			get
			{
				return this.ConfigurationSnapshot.OwaClientServer;
			}
		}

		public VariantConfigurationSnapshot.OwaClientSettingsIni ClientSettings
		{
			get
			{
				return this.ConfigurationSnapshot.OwaClient;
			}
		}

		public VariantConfigurationSnapshot.OwaServerSettingsIni ServerSettings
		{
			get
			{
				return this.ConfigurationSnapshot.OwaServer;
			}
		}

		public virtual string[] GetClientEnabledFeatures()
		{
			return this.allEnabledClientFeatures.ToArray<string>();
		}

		public HashSet<string> GetEnabledFlightedFeatures(FlightedFeatureScope scope)
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (scope.HasFlag(FlightedFeatureScope.Client))
			{
				hashSet.UnionWith(this.enabledClientOnlyFeatures);
			}
			if (scope.HasFlag(FlightedFeatureScope.Server))
			{
				hashSet.UnionWith(this.enabledServerOnlyFeatures);
			}
			if (scope.HasFlag(FlightedFeatureScope.ClientServer))
			{
				hashSet.UnionWith(this.enabledClientServerFeatures);
			}
			return hashSet;
		}

		public bool IsFeatureSupported(string featureId)
		{
			return this.allEnabledClientFeatures.Contains(featureId) || this.IsFlightedFeatureEnabled(featureId);
		}

		private bool IsFlightedFeatureEnabled(string featureId)
		{
			return this.enabledClientOnlyFeatures.Contains(featureId) || this.enabledServerOnlyFeatures.Contains(featureId) || this.enabledClientServerFeatures.Contains(featureId);
		}

		private readonly VariantConfigurationSnapshot configurationSnapshot;

		private readonly HashSet<string> allEnabledClientFeatures;

		private readonly HashSet<string> enabledClientOnlyFeatures;

		private readonly HashSet<string> enabledClientServerFeatures;

		private readonly HashSet<string> enabledServerOnlyFeatures;
	}
}
