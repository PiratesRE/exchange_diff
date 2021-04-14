using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum FastTransferCopyFlag : uint
	{
		None = 0U,
		CopyMailboxPerUserData = 8U,
		CopyFolderPerUserData = 16U,
		MoveUser = 128U,
		CopySubfolders = 16U,
		SendEntryId = 32U,
		Transport = 256U,
		RecoverMode = 512U,
		ForceUnicode = 1024U,
		FastTrasferStream = 2048U,
		BestBody = 8192U,
		Unicode = 2147483648U
	}
}
