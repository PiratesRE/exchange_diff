using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Data.Transport
{
	[ComVisible(false)]
	[SuppressUnmanagedCodeSecurity]
	internal static class DnsNativeMethods2
	{
		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DnsValidateName_W")]
		internal static extern int ValidateName([In] string name, int format);

		private const string DNSAPI = "dnsapi.dll";
	}
}
