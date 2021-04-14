using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum DelegateSaveState
	{
		None = 0,
		DelegateForwardingRule = 1,
		ADSendOnBehalf = 2,
		FolderPermissions = 4,
		FreeBusyDelegateInfo = 8,
		RestoreDelegateForwardingRule = 16,
		RestoreADSendOnBehalf = 32,
		RestoreFolderPermissions = 64,
		RestoreFreeBusyDelegateInfo = 128
	}
}
