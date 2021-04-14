using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Flags]
	internal enum ServiceType
	{
		FileSystemDriver = 2,
		KernelDriver = 1,
		Win32OwnProcess = 16,
		Win32ShareProcess = 32
	}
}
