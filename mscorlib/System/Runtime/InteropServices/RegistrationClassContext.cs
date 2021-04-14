using System;

namespace System.Runtime.InteropServices
{
	[Flags]
	public enum RegistrationClassContext
	{
		InProcessServer = 1,
		InProcessHandler = 2,
		LocalServer = 4,
		InProcessServer16 = 8,
		RemoteServer = 16,
		InProcessHandler16 = 32,
		Reserved1 = 64,
		Reserved2 = 128,
		Reserved3 = 256,
		Reserved4 = 512,
		NoCodeDownload = 1024,
		Reserved5 = 2048,
		NoCustomMarshal = 4096,
		EnableCodeDownload = 8192,
		NoFailureLog = 16384,
		DisableActivateAsActivator = 32768,
		EnableActivateAsActivator = 65536,
		FromDefaultContext = 131072
	}
}
