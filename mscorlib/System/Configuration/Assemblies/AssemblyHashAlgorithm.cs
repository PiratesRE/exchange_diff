using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Assemblies
{
	[ComVisible(true)]
	[Serializable]
	public enum AssemblyHashAlgorithm
	{
		None,
		MD5 = 32771,
		SHA1,
		[ComVisible(false)]
		SHA256 = 32780,
		[ComVisible(false)]
		SHA384,
		[ComVisible(false)]
		SHA512
	}
}
