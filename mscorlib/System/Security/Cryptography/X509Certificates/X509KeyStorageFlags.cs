using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.X509Certificates
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum X509KeyStorageFlags
	{
		DefaultKeySet = 0,
		UserKeySet = 1,
		MachineKeySet = 2,
		Exportable = 4,
		UserProtected = 8,
		PersistKeySet = 16,
		EphemeralKeySet = 32
	}
}
