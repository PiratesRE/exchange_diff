using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal abstract class InternalNativeMethods
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct HDITEM
		{
			public uint mask;

			public int cxy;

			public string pszText;

			public IntPtr hbm;

			public int cchTextMax;

			public int fmt;

			public IntPtr lParam;

			public int iImage;

			public int iOrder;

			public uint type;

			public IntPtr pvFilter;
		}
	}
}
