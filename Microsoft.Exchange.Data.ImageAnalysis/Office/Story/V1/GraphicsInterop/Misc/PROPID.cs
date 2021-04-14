using System;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Misc
{
	[Flags]
	internal enum PROPID : ushort
	{
		VT_EMPTY = 0,
		VT_NULL = 1,
		VT_I1 = 16,
		VT_UI1 = 17,
		VT_I2 = 2,
		VT_UI2 = 18,
		VT_I4 = 3,
		VT_UI4 = 19,
		VT_INT = 22,
		VT_UINT = 23,
		VT_I8 = 20,
		VT_UI8 = 21,
		VT_R4 = 4,
		VT_R8 = 5,
		VT_BOOL = 11,
		VT_ERROR = 10,
		VT_CY = 6,
		VT_DATE = 7,
		VT_FILETIME = 64,
		VT_CLSID = 72,
		VT_CF = 71,
		VT_BSTR = 8,
		VT_BSTR_BLOB = 4095,
		VT_BLOB = 65,
		VT_BLOBOBJECT = 70,
		VT_LPSTR = 30,
		VT_LPWSTR = 31,
		VT_UNKNOWN = 13,
		VT_DISPATCH = 9,
		VT_STREAM = 66,
		VT_STREAMED_OBJECT = 68,
		VT_STORAGE = 67,
		VT_STORED_OBJECT = 69,
		VT_VERSIONED_STREAM = 73,
		VT_DECIMAL = 14,
		VT_VECTOR = 4096,
		VT_ARRAY = 8192,
		VT_BYREF = 16384,
		VT_VARIANT = 12,
		VT_TYPEMASK = 4095
	}
}
