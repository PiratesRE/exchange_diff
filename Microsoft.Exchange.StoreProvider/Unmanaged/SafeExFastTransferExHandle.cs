using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExFastTransferExHandle : SafeExInterfaceHandle, IExFastTransferEx, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExFastTransferExHandle()
		{
		}

		internal SafeExFastTransferExHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExFastTransferExHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExFastTransferExHandle>(this);
		}

		public int Config(int ulFlags, int ulTransferMethod)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_Config(this.handle, ulFlags, ulTransferMethod);
		}

		public int TransferBuffer(int cbData, byte[] data, out int cbProcessed)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_TransferBuffer(this.handle, cbData, data, out cbProcessed);
		}

		public int IsInterfaceOk(int ulTransferMethod, ref Guid refiid, IntPtr lpPropTagArray, int ulFlags)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_IsInterfaceOk(this.handle, ulTransferMethod, ref refiid, lpPropTagArray, ulFlags);
		}

		public int GetObjectType(out Guid iid)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_GetObjectType(this.handle, out iid);
		}

		public int GetLastLowLevelError(out int lowLevelError)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_GetLastLowLevelError(this.handle, out lowLevelError);
		}

		public unsafe int GetServerVersion(int cbBufferSize, byte* pBuffer, out int cbBuffer)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_GetServerVersion(this.handle, cbBufferSize, pBuffer, out cbBuffer);
		}

		public unsafe int TellPartnerVersion(int cbBuffer, byte* pBuffer)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_TellPartnerVersion(this.handle, cbBuffer, pBuffer);
		}

		public int IsPrivateLogon()
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_IsPrivateLogon(this.handle);
		}

		public int StartMdbEventsImport()
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_StartMdbEventsImport(this.handle);
		}

		public int FinishMdbEventsImport(bool bSuccess)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_FinishMdbEventsImport(this.handle, bSuccess);
		}

		public int SetWatermarks(int cWMs, MDBEVENTWMRAW[] WMs)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_SetWatermarks(this.handle, cWMs, WMs);
		}

		public int AddMdbEvents(int cbRequest, byte[] pbRequest)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_AddMdbEvents(this.handle, cbRequest, pbRequest);
		}

		public int SetReceiveFolder(int cbEntryId, byte[] entryId, string messageClass)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_SetReceiveFolder(this.handle, cbEntryId, entryId, messageClass);
		}

		public unsafe int SetPerUser(ref MapiLtidNative pltid, Guid* guidReplica, int lib, byte[] pb, int cb, bool fLast)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_SetPerUser(this.handle, ref pltid, guidReplica, lib, pb, cb, fLast);
		}

		public unsafe int SetProps(int cValues, SPropValue* lpPropArray)
		{
			return SafeExFastTransferExHandle.IExchangeFastTransferEx_SetProps(this.handle, cValues, lpPropArray);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_Config(IntPtr iExchangeFastTransferEx, int ulFlags, int ulTransferMethod);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_TransferBuffer(IntPtr iExchangeFastTransferEx, int cbData, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] byte[] data, out int cbProcessed);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_IsInterfaceOk(IntPtr iExchangeFastTransferEx, int ulTransferMethod, ref Guid refiid, IntPtr lpPropTagArray, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_GetObjectType(IntPtr iExchangeFastTransferEx, out Guid iid);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_GetLastLowLevelError(IntPtr iExchangeFastTransferEx, out int lowLevelError);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeFastTransferEx_GetServerVersion(IntPtr iExchangeFastTransferEx, int cbBufferSize, byte* pBuffer, out int cbBuffer);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeFastTransferEx_TellPartnerVersion(IntPtr iExchangeFastTransferEx, int cbBuffer, byte* pBuffer);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_IsPrivateLogon(IntPtr iExchangeFastTransferEx);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_StartMdbEventsImport(IntPtr iExchangeFastTransferEx);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_FinishMdbEventsImport(IntPtr iExchangeFastTransferEx, bool bSuccess);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_SetWatermarks(IntPtr iExchangeFastTransferEx, int cWMs, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] MDBEVENTWMRAW[] WMs);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_AddMdbEvents(IntPtr iExchangeFastTransferEx, [MarshalAs(UnmanagedType.U4)] [In] int cbRequest, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] byte[] pbRequest);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeFastTransferEx_SetReceiveFolder(IntPtr iExchangeFastTransferEx, int cbEntryId, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] byte[] entryId, [MarshalAs(UnmanagedType.LPStr)] [In] string messageClass);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeFastTransferEx_SetPerUser(IntPtr iExchangeFastTransferEx, ref MapiLtidNative pltid, Guid* guidReplica, int lib, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] [In] byte[] pb, int cb, bool fLast);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeFastTransferEx_SetProps(IntPtr iExchangeFastTransferEx, int cValues, SPropValue* lpPropArray);
	}
}
