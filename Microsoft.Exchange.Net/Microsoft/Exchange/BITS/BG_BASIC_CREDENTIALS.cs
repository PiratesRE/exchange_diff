using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.BITS
{
	internal struct BG_BASIC_CREDENTIALS
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string UserName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string Password;
	}
}
