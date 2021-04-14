using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct MAPIERROR
	{
		public string ErrorText(bool unicodeEncoded)
		{
			if (this.lpszError == IntPtr.Zero)
			{
				return null;
			}
			if (!unicodeEncoded)
			{
				return Marshal.PtrToStringAnsi(this.lpszError);
			}
			return Marshal.PtrToStringUni(this.lpszError);
		}

		public string Component(bool unicodeEncoded)
		{
			if (this.lpszComponent == IntPtr.Zero)
			{
				return null;
			}
			if (!unicodeEncoded)
			{
				return Marshal.PtrToStringAnsi(this.lpszComponent);
			}
			return Marshal.PtrToStringUni(this.lpszComponent);
		}

		public static readonly int LowLevelErrorOffset = (int)Marshal.OffsetOf(typeof(MAPIERROR), "ulLowLevelError");

		internal int ulVersion;

		private IntPtr lpszError;

		private IntPtr lpszComponent;

		internal int ulLowLevelError;

		internal int ulContext;
	}
}
