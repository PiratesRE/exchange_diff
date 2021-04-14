using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_ESEBACK_CALLBACKS_IMPL
	{
		internal NATIVE_ESEBACK_CALLBACKS_IMPL(ref NATIVE_ESEBACK_CALLBACKS native)
		{
			this.pfnPrepareInstance = Marshal.GetFunctionPointerForDelegate(native.pfnPrepareInstance);
			this.pfnDoneWithInstance = Marshal.GetFunctionPointerForDelegate(native.pfnDoneWithInstance);
			this.pfnGetDatabasesInfo = Marshal.GetFunctionPointerForDelegate(native.pfnGetDatabasesInfo);
			this.pfnFreeDatabasesInfo = Marshal.GetFunctionPointerForDelegate(native.pfnFreeDatabasesInfo);
			this.pfnIsSGReplicated = Marshal.GetFunctionPointerForDelegate(native.pfnIsSGReplicated);
			this.pfnFreeShipLogInfo = Marshal.GetFunctionPointerForDelegate(native.pfnFreeShipLogInfo);
			this.pfnServerAccessCheck = Marshal.GetFunctionPointerForDelegate(native.pfnServerAccessCheck);
			this.pfnTrace = Marshal.GetFunctionPointerForDelegate(native.pfnTrace);
		}

		public IntPtr pfnPrepareInstance;

		public IntPtr pfnDoneWithInstance;

		public IntPtr pfnGetDatabasesInfo;

		public IntPtr pfnFreeDatabasesInfo;

		public IntPtr pfnIsSGReplicated;

		public IntPtr pfnFreeShipLogInfo;

		public IntPtr pfnServerAccessCheck;

		public IntPtr pfnTrace;
	}
}
