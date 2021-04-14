using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[SecurityCritical(SecurityCriticalScope.Everything)]
	[StructLayout(LayoutKind.Sequential)]
	internal class ActivationServerInfo
	{
		public uint Version;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string PubKey = string.Empty;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string Url = string.Empty;
	}
}
