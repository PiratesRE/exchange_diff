using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum HostProtectionResource
	{
		None = 0,
		Synchronization = 1,
		SharedState = 2,
		ExternalProcessMgmt = 4,
		SelfAffectingProcessMgmt = 8,
		ExternalThreading = 16,
		SelfAffectingThreading = 32,
		SecurityInfrastructure = 64,
		UI = 128,
		MayLeakOnAbort = 256,
		All = 511
	}
}
