﻿using System;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("86ddd2d7-ad80-44f6-a12e-63698b52825d")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IWinRTClassActivator
	{
		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IInspectable)]
		object ActivateInstance([MarshalAs(UnmanagedType.HString)] string activatableClassId);

		[SecurityCritical]
		IntPtr GetActivationFactory([MarshalAs(UnmanagedType.HString)] string activatableClassId, ref Guid iid);
	}
}
