using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum SyncFlag : ushort
	{
		None = 0,
		Unicode = 1,
		NoDeletions = 2,
		NoSoftDeletions = 4,
		ReadState = 8,
		Associated = 16,
		Normal = 32,
		NoConflicts = 64,
		OnlySpecifiedProps = 128,
		NoForeignKeys = 256,
		LimitedIMessage = 512,
		CatchUp = 1024,
		Conversations = 2048,
		NewMessage = 2048,
		MessageSelective = 4096,
		BestBody = 8192,
		IgnoreSpecifiedOnAssociated = 16384,
		ProgressMode = 32768
	}
}
