using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	[Serializable]
	public enum WindowsAccountType
	{
		Normal,
		Guest,
		System,
		Anonymous
	}
}
