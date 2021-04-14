using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.BITS
{
	internal struct BG_FILE_INFO
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string RemoteName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string LocalName;
	}
}
