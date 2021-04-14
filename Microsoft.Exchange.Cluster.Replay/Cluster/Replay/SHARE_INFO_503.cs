using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal struct SHARE_INFO_503
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		internal string Netname;

		internal uint Type;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string Remark;

		internal int Permissions;

		internal int Max_uses;

		internal int Current_uses;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string Path;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string Passwd;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string Servername;

		internal int Reserved;

		internal IntPtr Security_descriptor;
	}
}
