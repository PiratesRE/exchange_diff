using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DeviceAccessStateReason
	{
		[LocDescription(DirectoryStrings.IDs.Unknown)]
		Unknown,
		[LocDescription(DirectoryStrings.IDs.Global)]
		Global,
		[LocDescription(DirectoryStrings.IDs.Individual)]
		Individual,
		[LocDescription(DirectoryStrings.IDs.DeviceRule)]
		DeviceRule,
		[LocDescription(DirectoryStrings.IDs.Upgrade)]
		Upgrade,
		[LocDescription(DirectoryStrings.IDs.Policy)]
		Policy,
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
		CommandFrequency,
		[LocDescription(DirectoryStrings.IDs.ExternalMdm)]
		ExternallyManaged = 51,
		[LocDescription(DirectoryStrings.IDs.ExternalCompliance)]
		ExternalCompliance,
		[LocDescription(DirectoryStrings.IDs.ExternalEnrollment)]
		ExternalEnrollment
	}
}
