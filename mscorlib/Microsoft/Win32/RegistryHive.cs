using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	[Serializable]
	public enum RegistryHive
	{
		ClassesRoot = -2147483648,
		CurrentUser,
		LocalMachine,
		Users,
		PerformanceData,
		CurrentConfig,
		DynData
	}
}
