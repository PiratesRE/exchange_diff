using System;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[Flags]
	public enum GroupMailboxConfigurationAction
	{
		None = 0,
		SetRegionalSettings = 1,
		CreateDefaultFolders = 2,
		SetInitialFolderPermissions = 4,
		SetAllFolderPermissions = 8,
		ConfigureCalendar = 16,
		SendWelcomeMessage = 32,
		GenerateGroupPhoto = 64
	}
}
