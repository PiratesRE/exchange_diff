using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class ConnectedForestsInfoSingleton
	{
		public IExchangeExtensionSnapIn CurrentSnapIn { get; set; }

		public void UpdateInfo(IList<OrganizationSetting> forestInfos)
		{
			this.forestDisplayNameToInfoMap = new Dictionary<string, OrganizationSetting>();
			this.OtherForests = new List<string>();
			this.OtherOnPremiseForests = new List<string>();
			this.currentForest = null;
			foreach (OrganizationSetting organizationSetting in forestInfos)
			{
				this.forestDisplayNameToInfoMap.Add(organizationSetting.DisplayName, organizationSetting);
				if (organizationSetting.Key == this.CurrentSnapInKey)
				{
					this.currentForest = organizationSetting.DisplayName;
				}
				else
				{
					this.OtherForests.Add(organizationSetting.DisplayName);
					if (organizationSetting.Type == OrganizationType.RemoteOnPremise || organizationSetting.Type == OrganizationType.LocalOnPremise)
					{
						this.OtherOnPremiseForests.Add(organizationSetting.DisplayName);
					}
				}
			}
		}

		public string CurrentSnapInKey { get; set; }

		public string CurrentForest
		{
			get
			{
				return this.currentForest;
			}
		}

		public IList<string> OtherForests { get; private set; }

		public IList<string> OtherOnPremiseForests { get; private set; }

		public OrganizationSetting ForestInfoOf(string forest)
		{
			return this.forestDisplayNameToInfoMap[forest];
		}

		public OrganizationSetting CurrentForestSetting
		{
			get
			{
				return this.ForestInfoOf(this.currentForest);
			}
		}

		public static ConnectedForestsInfoSingleton GetInstance()
		{
			return ConnectedForestsInfoSingleton.instance;
		}

		private string currentForest;

		private Dictionary<string, OrganizationSetting> forestDisplayNameToInfoMap;

		private static ConnectedForestsInfoSingleton instance = new ConnectedForestsInfoSingleton();
	}
}
