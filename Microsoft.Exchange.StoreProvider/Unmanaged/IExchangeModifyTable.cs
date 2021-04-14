using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("2d734cb0-53fd-101b-b19d-08002b3056e3")]
	[ComImport]
	internal interface IExchangeModifyTable
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		int GetTable(int ulFlags, out IMAPITable iMAPITable);

		[PreserveSig]
		unsafe int ModifyTable(int ulFlags, [In] _RowList* lpRowList);
	}
}
