﻿using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("285a8871-c84a-11d7-850f-005cd062464f")]
	[ComImport]
	internal interface ISectionWithStringKey
	{
		void Lookup([MarshalAs(UnmanagedType.LPWStr)] string wzStringKey, [MarshalAs(UnmanagedType.Interface)] out object ppUnknown);

		bool IsCaseInsensitive { get; }
	}
}
