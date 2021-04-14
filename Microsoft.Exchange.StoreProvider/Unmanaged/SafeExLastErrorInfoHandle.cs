using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExLastErrorInfoHandle : SafeExInterfaceHandle, IExLastErrorInfo, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExLastErrorInfoHandle()
		{
		}

		internal SafeExLastErrorInfoHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExLastErrorInfoHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExLastErrorInfoHandle>(this);
		}

		public int GetLastError(int hResult, out int lpMapiError)
		{
			lpMapiError = 0;
			SafeExLinkedMemoryHandle safeExLinkedMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExLastErrorInfoHandle.ILastErrorInfo_GetLastError(this.handle, hResult, out safeExLinkedMemoryHandle);
				if (num == 0 && !safeExLinkedMemoryHandle.IsInvalid)
				{
					lpMapiError = Marshal.ReadInt32(safeExLinkedMemoryHandle.DangerousGetHandle(), MAPIERROR.LowLevelErrorOffset);
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

		public int GetExtendedErrorInfo(out DiagnosticContext pExtendedErrorInfo)
		{
			return this.InternalGetExtendedErrorInfo(out pExtendedErrorInfo);
		}

		private unsafe int InternalGetExtendedErrorInfo(out DiagnosticContext pExtendedErrorInfo)
		{
			pExtendedErrorInfo = null;
			SafeExMemoryHandle safeExMemoryHandle = null;
			int result;
			try
			{
				int num = SafeExLastErrorInfoHandle.ILastErrorInfo_GetExtendedErrorInfo(this.handle, out safeExMemoryHandle);
				if (num == 0)
				{
					pExtendedErrorInfo = new DiagnosticContext((THREAD_DIAG_CONTEXT*)safeExMemoryHandle.DangerousGetHandle().ToPointer());
				}
				else
				{
					pExtendedErrorInfo = new DiagnosticContext(null);
				}
				result = num;
			}
			finally
			{
				if (safeExMemoryHandle != null)
				{
					safeExMemoryHandle.Dispose();
				}
			}
			return result;
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int ILastErrorInfo_GetLastError(IntPtr iLastErrorInfo, int hResult, out SafeExLinkedMemoryHandle lpMapiError);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int ILastErrorInfo_GetExtendedErrorInfo(IntPtr iLastErrorInfo, out SafeExMemoryHandle pExtendedErrorInfo);
	}
}
