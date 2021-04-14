using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum ConnectionFlags
	{
		None = 0,
		UseAdminPrivilege = 1,
		UseDelegatedAuthPrivilege = 256,
		UseTransportPrivilege = 1024,
		UseReadOnlyPrivilege = 2048,
		UseReadWritePrivilege = 4096,
		IgnoreNoPublicFolders = 32768,
		RemoteSystemService = 4194304
	}
}
