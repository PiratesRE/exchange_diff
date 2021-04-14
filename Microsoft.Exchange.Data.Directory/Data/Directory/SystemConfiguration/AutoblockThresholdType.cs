using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum AutoblockThresholdType
	{
		[LocDescription(DirectoryStrings.IDs.UserAgentsChanges)]
		UserAgentsChanges,
		[LocDescription(DirectoryStrings.IDs.RecentCommands)]
		RecentCommands,
		[LocDescription(DirectoryStrings.IDs.Watsons)]
		Watsons,
		[LocDescription(DirectoryStrings.IDs.OutOfBudgets)]
		OutOfBudgets,
		[LocDescription(DirectoryStrings.IDs.SyncCommands)]
		SyncCommands,
		[LocDescription(DirectoryStrings.IDs.EnableNotificationEmail)]
		EnableNotificationEmail,
		[LocDescription(DirectoryStrings.IDs.EnableNotificationEmail)]
		CommandFrequency
	}
}
