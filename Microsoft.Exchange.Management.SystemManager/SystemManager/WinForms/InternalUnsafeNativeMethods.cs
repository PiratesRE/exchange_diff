using System;
using System.Runtime.InteropServices;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal static class InternalUnsafeNativeMethods
	{
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, [In] [Out] ref InternalNativeMethods.HDITEM lparam);

		public const string User32 = "user32.dll";

		public const string Shell32Dll = "shell32.dll";

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class BROWSEINFO
		{
			public IntPtr hwndOwner;

			public IntPtr pidlRoot;

			public IntPtr pszDisplayName;

			public string lpszTitle;

			public int ulFlags;

			public InternalUnsafeNativeMethods.BrowseCallbackProc lpfn;

			public IntPtr lParam;

			public int iImage;
		}

		public class Shell32
		{
			[DllImport("shell32.dll")]
			public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);

			[DllImport("shell32.dll", CharSet = CharSet.Auto)]
			public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

			[DllImport("shell32.dll", CharSet = CharSet.Auto)]
			public static extern IntPtr SHBrowseForFolder([In] InternalUnsafeNativeMethods.BROWSEINFO lpbi);

			[DllImport("shell32.dll")]
			public static extern int SHGetMalloc([MarshalAs(UnmanagedType.LPArray)] [Out] UnsafeNativeMethods.IMalloc[] ppMalloc);
		}

		public delegate int BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData);
	}
}
