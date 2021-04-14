using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace System.StubHelpers
{
	[SecurityCritical]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	[SuppressUnmanagedCodeSecurity]
	internal static class StubHelpers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsQCall(IntPtr pMD);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InitDeclaringType(IntPtr pMD);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetNDirectTarget(IntPtr pMD);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetDelegateTarget(Delegate pThis, ref IntPtr pStubArg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DemandPermission(IntPtr pNMD);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetLastError();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ThrowInteropParamException(int resID, int paramIdx);

		[SecurityCritical]
		internal static IntPtr AddToCleanupList(ref CleanupWorkList pCleanupWorkList, SafeHandle handle)
		{
			if (pCleanupWorkList == null)
			{
				pCleanupWorkList = new CleanupWorkList();
			}
			CleanupWorkListElement cleanupWorkListElement = new CleanupWorkListElement(handle);
			pCleanupWorkList.Add(cleanupWorkListElement);
			return StubHelpers.SafeHandleAddRef(handle, ref cleanupWorkListElement.m_owned);
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal static void DestroyCleanupList(ref CleanupWorkList pCleanupWorkList)
		{
			if (pCleanupWorkList != null)
			{
				pCleanupWorkList.Destroy();
				pCleanupWorkList = null;
			}
		}

		internal static Exception GetHRExceptionObject(int hr)
		{
			Exception ex = StubHelpers.InternalGetHRExceptionObject(hr);
			ex.InternalPreserveStackTrace();
			return ex;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Exception InternalGetHRExceptionObject(int hr);

		internal static Exception GetCOMHRExceptionObject(int hr, IntPtr pCPCMD, object pThis)
		{
			Exception ex = StubHelpers.InternalGetCOMHRExceptionObject(hr, pCPCMD, pThis, false);
			ex.InternalPreserveStackTrace();
			return ex;
		}

		internal static Exception GetCOMHRExceptionObject_WinRT(int hr, IntPtr pCPCMD, object pThis)
		{
			Exception ex = StubHelpers.InternalGetCOMHRExceptionObject(hr, pCPCMD, pThis, true);
			ex.InternalPreserveStackTrace();
			return ex;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Exception InternalGetCOMHRExceptionObject(int hr, IntPtr pCPCMD, object pThis, bool fForWinRT);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr CreateCustomMarshalerHelper(IntPtr pMD, int paramToken, IntPtr hndManagedType);

		[SecurityCritical]
		internal static IntPtr SafeHandleAddRef(SafeHandle pHandle, ref bool success)
		{
			if (pHandle == null)
			{
				throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_SafeHandle"));
			}
			pHandle.DangerousAddRef(ref success);
			if (!success)
			{
				return IntPtr.Zero;
			}
			return pHandle.DangerousGetHandle();
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal static void SafeHandleRelease(SafeHandle pHandle)
		{
			if (pHandle == null)
			{
				throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_SafeHandle"));
			}
			try
			{
				pHandle.DangerousRelease();
			}
			catch (Exception ex)
			{
				Mda.ReportErrorSafeHandleRelease(ex);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetCOMIPFromRCW(object objSrc, IntPtr pCPCMD, out IntPtr ppTarget, out bool pfNeedsRelease);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetCOMIPFromRCW_WinRT(object objSrc, IntPtr pCPCMD, out IntPtr ppTarget);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetCOMIPFromRCW_WinRTSharedGeneric(object objSrc, IntPtr pCPCMD, out IntPtr ppTarget);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetCOMIPFromRCW_WinRTDelegate(object objSrc, IntPtr pCPCMD, out IntPtr ppTarget);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ShouldCallWinRTInterface(object objSrc, IntPtr pCPCMD);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Delegate GetTargetForAmbiguousVariantCall(object objSrc, IntPtr pMT, out bool fUseString);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StubRegisterRCW(object pThis);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StubUnregisterRCW(object pThis);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetDelegateInvokeMethod(Delegate pThis);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object GetWinRTFactoryObject(IntPtr pCPCMD);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetWinRTFactoryReturnValue(object pThis, IntPtr pCtorEntry);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetOuterInspectable(object pThis, IntPtr pCtorMD);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Exception TriggerExceptionSwallowedMDA(Exception ex, IntPtr pManagedTarget);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CheckCollectedDelegateMDA(IntPtr pEntryThunk);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr ProfilerBeginTransitionCallback(IntPtr pSecretParam, IntPtr pThread, object pThis);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ProfilerEndTransitionCallback(IntPtr pMD, IntPtr pThread);

		internal static void CheckStringLength(int length)
		{
			StubHelpers.CheckStringLength((uint)length);
		}

		internal static void CheckStringLength(uint length)
		{
			if (length > 2147483632U)
			{
				throw new MarshalDirectiveException(Environment.GetResourceString("Marshaler_StringTooLong"));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern int strlen(sbyte* ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DecimalCanonicalizeInternal(ref decimal dec);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void FmtClassUpdateNativeInternal(object obj, byte* pNative, ref CleanupWorkList pCleanupWorkList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void FmtClassUpdateCLRInternal(object obj, byte* pNative);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void LayoutDestroyNativeInternal(byte* pNative, IntPtr pMT);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object AllocateInternal(IntPtr typeHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void MarshalToUnmanagedVaListInternal(IntPtr va_list, uint vaListSize, IntPtr pArgIterator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void MarshalToManagedVaListInternal(IntPtr va_list, IntPtr pArgIterator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint CalcVaListSize(IntPtr va_list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ValidateObject(object obj, IntPtr pMD, object pThis);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LogPinnedArgument(IntPtr localDesc, IntPtr nativeArg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ValidateByref(IntPtr byref, IntPtr pMD, object pThis);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetStubContext();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetStubContextAddr();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void TriggerGCForMDA();
	}
}
