using System;

namespace Microsoft.Mapi
{
	internal enum CopyPropertiesFlags
	{
		None,
		Move,
		NoReplace,
		CopyMailboxPerUserData = 8,
		CopyFolderPerUserData = 16,
		StripLargeRulesForDownlevelTargets = 2048
	}
}
