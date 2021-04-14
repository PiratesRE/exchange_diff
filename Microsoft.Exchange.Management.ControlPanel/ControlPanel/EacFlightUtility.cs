using System;
using System.Collections.Generic;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class EacFlightUtility
	{
		public static VariantConfigurationSnapshot EacGlobalSnapshot
		{
			get
			{
				if (EacFlightUtility.eacGlobalSnapshot == null)
				{
					lock (EacFlightUtility.eacGlobalSnapshotLocker)
					{
						if (EacFlightUtility.eacGlobalSnapshot == null)
						{
							EacFlightUtility.eacGlobalSnapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
						}
					}
				}
				return EacFlightUtility.eacGlobalSnapshot;
			}
		}

		public static string[] FeaturePrefixs
		{
			get
			{
				return new string[]
				{
					"Global.",
					"Eac."
				};
			}
		}

		internal static IFeature GetFeature(this VariantConfigurationSnapshot snapshot, string eacSecion)
		{
			return EacFlightUtility.GetObjectForEac<IFeature>(snapshot, eacSecion);
		}

		private static T GetObjectForEac<T>(VariantConfigurationSnapshot snapshot, string eacSecion) where T : class, ISettings
		{
			if (eacSecion == null)
			{
				throw new ArgumentNullException("eacSection");
			}
			int num = eacSecion.IndexOf('.');
			if (num == -1 || num == eacSecion.Length - 1)
			{
				throw new ArgumentException(string.Format("Incorrect section name: {0}, the valid EAC feature name should be 'Eac.XXX', 'Global.XXX'", eacSecion));
			}
			string text = eacSecion.Substring(0, num + 1);
			string id = eacSecion.Substring(num + 1);
			string a;
			if ((a = text) != null)
			{
				if (a == "Global.")
				{
					return snapshot.Global.GetObject<T>(id);
				}
				if (a == "Eac.")
				{
					return snapshot.Eac.GetObject<T>(id);
				}
			}
			throw new ArgumentException(string.Format("Incorrect section name: {0}, the valid EAC feature name should be 'Eac.XXX', 'Global.XXX'", eacSecion));
		}

		public static VariantConfigurationSnapshot GetSnapshotForCurrentUser()
		{
			bool flag;
			return EacFlightUtility.GetSnapshotForCurrentUser(out flag);
		}

		internal static VariantConfigurationSnapshot GetSnapshotForCurrentUser(out bool isGlobal)
		{
			RbacPrincipal current = RbacPrincipal.GetCurrent(false);
			if (current != null && current.RbacConfiguration != null && current.RbacConfiguration.ConfigurationSettings != null && current.RbacConfiguration.ConfigurationSettings.VariantConfigurationSnapshot != null)
			{
				isGlobal = false;
				return current.RbacConfiguration.ConfigurationSettings.VariantConfigurationSnapshot;
			}
			isGlobal = true;
			return EacFlightUtility.EacGlobalSnapshot;
		}

		internal static Dictionary<string, bool> GetAllEacRelatedFeatures(VariantConfigurationSnapshot snapshot)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (KeyValuePair<string, IFeature> keyValuePair in snapshot.Global.GetObjectsOfType<IFeature>())
			{
				dictionary.Add("Global." + keyValuePair.Key, keyValuePair.Value.Enabled);
			}
			foreach (KeyValuePair<string, IFeature> keyValuePair2 in snapshot.Eac.GetObjectsOfType<IFeature>())
			{
				dictionary.Add("Eac." + keyValuePair2.Key, keyValuePair2.Value.Enabled);
			}
			return dictionary;
		}

		public const string GlobalSettingsPrefix = "Global.";

		public const string EacSettingsPrefix = "Eac.";

		public const char FeatureNameSeperator = '.';

		private static volatile VariantConfigurationSnapshot eacGlobalSnapshot;

		private static object eacGlobalSnapshotLocker = new object();
	}
}
