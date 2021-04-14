using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExMapiTable : IExInterface, IDisposeTrackable, IDisposable
	{
		int Advise(AdviseFlags ulEventMask, IMAPIAdviseSink lpAdviseSink, out IntPtr piConnection);

		int AdviseEx(AdviseFlags ulEventMask, IntPtr iOnNotifyDelegate, ulong callbackId, out IntPtr piConnection);

		int Unadvise(IntPtr iConnection);

		int GetStatus(out int ulStatus, out int ulTableType);

		int SetColumns(PropTag[] lpPropTagArray, int ulFlags);

		int QueryColumns(int ulFlags, out PropTag[] propList);

		int GetRowCount(int flags, out int count);

		int SeekRow(uint bookmark, int crowSeek, ref int lpcrowSought);

		int QueryPosition(ref int ulRows, ref int ulNumerator, ref int ulDenominator);

		int FindRow(Restriction lpRes, uint bookmark, int ulFlags);

		int Restrict(Restriction lpRes, int ulFlags);

		int SortTable(SortOrder lpSort, int ulFlags);

		int QueryRows(int crows, int ulFlags, out PropValue[][] lpSRowset);

		int ExpandRow(long categoryId, int ulRowCount, int ulFlags, out PropValue[][] lpSRowset, out int ulMoreRows);

		int CollapseRow(long categoryId, int ulFlags, out int ulRowCount);

		int CreateBookmark(out uint bookmark);

		int FreeBookmark(uint bookmark);

		int SeekRowBookmark(uint bookmark, int crowSeek, bool fWantRowsSought, out bool fSoughtLess, ref int crowSought, out bool fPositionChanged);

		int GetCollapseState(byte[] pbInstanceKey, out byte[] pbCollapseState);

		int SetCollapseState(byte[] pbCollapseState, out uint bookmark);
	}
}
