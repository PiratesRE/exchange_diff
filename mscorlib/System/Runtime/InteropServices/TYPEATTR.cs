using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.TYPEATTR instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct TYPEATTR
	{
		public const int MEMBER_ID_NIL = -1;

		public Guid guid;

		public int lcid;

		public int dwReserved;

		public int memidConstructor;

		public int memidDestructor;

		public IntPtr lpstrSchema;

		public int cbSizeInstance;

		public TYPEKIND typekind;

		public short cFuncs;

		public short cVars;

		public short cImplTypes;

		public short cbSizeVft;

		public short cbAlignment;

		public TYPEFLAGS wTypeFlags;

		public short wMajorVerNum;

		public short wMinorVerNum;

		public TYPEDESC tdescAlias;

		public IDLDESC idldescType;
	}
}
