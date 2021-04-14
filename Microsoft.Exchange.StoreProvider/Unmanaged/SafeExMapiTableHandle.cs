using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExMapiTableHandle : SafeExInterfaceHandle, IExMapiTable, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExMapiTableHandle()
		{
		}

		internal SafeExMapiTableHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExMapiTableHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMapiTableHandle>(this);
		}

		public int Advise(AdviseFlags ulEventMask, IMAPIAdviseSink lpAdviseSink, out IntPtr piConnection)
		{
			return SafeExMapiTableHandle.IMAPITable_Advise(this.handle, ulEventMask, lpAdviseSink, out piConnection);
		}

		public int AdviseEx(AdviseFlags ulEventMask, IntPtr iOnNotifyDelegate, ulong callbackId, out IntPtr piConnection)
		{
			return SafeExMapiTableHandle.IExRpcTable_AdviseEx(this.handle, ulEventMask, iOnNotifyDelegate, callbackId, out piConnection);
		}

		public int Unadvise(IntPtr iConnection)
		{
			return SafeExMapiTableHandle.IMAPITable_Unadvise(this.handle, iConnection);
		}

		public int GetStatus(out int ulStatus, out int ulTableType)
		{
			return SafeExMapiTableHandle.IMAPITable_GetStatus(this.handle, out ulStatus, out ulTableType);
		}

		public int SetColumns(PropTag[] lpPropTagArray, int ulFlags)
		{
			return SafeExMapiTableHandle.IMAPITable_SetColumns(this.handle, lpPropTagArray, ulFlags);
		}

		public int QueryColumns(int ulFlags, out PropTag[] propList)
		{
			propList = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiTableHandle.IMAPITable_QueryColumns(this.handle, ulFlags, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					propList = safeExLinkedMemoryHandle.ReadPropTagArray();
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int GetRowCount(int flags, out int count)
		{
			return SafeExMapiTableHandle.IMAPITable_GetRowCount(this.handle, flags, out count);
		}

		public int SeekRow(uint bookmark, int crowSeek, ref int lpcrowSought)
		{
			return SafeExMapiTableHandle.IMAPITable_SeekRow(this.handle, bookmark, crowSeek, ref lpcrowSought);
		}

		public int QueryPosition(ref int ulRows, ref int ulNumerator, ref int ulDenominator)
		{
			return SafeExMapiTableHandle.IMAPITable_QueryPosition(this.handle, ref ulRows, ref ulNumerator, ref ulDenominator);
		}

		public int FindRow(Restriction lpRes, uint bookmark, int ulFlags)
		{
			return this.InternalFindRow(lpRes, bookmark, ulFlags);
		}

		private unsafe int InternalFindRow(Restriction lpRes, uint bookmark, int ulFlags)
		{
			SRestriction* ptr = null;
			int bytesToMarshal = lpRes.GetBytesToMarshal();
			byte* ptr2 = stackalloc byte[(UIntPtr)bytesToMarshal];
			ptr = (SRestriction*)ptr2;
			ptr2 += (SRestriction.SizeOf + 7 & -8);
			lpRes.MarshalToNative(ptr, ref ptr2);
			return SafeExMapiTableHandle.IMAPITable_FindRow(this.handle, ptr, bookmark, ulFlags);
		}

		public int Restrict(Restriction lpRes, int ulFlags)
		{
			return this.InternalRestrict(lpRes, ulFlags);
		}

		private unsafe int InternalRestrict(Restriction lpRes, int ulFlags)
		{
			int bytesToMarshal = lpRes.GetBytesToMarshal();
			byte* ptr = stackalloc byte[(UIntPtr)bytesToMarshal];
			SRestriction* ptr2 = (SRestriction*)ptr;
			ptr += (SRestriction.SizeOf + 7 & -8);
			lpRes.MarshalToNative(ptr2, ref ptr);
			return SafeExMapiTableHandle.IMAPITable_Restrict(this.handle, ptr2, ulFlags);
		}

		public int SortTable(SortOrder lpSort, int ulFlags)
		{
			return this.InternalSortTable(lpSort, ulFlags);
		}

		private unsafe int InternalSortTable(SortOrder lpSort, int ulFlags)
		{
			int bytesToMarshal = lpSort.GetBytesToMarshal();
			byte* ptr = stackalloc byte[(UIntPtr)bytesToMarshal];
			SSortOrderSet* ptr2 = (SSortOrderSet*)ptr;
			lpSort.MarshalToNative(ptr2);
			return SafeExMapiTableHandle.IMAPITable_SortTable(this.handle, ptr2, ulFlags);
		}

		public int QueryRows(int crows, int ulFlags, out PropValue[][] lpSRowset)
		{
			lpSRowset = null;
			SafeExProwsHandle safeExProwsHandle = null;
			int result;
			try
			{
				int num = SafeExMapiTableHandle.IMAPITable_QueryRows(this.handle, crows, ulFlags, out safeExProwsHandle);
				if (num == 0)
				{
					lpSRowset = SRowSet.Unmarshal(safeExProwsHandle);
				}
				result = num;
			}
			finally
			{
				if (safeExProwsHandle != null)
				{
					safeExProwsHandle.Dispose();
				}
			}
			return result;
		}

		public int ExpandRow(long categoryId, int ulRowCount, int ulFlags, out PropValue[][] lpSRowset, out int ulMoreRows)
		{
			lpSRowset = null;
			SafeExProwsHandle safeExProwsHandle = null;
			int result;
			try
			{
				int num = SafeExMapiTableHandle.IMAPITable_ExpandRow(this.handle, categoryId, ulRowCount, ulFlags, out safeExProwsHandle, out ulMoreRows);
				if (num == 0)
				{
					lpSRowset = SRowSet.Unmarshal(safeExProwsHandle);
				}
				result = num;
			}
			finally
			{
				if (safeExProwsHandle != null)
				{
					safeExProwsHandle.Dispose();
				}
			}
			return result;
		}

		public int CollapseRow(long categoryId, int ulFlags, out int ulRowCount)
		{
			return SafeExMapiTableHandle.IMAPITable_CollapseRow(this.handle, categoryId, ulFlags, out ulRowCount);
		}

		public int CreateBookmark(out uint bookmark)
		{
			return SafeExMapiTableHandle.IMAPITable_CreateBookmark(this.handle, out bookmark);
		}

		public int FreeBookmark(uint bookmark)
		{
			return SafeExMapiTableHandle.IMAPITable_FreeBookmark(this.handle, bookmark);
		}

		public int SeekRowBookmark(uint bookmark, int crowSeek, bool fWantRowsSought, out bool fSoughtLess, ref int crowSought, out bool fPositionChanged)
		{
			return SafeExMapiTableHandle.IExRpcTable_SeekRowBookmark(this.handle, bookmark, crowSeek, fWantRowsSought, out fSoughtLess, ref crowSought, out fPositionChanged);
		}

		public int GetCollapseState(byte[] pbInstanceKey, out byte[] pbCollapseState)
		{
			pbCollapseState = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2;
				int num = SafeExMapiTableHandle.IMAPITable_GetCollapseState(this.handle, (pbInstanceKey != null) ? pbInstanceKey.Length : 0, pbInstanceKey, out num2, out safeExLinkedMemoryHandle);
				if (num == 0)
				{
					byte[] array = Array<byte>.Empty;
					if (num2 > 0 && safeExLinkedMemoryHandle != null)
					{
						array = new byte[num2];
						Marshal.Copy(safeExLinkedMemoryHandle.DangerousGetHandle(), array, 0, num2);
					}
					pbCollapseState = array;
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
			}
			return result;
		}

		public int SetCollapseState(byte[] pbCollapseState, out uint bookmark)
		{
			return SafeExMapiTableHandle.IMAPITable_SetCollapseState(this.handle, (pbCollapseState != null) ? pbCollapseState.Length : 0, pbCollapseState, out bookmark);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcTable_SeekRowBookmark(IntPtr iMAPITable, uint bookmark, int crowSeek, bool fWantRowsSought, out bool fSoughtLess, ref int crowSought, out bool fPositionChanged);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcTable_AdviseEx(IntPtr iMAPITable, AdviseFlags ulEventMask, IntPtr iOnNotifyDelegate, ulong callbackId, out IntPtr piConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_Advise(IntPtr iMAPITable, AdviseFlags ulEventMask, IMAPIAdviseSink lpAdviseSink, out IntPtr piConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_Unadvise(IntPtr iMAPITable, IntPtr iConnection);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_GetStatus(IntPtr iMAPITable, out int ulStatus, out int ulTableType);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_SetColumns(IntPtr iMAPITable, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_QueryColumns(IntPtr iMAPITable, int ulFlags, out SafeExLinkedMemoryHandle propList);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_GetRowCount(IntPtr iMAPITable, int flags, out int count);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_SeekRow(IntPtr iMAPITable, uint bookmark, int crowSeek, ref int lpcrowSought);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_QueryPosition(IntPtr iMAPITable, ref int ulRows, ref int ulNumerator, ref int ulDenominator);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPITable_FindRow(IntPtr iMAPITable, [In] SRestriction* lpRes, uint bookmark, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPITable_Restrict(IntPtr iMAPITable, [In] SRestriction* lpRes, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPITable_SortTable(IntPtr iMAPITable, [In] SSortOrderSet* lpSort, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_QueryRows(IntPtr iMAPITable, int crows, int ulFlags, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_ExpandRow(IntPtr iMAPITable, long categoryId, int ulRowCount, int ulFlags, out SafeExProwsHandle lpSRowset, out int ulMoreRows);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_CollapseRow(IntPtr iMAPITable, long categoryId, int ulFlags, out int ulRowCount);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_CreateBookmark(IntPtr iMAPITable, out uint bookmark);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_FreeBookmark(IntPtr iMAPITable, uint bookmark);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_GetCollapseState(IntPtr iMAPITable, int cbInstanceKey, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbInstanceKey, out int cbCollapseState, out SafeExLinkedMemoryHandle pbCollapseState);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPITable_SetCollapseState(IntPtr iMAPITable, int cbCollapseState, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbCollapseState, out uint bookmark);
	}
}
