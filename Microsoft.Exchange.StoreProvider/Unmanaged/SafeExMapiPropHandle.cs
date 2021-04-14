using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExMapiPropHandle : SafeExInterfaceHandle, IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExMapiPropHandle()
		{
		}

		internal SafeExMapiPropHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExMapiPropHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMapiPropHandle>(this);
		}

		public int SaveChanges(int ulFlags)
		{
			return SafeExMapiPropHandle.IMAPIProp_SaveChanges(this.handle, ulFlags);
		}

		public int GetProps(ICollection<PropTag> lpPropTags, int ulFlags, out PropValue[] lppPropArray)
		{
			return this.InternalGetProps(lpPropTags, ulFlags, out lppPropArray);
		}

		private unsafe int InternalGetProps(ICollection<PropTag> lpPropTags, int ulFlags, out PropValue[] lppPropArray)
		{
			lppPropArray = null;
			PropTag[] lpPropTagArray = PropTagHelper.SPropTagArray(lpPropTags);
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num2;
				int num = SafeExMapiPropHandle.IMAPIProp_GetProps(this.handle, lpPropTagArray, ulFlags, out num2, out safeExLinkedMemoryHandle);
				if (num >= 0)
				{
					lppPropArray = new PropValue[num2];
					SPropValue* ptr = (SPropValue*)safeExLinkedMemoryHandle.DangerousGetHandle().ToPointer();
					for (int i = 0; i < num2; i++)
					{
						lppPropArray[i] = new PropValue(ptr + i);
					}
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

		public int GetPropList(int ulFlags, out PropTag[] propList)
		{
			propList = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiPropHandle.IMAPIProp_GetPropList(this.handle, ulFlags, out safeExLinkedMemoryHandle);
				if (num == 0 && safeExLinkedMemoryHandle != null)
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

		public int OpenProperty(int propTag, Guid lpInterface, int interfaceOptions, int ulFlags, out IExInterface iObj)
		{
			iObj = null;
			SafeExInterfaceHandle safeExInterfaceHandle = null;
			int result = SafeExMapiPropHandle.IMAPIProp_OpenProperty(this.handle, propTag, lpInterface, interfaceOptions, ulFlags, out safeExInterfaceHandle);
			iObj = safeExInterfaceHandle;
			return result;
		}

		public int SetProps(ICollection<PropValue> lpPropArray, out PropProblem[] lppProblems)
		{
			return this.InternalSetProps(lpPropArray, out lppProblems);
		}

		private unsafe int InternalSetProps(ICollection<PropValue> lpPropArray, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			int num = 0;
			foreach (PropValue propValue in lpPropArray)
			{
				num += propValue.GetBytesToMarshal();
			}
			fixed (byte* ptr = new byte[num])
			{
				PropValue.MarshalToNative(lpPropArray, ptr);
				SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
				int result;
				try
				{
					int num2 = SafeExMapiPropHandle.IMAPIProp_SetProps(this.handle, lpPropArray.Count, (SPropValue*)ptr, out safeExLinkedMemoryHandle);
					if (!safeExLinkedMemoryHandle.IsInvalid)
					{
						lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
					}
					result = num2;
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
		}

		public int DeleteProps(ICollection<PropTag> lpPropTags, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			PropTag[] lpPropTagArray = PropTagHelper.SPropTagArray(lpPropTags);
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiPropHandle.IMAPIProp_DeleteProps(this.handle, lpPropTagArray, out safeExLinkedMemoryHandle);
				if (num == 0 && safeExLinkedMemoryHandle != null)
				{
					lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
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

		public int CopyTo(int ciidExclude, Guid[] rgiidExclude, PropTag[] lpExcludeProps, IntPtr ulUiParam, IntPtr lpProgress, Guid lpInterface, IntPtr iMAPIPropDest, int ulFlags, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiPropHandle.IMAPIProp_CopyTo(this.handle, ciidExclude, rgiidExclude, lpExcludeProps, ulUiParam, lpProgress, lpInterface, iMAPIPropDest, ulFlags, out safeExLinkedMemoryHandle);
				if (num == 0 && safeExLinkedMemoryHandle != null)
				{
					lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
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

		public int CopyTo_External(int ciidExclude, Guid[] rgiidExclude, PropTag[] lpExcludeProps, IntPtr ulUiParam, IntPtr lpProgress, Guid lpInterface, IMAPIProp iMAPIPropDest, int ulFlags, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiPropHandle.IMAPIProp_CopyTo_External(this.handle, ciidExclude, rgiidExclude, lpExcludeProps, ulUiParam, lpProgress, lpInterface, iMAPIPropDest, ulFlags, out safeExLinkedMemoryHandle);
				if (num == 0 && safeExLinkedMemoryHandle != null)
				{
					lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
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

		public int CopyProps(PropTag[] lpIncludeProps, IntPtr ulUIParam, IntPtr lpProgress, Guid lpInterface, IntPtr iMAPIPropDest, int ulFlags, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiPropHandle.IMAPIProp_CopyProps(this.handle, lpIncludeProps, ulUIParam, lpProgress, lpInterface, iMAPIPropDest, ulFlags, out safeExLinkedMemoryHandle);
				if (num == 0 && safeExLinkedMemoryHandle != null)
				{
					lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
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

		public int CopyProps_External(PropTag[] lpIncludeProps, IntPtr ulUIParam, IntPtr lpProgress, Guid lpInterface, IMAPIProp iMAPIPropDest, int ulFlags, out PropProblem[] lppProblems)
		{
			lppProblems = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExMapiPropHandle.IMAPIProp_CopyProps_External(this.handle, lpIncludeProps, ulUIParam, lpProgress, lpInterface, iMAPIPropDest, ulFlags, out safeExLinkedMemoryHandle);
				if (num == 0 && safeExLinkedMemoryHandle != null)
				{
					lppProblems = safeExLinkedMemoryHandle.ReadPropProblemArray();
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

		public int GetNamesFromIDs(ICollection<PropTag> lppPropTagArray, Guid guidPropSet, int ulFlags, out NamedProp[] lppNames)
		{
			return this.InternalGetNamesFromIds(lppPropTagArray, guidPropSet, ulFlags, out lppNames);
		}

		private unsafe int InternalGetNamesFromIds(ICollection<PropTag> lppPropTagArray, Guid guidPropSet, int ulFlags, out NamedProp[] lppNames)
		{
			lppNames = null;
			Guid* lpGuid = (!Guid.Empty.Equals(guidPropSet)) ? (&guidPropSet) : null;
			int** lppPropTagArray2 = null;
			if (lppPropTagArray != null)
			{
				int* ptr = stackalloc int[checked(unchecked((UIntPtr)(lppPropTagArray.Count + 1)) * 4)];
				int num = 0;
				ptr[(IntPtr)(num++) * 4] = lppPropTagArray.Count;
				foreach (PropTag propTag in lppPropTagArray)
				{
					ptr[(IntPtr)(num++) * 4] = (int)propTag;
				}
				lppPropTagArray2 = &ptr;
			}
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int num2 = 0;
			int result;
			try
			{
				int num3 = SafeExMapiPropHandle.IMAPIProp_GetNamesFromIDs(this.handle, lppPropTagArray2, lpGuid, ulFlags, ref num2, out safeExLinkedMemoryHandle);
				if (num3 == 0)
				{
					NamedProp[] array = new NamedProp[num2];
					safeExLinkedMemoryHandle.CopyTo(array, 0, num2);
					lppNames = array;
				}
				result = num3;
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

		public int GetIDsFromNames(ICollection<NamedProp> lppPropNames, int ulFlags, out PropTag[] lpPropIDs)
		{
			return this.InternalGetIDsFromNames(lppPropNames, ulFlags, out lpPropIDs);
		}

		private unsafe int InternalGetIDsFromNames(ICollection<NamedProp> lppPropNames, int ulFlags, out PropTag[] lpPropIDs)
		{
			lpPropIDs = null;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = 0;
				num = (Marshal.SizeOf(typeof(IntPtr)) * (lppPropNames.Count + 1) + 7 & -8);
				foreach (NamedProp namedProp in lppPropNames)
				{
					if (namedProp == null)
					{
						throw MapiExceptionHelper.ArgumentNullException("np[i]");
					}
					num += namedProp.GetBytesToMarshal();
				}
				try
				{
					fixed (byte* ptr = new byte[num])
					{
						SNameId** ptr2 = (SNameId**)ptr;
						byte* ptr3 = ptr + (Marshal.SizeOf(typeof(IntPtr)) * lppPropNames.Count + 7 & -8);
						byte* ptr4 = ptr3 + (SNameId.SizeOf * lppPropNames.Count + 7 & -8);
						SNameId* ptr5 = (SNameId*)ptr3;
						int num2 = 0;
						foreach (NamedProp namedProp2 in lppPropNames)
						{
							namedProp2.MarshalToNative(ptr5, ref ptr4);
							*(IntPtr*)(ptr2 + (IntPtr)(num2++) * (IntPtr)sizeof(SNameId*)) = ptr5;
							ptr5++;
						}
						int num3 = SafeExMapiPropHandle.IMAPIProp_GetIDsFromNames(this.handle, lppPropNames.Count, ptr2, ulFlags, out safeExLinkedMemoryHandle);
						if (num3 == 0)
						{
							lpPropIDs = safeExLinkedMemoryHandle.ReadPropTagArray();
						}
						result = num3;
					}
				}
				finally
				{
					byte* ptr = null;
				}
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

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_SaveChanges(IntPtr iMAPIProp, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_GetProps(IntPtr iMAPIProp, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags, out int lpcValues, out SafeExLinkedMemoryHandle lppPropArray);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_GetPropList(IntPtr iMAPIProp, int ulFlags, out SafeExLinkedMemoryHandle propList);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_OpenProperty(IntPtr iMAPIProp, int propTag, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, int interfaceOptions, int ulFlags, out SafeExInterfaceHandle iObj);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPIProp_SetProps(IntPtr iMAPIProp, int cValues, SPropValue* lpPropArray, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_DeleteProps(IntPtr iMAPIProp, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_CopyTo(IntPtr iMAPIProp, int ciidExclude, [MarshalAs(UnmanagedType.LPArray)] [In] Guid[] rgiidExclude, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps, IntPtr ulUiParam, IntPtr lpProgress, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, IntPtr iMAPIPropDest, int ulFlags, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_CopyTo_External(IntPtr iMAPIProp, int ciidExclude, [MarshalAs(UnmanagedType.LPArray)] [In] Guid[] rgiidExclude, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps, IntPtr ulUiParam, IntPtr lpProgress, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, IMAPIProp iMAPIPropDest, int ulFlags, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_CopyProps(IntPtr iMAPIProp, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, IntPtr ulUIParam, IntPtr lpProgress, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, IntPtr iMAPIPropDest, int ulFlags, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IMAPIProp_CopyProps_External(IntPtr iMAPIProp, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, IntPtr ulUIParam, IntPtr lpProgress, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, IMAPIProp iMAPIPropDest, int ulFlags, out SafeExLinkedMemoryHandle lppProblems);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPIProp_GetNamesFromIDs(IntPtr iMAPIProp, int** lppPropTagArray, Guid* lpGuid, int ulFlags, ref int cPropNames, out SafeExLinkedMemoryHandle lppNames);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IMAPIProp_GetIDsFromNames(IntPtr iMAPIProp, int cPropNames, SNameId** lppPropNames, int ulFlags, out SafeExLinkedMemoryHandle lpPropIDs);
	}
}
