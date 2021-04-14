using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class PublicFolderMailboxAssistantInfo : PublicFolderMailboxMonitoringInfo
	{
		internal const string LastAssistantCycleLogConfigurationName = "PublicFolderLastAssistantCycleLog";

		internal const string AssistantInfoConfigurationName = "PublicFolderAssistantInfo";
	}
}
