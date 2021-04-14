using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExModifyTableHandle : SafeExInterfaceHandle, IExModifyTable, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExModifyTableHandle()
		{
		}

		internal SafeExModifyTableHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExModifyTableHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExModifyTableHandle>(this);
		}

		public int GetTable(int ulFlags, out IExMapiTable iMAPITable)
		{
			SafeExMapiTableHandle safeExMapiTableHandle = null;
			int result = SafeExModifyTableHandle.IExchangeModifyTable_GetTable(this.handle, ulFlags, out safeExMapiTableHandle);
			iMAPITable = safeExMapiTableHandle;
			return result;
		}

		public int ModifyTable(int ulFlags, ICollection<RowEntry> lpRowList)
		{
			return this.InternalModifyTable(ulFlags, lpRowList);
		}

		private unsafe int InternalModifyTable(int ulFlags, ICollection<RowEntry> lpRowList)
		{
			int num = (_RowList.SizeOf + 7 & -8) + (_RowEntry.SizeOf * lpRowList.Count + 7 & -8);
			foreach (RowEntry rowEntry in lpRowList)
			{
				num += (SPropValue.SizeOf * rowEntry.Values.Count + 7 & -8);
				foreach (PropValue propValue in rowEntry.Values)
				{
					num += propValue.GetBytesToMarshal();
				}
			}
			fixed (byte* ptr = new byte[num])
			{
				_RowList* ptr2 = (_RowList*)ptr;
				_RowEntry* ptr3 = &ptr2->aEntries;
				byte* ptr4 = ptr + (_RowList.SizeOf + 7 & -8) + (_RowEntry.SizeOf * lpRowList.Count + 7 & -8);
				ptr2->cEntries = lpRowList.Count;
				foreach (RowEntry rowEntry2 in lpRowList)
				{
					SPropValue* ptr5 = (SPropValue*)ptr4;
					ptr4 += (SPropValue.SizeOf * rowEntry2.Values.Count + 7 & -8);
					ptr3->ulRowFlags = (int)rowEntry2.RowFlags;
					ptr3->cValues = rowEntry2.Values.Count;
					ptr3->rgPropVals = ptr5;
					foreach (PropValue propValue2 in rowEntry2.Values)
					{
						propValue2.MarshalToNative(ptr5, ref ptr4);
						ptr5++;
					}
					ptr3++;
				}
				return SafeExModifyTableHandle.IExchangeModifyTable_ModifyTable(this.handle, ulFlags, (_RowList*)ptr);
			}
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeModifyTable_GetTable(IntPtr iExchangeModifyTable, int ulFlags, out SafeExMapiTableHandle iMAPITable);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeModifyTable_ModifyTable(IntPtr iExchangeModifyTable, int ulFlags, [In] _RowList* lpRowList);
	}
}
