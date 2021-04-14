﻿using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.IPersistFile instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Guid("0000010b-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface UCOMIPersistFile
	{
		void GetClassID(out Guid pClassID);

		[PreserveSig]
		int IsDirty();

		void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, int dwMode);

		void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);

		void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

		void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
	}
}
