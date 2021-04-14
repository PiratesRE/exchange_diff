﻿using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Cluster.Shared
{
	[Guid("79427A2B-F895-40e0-BE79-B57DC82ED231")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IKernelTransaction
	{
		int GetHandle(out IntPtr pHandle);
	}
}
