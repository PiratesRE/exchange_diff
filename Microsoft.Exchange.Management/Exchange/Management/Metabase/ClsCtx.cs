using System;

namespace Microsoft.Exchange.Management.Metabase
{
	[Flags]
	internal enum ClsCtx
	{
		InprocServer = 1,
		InprocHandler = 2,
		LocalServer = 4,
		InprocServer16 = 8,
		RemoteServer = 16,
		InprocHandler16 = 32,
		Reserved1 = 64,
		Reserved2 = 128,
		Reserved3 = 256,
		Reserved4 = 512,
		NoCodeDownload = 1024,
		Reserved5 = 2048,
		NoCustomMarshal = 4096,
		EnableCodeDownload = 8192,
		NoFailureLog = 16384,
		DisableAAA = 32768,
		EnableAAA = 65536,
		FromDefaultContext = 131072,
		Activate32BitServer = 262144,
		Activate64BitServer = 524288
	}
}
