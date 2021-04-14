﻿using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	internal struct IStore_BindingResult
	{
		[MarshalAs(UnmanagedType.U4)]
		public uint Flags;

		[MarshalAs(UnmanagedType.U4)]
		public uint Disposition;

		public IStore_BindingResult_BoundVersion Component;

		public Guid CacheCoherencyGuid;

		[MarshalAs(UnmanagedType.SysInt)]
		public IntPtr Reserved;
	}
}
