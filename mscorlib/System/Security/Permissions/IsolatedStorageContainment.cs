using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public enum IsolatedStorageContainment
	{
		None,
		DomainIsolationByUser = 16,
		ApplicationIsolationByUser = 21,
		AssemblyIsolationByUser = 32,
		DomainIsolationByMachine = 48,
		AssemblyIsolationByMachine = 64,
		ApplicationIsolationByMachine = 69,
		DomainIsolationByRoamingUser = 80,
		AssemblyIsolationByRoamingUser = 96,
		ApplicationIsolationByRoamingUser = 101,
		AdministerIsolatedStorageByUser = 112,
		UnrestrictedIsolatedStorage = 240
	}
}
