using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SessionCapabilitiesFlags
	{
		None = 0,
		CanSend = 1,
		CanDeliver = 2,
		CanCreateDefaultFolders = 4,
		MustHideDefaultFolders = 8,
		CanHaveDelegateUsers = 16,
		CanHaveExternalUsers = 32,
		CanHaveRules = 64,
		CanHaveJunkEmailRule = 128,
		CanHaveMasterCategoryList = 256,
		CanHaveOof = 512,
		CanHaveUserConfigurationManager = 1024,
		MustCreateFolderHierarchy = 2048,
		CanHaveCulture = 4096,
		CanSetCalendarAPIProperties = 8192,
		ReadOnly = 16384,
		Default = 6135
	}
}
