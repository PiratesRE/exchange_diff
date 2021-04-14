using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct NATIVE_ERRINFOBASIC
	{
		public const int HierarchySize = 8;

		public const int SourceFileLength = 64;

		public uint cbStruct;

		public JET_err errValue;

		public JET_ERRCAT errcatMostSpecific;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] rgCategoricalHierarchy;

		public uint lSourceLine;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string rgszSourceFile;
	}
}
