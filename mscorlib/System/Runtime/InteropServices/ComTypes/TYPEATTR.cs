using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct TYPEATTR
	{
		[__DynamicallyInvokable]
		public const int MEMBER_ID_NIL = -1;

		[__DynamicallyInvokable]
		public Guid guid;

		[__DynamicallyInvokable]
		public int lcid;

		[__DynamicallyInvokable]
		public int dwReserved;

		[__DynamicallyInvokable]
		public int memidConstructor;

		[__DynamicallyInvokable]
		public int memidDestructor;

		public IntPtr lpstrSchema;

		[__DynamicallyInvokable]
		public int cbSizeInstance;

		[__DynamicallyInvokable]
		public TYPEKIND typekind;

		[__DynamicallyInvokable]
		public short cFuncs;

		[__DynamicallyInvokable]
		public short cVars;

		[__DynamicallyInvokable]
		public short cImplTypes;

		[__DynamicallyInvokable]
		public short cbSizeVft;

		[__DynamicallyInvokable]
		public short cbAlignment;

		[__DynamicallyInvokable]
		public TYPEFLAGS wTypeFlags;

		[__DynamicallyInvokable]
		public short wMajorVerNum;

		[__DynamicallyInvokable]
		public short wMinorVerNum;

		[__DynamicallyInvokable]
		public TYPEDESC tdescAlias;

		[__DynamicallyInvokable]
		public IDLDESC idldescType;
	}
}
