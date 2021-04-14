using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExMapiContainerHandle : SafeExMapiPropHandle, IExMapiContainer, IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExMapiContainerHandle()
		{
		}

		internal SafeExMapiContainerHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExMapiContainerHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMapiContainerHandle>(this);
		}

		public int GetContentsTable(int ulFlags, out IExMapiTable iMAPITable)
		{
			SafeExMapiTableHandle safeExMapiTableHandle = null;
			int result = SafeExMapiContainerHandle.IMAPIContainer_GetContentsTable(this.handle, ulFlags, out safeExMapiTableHandle);
			iMAPITable = safeExMapiTableHandle;
			return result;
		}

		public int GetHierarchyTable(int ulFlags, out IExMapiTable iMAPITable)
		{
			SafeExMapiTableHandle safeExMapiTableHandle = null;
			int result = SafeExMapiContainerHandle.IMAPIContainer_GetHierarchyTable(this.handle, ulFlags, out safeExMapiTableHandle);
			iMAPITable = safeExMapiTableHandle;
			return result;
		}

		public int OpenEntry(byte[] lpEntryID, Guid lpInterface, int ulFlags, out int lpulObjType, out IExInterface iObj)
		{
			SafeExInterfaceHandle safeExInterfaceHandle = null;
			int result = SafeExMapiContainerHandle.IMAPIContainer_OpenEntry(this.handle, (lpEntryID != null) ? lpEntryID.Length : 0, lpEntryID, lpInterface, ulFlags, out lpulObjType, out safeExInterfaceHandle);
			iObj = safeExInterfaceHandle;
			return result;
		}

		public int SetSearchCriteria(Restriction lpRestriction, byte[][] lpContainerList, int ulSearchFlags)
		{
			return this.InternalSetSearchCriteria(lpRestriction, lpContainerList, ulSearchFlags);
		}

		private unsafe int InternalSetSearchCriteria(Restriction lpRestriction, byte[][] lpContainerList, int ulSearchFlags)
		{
			bool flag = lpRestriction == null;
			bool flag2 = lpContainerList == null;
			int num = 1;
			int num2 = 1;
			if (!flag)
			{
				num = lpRestriction.GetBytesToMarshal();
			}
			SBinary[] array = null;
			if (lpContainerList != null)
			{
				array = new SBinary[lpContainerList.GetLength(0)];
				for (int i = 0; i < lpContainerList.GetLength(0); i++)
				{
					array[i] = new SBinary(lpContainerList[i]);
				}
				num2 = SBinaryArray.GetBytesToMarshal(array);
			}
			fixed (byte* ptr = new byte[num])
			{
				fixed (byte* ptr2 = new byte[num2])
				{
					if (!flag)
					{
						byte* ptr3 = ptr + (SRestriction.SizeOf + 7 & -8);
						lpRestriction.MarshalToNative((SRestriction*)ptr, ref ptr3);
					}
					if (!flag2)
					{
						SBinaryArray.MarshalToNative(ptr2, array);
					}
					return SafeExMapiContainerHandle.IMAPIContainer_SetSearchCriteria(this.handle, flag ? null : ((SRestriction*)ptr), flag2 ? null : ((_SBinaryArray*)ptr2), ulSearchFlags);
				}
			}
		}

		public int GetSearchCriteria(int ulFlags, out Restriction lpRestriction, out byte[][] lpContainerList, out int ulSearchState)
		{
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle2 = null;
			lpRestriction = null;
			lpContainerList = null;
			int result;
			try
			{
				int num = SafeExMapiContainerHandle.IMAPIContainer_GetSearchCriteria(this.handle, ulFlags, out safeExLinkedMemoryHandle, out safeExLinkedMemoryHandle2, out ulSearchState);
				if (num == 0)
				{
					lpContainerList = _SBinaryArray.Unmarshal(safeExLinkedMemoryHandle2);
					lpRestriction = Restriction.Unmarshal(safeExLinkedMemoryHandle);
				}
				result = num;
			}
			finally
			{
				if (safeExLinkedMemoryHandle != null)
				{
					safeExLinkedMemoryHandle.Dispose();
				}
				if (safeExLinkedMemoryHandle2 != null)
				{
					safeExLinkedMemoryHandle2.Dispose();
				}
			}
			return result;
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIContainer_GetContentsTable(IntPtr iMAPIContainer, int ulFlags, out SafeExMapiTableHandle iMAPITable);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIContainer_GetHierarchyTable(IntPtr iMAPIContainer, int ulFlags, out SafeExMapiTableHandle iMAPITable);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIContainer_OpenEntry(IntPtr iMAPIContainer, int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, int ulFlags, out int lpulObjType, out SafeExInterfaceHandle iObj);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPIContainer_SetSearchCriteria(IntPtr iMAPIContainer, [In] SRestriction* lpRestriction, _SBinaryArray* lpContainerList, int ulSearchFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIContainer_GetSearchCriteria(IntPtr iMAPIContainer, int ulFlags, out SafeExLinkedMemoryHandle lpRestriction, out SafeExLinkedMemoryHandle lpContainerList, out int ulSearchState);
	}
}
