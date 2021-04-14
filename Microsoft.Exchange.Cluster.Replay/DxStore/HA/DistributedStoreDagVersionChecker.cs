using System;
using System.Linq;
using Microsoft.Exchange.DxStore.HA.Events;
using Microsoft.Win32;

namespace Microsoft.Exchange.DxStore.HA
{
	internal class DistributedStoreDagVersionChecker
	{
		public DistributedStoreDagVersionChecker(DistributedStoreTopologyProvider topoProvider, string featureName, int minimumRequiredVersion)
		{
			this.topoProvider = topoProvider;
			this.featureName = featureName;
			this.minimumRequiredVersion = minimumRequiredVersion;
		}

		public Tuple<string, int> GetLowestServerVersionInDag(out Exception exception)
		{
			exception = null;
			if (this.topoProvider != null)
			{
				Tuple<string, int>[] dagServerVersionsSortedBestEffort = this.topoProvider.GetDagServerVersionsSortedBestEffort(out exception);
				if (dagServerVersionsSortedBestEffort != null)
				{
					return dagServerVersionsSortedBestEffort.FirstOrDefault<Tuple<string, int>>();
				}
			}
			return null;
		}

		public bool CheckVersionCompatibility()
		{
			if (this.isVersionRequirementSatisfied)
			{
				return true;
			}
			Tuple<string, int> tuple = null;
			bool flag = false;
			Exception ex = null;
			int? minimumVersionPersisted = this.GetMinimumVersionPersisted();
			this.isVersionRequirementSatisfied = (minimumVersionPersisted != null && minimumVersionPersisted.Value >= this.minimumRequiredVersion);
			if (!this.isVersionRequirementSatisfied)
			{
				flag = true;
				tuple = this.GetLowestServerVersionInDag(out ex);
				if (tuple != null)
				{
					this.isVersionRequirementSatisfied = (tuple.Item2 >= this.minimumRequiredVersion);
					if (this.isVersionRequirementSatisfied)
					{
						this.SetMinimumVersionPersisted(tuple.Item2);
					}
				}
			}
			string text = (minimumVersionPersisted != null) ? minimumVersionPersisted.Value.ToString() : "<unknown>";
			string text2 = "<none>";
			int num = -1;
			if (flag && tuple != null)
			{
				text2 = tuple.Item1;
				num = tuple.Item2;
			}
			if (this.isVersionRequirementSatisfied)
			{
				DxStoreHACrimsonEvents.StartupVersionCheckSatisfied.Log<string, string, int, int, string>(text, text2, num, this.minimumRequiredVersion, "<none>");
			}
			else
			{
				DxStoreHACrimsonEvents.StartupVersionCheckNotSatisfied.LogPeriodic<string, string, int, int, string>("StartupVersionCheck", TimeSpan.FromMinutes(15.0), text, text2, num, this.minimumRequiredVersion, (ex != null) ? ex.Message : "<none>");
			}
			return this.isVersionRequirementSatisfied;
		}

		private int? GetMinimumVersionPersisted()
		{
			object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\DistributedStore\\VersionCache", this.featureName, null);
			if (value != null && value is int)
			{
				return new int?((int)value);
			}
			return null;
		}

		private void SetMinimumVersionPersisted(int version)
		{
			Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\DistributedStore\\VersionCache", this.featureName, version);
		}

		internal const string VersionCacheKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\DistributedStore\\VersionCache";

		private readonly DistributedStoreTopologyProvider topoProvider;

		private readonly string featureName;

		private readonly int minimumRequiredVersion;

		private bool isVersionRequirementSatisfied;
	}
}
