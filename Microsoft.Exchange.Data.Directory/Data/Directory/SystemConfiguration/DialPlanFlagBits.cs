using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum DialPlanFlagBits
	{
		[LocDescription(DirectoryStrings.IDs.None)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.TUIPromptEditingEnabled)]
		TUIPromptEditingEnabled = 1
	}
}
