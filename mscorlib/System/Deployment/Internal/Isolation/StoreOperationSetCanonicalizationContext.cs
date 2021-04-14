using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
	internal struct StoreOperationSetCanonicalizationContext
	{
		[SecurityCritical]
		public StoreOperationSetCanonicalizationContext(string Bases, string Exports)
		{
			this.Size = (uint)Marshal.SizeOf(typeof(StoreOperationSetCanonicalizationContext));
			this.Flags = StoreOperationSetCanonicalizationContext.OpFlags.Nothing;
			this.BaseAddressFilePath = Bases;
			this.ExportsFilePath = Exports;
		}

		public void Destroy()
		{
		}

		[MarshalAs(UnmanagedType.U4)]
		public uint Size;

		[MarshalAs(UnmanagedType.U4)]
		public StoreOperationSetCanonicalizationContext.OpFlags Flags;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string BaseAddressFilePath;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string ExportsFilePath;

		[Flags]
		public enum OpFlags
		{
			Nothing = 0
		}
	}
}
