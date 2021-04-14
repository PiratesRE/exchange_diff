using System;
using System.Runtime.InteropServices;

namespace System.IO.IsolatedStorage
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum IsolatedStorageScope
	{
		None = 0,
		User = 1,
		Domain = 2,
		Assembly = 4,
		Roaming = 8,
		Machine = 16,
		Application = 32
	}
}
