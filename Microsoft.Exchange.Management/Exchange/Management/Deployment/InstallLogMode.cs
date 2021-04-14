using System;

namespace Microsoft.Exchange.Management.Deployment
{
	public enum InstallLogMode
	{
		None,
		FatalExit,
		Error,
		Warning = 4,
		User = 8,
		Info = 16,
		ResolveSource = 64,
		OutOfDiskSpace = 128,
		ActionStart = 256,
		ActionData = 512,
		CommonData = 2048,
		PropertyDump = 1024,
		Verbose = 4096,
		ExtraDebug = 8192,
		Progress = 1024,
		Initialize = 4096,
		Terminate = 8192,
		ShowDialog = 16384,
		All = 16383
	}
}
