using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum Rights : uint
	{
		None = 0U,
		ReadAny = 1U,
		Create = 2U,
		EditOwned = 8U,
		DeleteOwned = 16U,
		EditAny = 32U,
		DeleteAny = 64U,
		CreateSubfolder = 128U,
		Owner = 256U,
		Contact = 512U,
		Visible = 1024U,
		FreeBusySimple = 2048U,
		FreeBusyDetailed = 4096U
	}
}
