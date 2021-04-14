using System;

namespace System.Runtime.CompilerServices
{
	public static class RuntimeFeature
	{
		public static bool IsSupported(string feature)
		{
			return feature == "PortablePdb" && !AppContextSwitches.IgnorePortablePDBsInStackTraces;
		}

		public const string PortablePdb = "PortablePdb";
	}
}
