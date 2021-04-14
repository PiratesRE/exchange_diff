using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization.Formatters
{
	[SecurityCritical]
	[ComVisible(true)]
	public sealed class InternalRM
	{
		[Conditional("_LOGGING")]
		public static void InfoSoap(params object[] messages)
		{
		}

		public static bool SoapCheckEnabled()
		{
			return BCLDebug.CheckEnabled("SOAP");
		}
	}
}
