using System;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	[Flags]
	internal enum ConnectFlags
	{
		None = 0,
		UseAdminPrivilege = 1,
		UseDelegatedAuthPrivilege = 256,
		UseTransportPrivilege = 1024
	}
}
