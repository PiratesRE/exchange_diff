using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.CsmSdk;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EacFlightProvider : FlightProvider
	{
		public EacFlightProvider()
		{
			if (!EacFlightProvider.initialized)
			{
				lock (EacFlightProvider.locker)
				{
					if (!EacFlightProvider.initialized)
					{
						EacFlightProvider.eacUrlToSectionMap = EacFlightUtility.EacGlobalSnapshot.Eac.GetObjectsOfType<IUrl>().ToDictionary((KeyValuePair<string, IUrl> pair) => pair.Value.Url, (KeyValuePair<string, IUrl> pair) => pair.Key, StringComparer.OrdinalIgnoreCase);
						EacFlightProvider.eacRewriteToSectionMap = EacFlightUtility.EacGlobalSnapshot.Eac.GetObjectsOfType<IUrlMapping>().ToDictionary((KeyValuePair<string, IUrlMapping> pair) => pair.Value.Url, (KeyValuePair<string, IUrlMapping> pair) => pair.Key, StringComparer.OrdinalIgnoreCase);
						EacFlightProvider.initialized = true;
					}
				}
			}
		}

		public static EacFlightProvider Instance
		{
			get
			{
				return (EacFlightProvider)FlightProvider.Instance;
			}
		}

		public override bool IsFeatureEnabled(string eacFeatureName)
		{
			VariantConfigurationSnapshot snapshot = eacFeatureName.StartsWith("Global.") ? EacFlightUtility.EacGlobalSnapshot : EacFlightUtility.GetSnapshotForCurrentUser();
			return snapshot.GetFeature(eacFeatureName).Enabled;
		}

		public override string[] GetAllEnabledFeatures()
		{
			return (from pair in this.GetAllFeatures()
			where pair.Value
			select pair.Key).ToArray<string>();
		}

		public Dictionary<string, bool> GetAllFeatures()
		{
			VariantConfigurationSnapshot snapshotForCurrentUser = EacFlightUtility.GetSnapshotForCurrentUser();
			return EacFlightUtility.GetAllEacRelatedFeatures(snapshotForCurrentUser);
		}

		public string GetRewriteUrl(string url)
		{
			string text = null;
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			url = url.Trim().Replace('\\', '/');
			if (EacFlightProvider.eacRewriteToSectionMap.ContainsKey(url))
			{
				VariantConfigurationSnapshot snapshotForCurrentUser = EacFlightUtility.GetSnapshotForCurrentUser();
				IUrlMapping @object = snapshotForCurrentUser.Eac.GetObject<IUrlMapping>(EacFlightProvider.eacRewriteToSectionMap[url]);
				text = @object.RemapTo;
				if (text != null)
				{
					text = text.Trim();
				}
				if (text == string.Empty)
				{
					text = null;
				}
			}
			return text;
		}

		public bool IsUrlEnabled(string url)
		{
			bool result = true;
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			url = url.Trim().Replace('\\', '/');
			if (EacFlightProvider.eacUrlToSectionMap.ContainsKey(url))
			{
				VariantConfigurationSnapshot snapshotForCurrentUser = EacFlightUtility.GetSnapshotForCurrentUser();
				IUrl @object = snapshotForCurrentUser.Eac.GetObject<IUrl>(EacFlightProvider.eacUrlToSectionMap[url]);
				result = @object.Enabled;
			}
			return result;
		}

		private static IDictionary<string, string> eacUrlToSectionMap;

		private static IDictionary<string, string> eacRewriteToSectionMap;

		private static volatile bool initialized = false;

		private static object locker = new object();
	}
}
