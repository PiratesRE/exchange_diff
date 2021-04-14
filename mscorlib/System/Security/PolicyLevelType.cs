using System;
using System.Runtime.InteropServices;

namespace System.Security
{
	[ComVisible(true)]
	[Serializable]
	public enum PolicyLevelType
	{
		User,
		Machine,
		Enterprise,
		AppDomain
	}
}
