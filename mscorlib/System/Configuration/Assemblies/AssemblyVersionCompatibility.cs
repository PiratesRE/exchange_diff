using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Assemblies
{
	[ComVisible(true)]
	[Serializable]
	public enum AssemblyVersionCompatibility
	{
		SameMachine = 1,
		SameProcess,
		SameDomain
	}
}
