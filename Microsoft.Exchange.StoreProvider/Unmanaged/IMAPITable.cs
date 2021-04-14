using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00020301-0000-0000-C000-000000000046")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComImport]
	internal interface IMAPITable
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		int Advise(AdviseFlags ulEventMask, IMAPIAdviseSink lpAdviseSink, out IntPtr piConnection);

		[PreserveSig]
		int Unadvise(IntPtr iConnection);

		[PreserveSig]
		int GetStatus(out int ulStatus, out int ulTableType);

		[PreserveSig]
		int SetColumns([MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags);

		[PreserveSig]
		int QueryColumns(int ulFlags, [PointerType("PropTag*")] out SafeExLinkedMemoryHandle propList);

		[PreserveSig]
		int GetRowCount(int flags, out int count);

		[PreserveSig]
		int SeekRow(uint bookmark, int crowSeek, ref int lpcrowSought);

		[PreserveSig]
		int Slot0b();

		[PreserveSig]
		int QueryPosition(ref int ulRows, ref int ulNumerator, ref int ulDenominator);

		[PreserveSig]
		unsafe int FindRow([In] SRestriction* lpRes, uint bookmark, int ulFlags);

		[PreserveSig]
		unsafe int Restrict([In] SRestriction* lpRes, int ulFlags);

		[PreserveSig]
		int CreateBookmark(out uint bookmark);

		[PreserveSig]
		int FreeBookmark(uint bookmark);

		[PreserveSig]
		unsafe int SortTable([In] SSortOrderSet* lpSort, int ulFlags);

		[PreserveSig]
		int Slot12();

		[PreserveSig]
		int QueryRows(int crows, int ulFlags, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int Slot14();

		[PreserveSig]
		int ExpandRow(long categoryId, int ulRowCount, int ulFlags, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset, out int ulMoreRows);

		[PreserveSig]
		int CollapseRow(long categoryId, int ulFlags, out int ulRowCount);

		[PreserveSig]
		int Slot17();

		[PreserveSig]
		int GetCollapseState(int cbInstanceKey, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbInstanceKey, out int cbCollapseState, out SafeExLinkedMemoryHandle pbCollapseState);

		[PreserveSig]
		int SetCollapseState(int cbCollapseState, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbCollapseState, out uint bookmark);
	}
}
