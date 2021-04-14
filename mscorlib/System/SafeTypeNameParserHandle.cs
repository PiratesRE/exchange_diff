using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace System
{
	[SecurityCritical]
	internal class SafeTypeNameParserHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void _ReleaseTypeNameParser(IntPtr pTypeNameParser);

		public SafeTypeNameParserHandle() : base(true)
		{
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			SafeTypeNameParserHandle._ReleaseTypeNameParser(this.handle);
			this.handle = IntPtr.Zero;
			return true;
		}
	}
}
