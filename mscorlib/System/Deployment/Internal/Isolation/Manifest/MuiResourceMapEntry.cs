﻿using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class MuiResourceMapEntry : IDisposable
	{
		~MuiResourceMapEntry()
		{
			this.Dispose(false);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		[SecuritySafeCritical]
		public void Dispose(bool fDisposing)
		{
			if (this.ResourceTypeIdInt != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(this.ResourceTypeIdInt);
				this.ResourceTypeIdInt = IntPtr.Zero;
			}
			if (this.ResourceTypeIdString != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(this.ResourceTypeIdString);
				this.ResourceTypeIdString = IntPtr.Zero;
			}
			if (fDisposing)
			{
				GC.SuppressFinalize(this);
			}
		}

		[MarshalAs(UnmanagedType.SysInt)]
		public IntPtr ResourceTypeIdInt;

		public uint ResourceTypeIdIntSize;

		[MarshalAs(UnmanagedType.SysInt)]
		public IntPtr ResourceTypeIdString;

		public uint ResourceTypeIdStringSize;
	}
}
