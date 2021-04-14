using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public interface IDxStoreConfigProvider
	{
		void RefreshTopology(bool isForceRefresh = false);

		InstanceManagerConfig GetManagerConfig();

		string[] GetAllGroupNames();

		InstanceGroupConfig[] GetAllGroupConfigs();

		InstanceGroupConfig GetGroupConfig(string groupName, bool isFillDefaultValueIfNotExist = false);

		string[] GetGroupMemberNames(string groupName);

		InstanceGroupMemberConfig[] GetGroupMemberConfigs(string groupName);

		void DisableGroup(string groupName);

		void EnableGroup(string groupName);

		void RemoveGroupConfig(string groupName);

		string GetDefaultGroupName();

		void SetDefaultGroupName(string groupName);

		void RemoveDefaultGroupName();

		void SetRestartRequired(string groupName, bool isRestartRequired);
	}
}
