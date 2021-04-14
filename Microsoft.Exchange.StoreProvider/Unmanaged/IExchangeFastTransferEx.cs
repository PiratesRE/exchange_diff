using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("1AD3079C-5325-4b68-A57E-E8FF2BD58E53")]
	[ComImport]
	internal interface IExchangeFastTransferEx
	{
		[PreserveSig]
		int Config(int ulFlags, int ulTransferMethod);

		[PreserveSig]
		int TransferBuffer(int cbData, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] byte[] data, out int cbProcessed);

		[PreserveSig]
		bool IsInterfaceOk(int ulTransferMethod, ref Guid refiid, IntPtr lpPropTagArray, int ulFlags);

		[PreserveSig]
		int GetObjectType(out Guid iid);

		[PreserveSig]
		int GetLastLowLevelError(out int lowLevelError);

		[PreserveSig]
		unsafe int GetServerVersion(int cbBufferSize, byte* pBuffer, out int cbBuffer);

		[PreserveSig]
		unsafe int TellPartnerVersion(int cbBuffer, byte* pBuffer);

		[PreserveSig]
		bool IsPrivateLogon();

		[PreserveSig]
		int StartMdbEventsImport();

		[PreserveSig]
		int FinishMdbEventsImport(bool bSuccess);

		[PreserveSig]
		int SetWatermarks(int cWMs, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] MDBEVENTWMRAW[] WMs);

		[PreserveSig]
		int AddMdbEvents([MarshalAs(UnmanagedType.U4)] [In] int cbRequest, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] byte[] pbRequest);

		[PreserveSig]
		int SetReceiveFolder(int cbEntryId, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] byte[] entryId, [MarshalAs(UnmanagedType.LPStr)] [In] string messageClass);

		[PreserveSig]
		unsafe int SetPerUser(ref MapiLtidNative pltid, Guid* guidReplica, int lib, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] [In] byte[] pb, int cb, bool fLast);

		[PreserveSig]
		unsafe int SetProps(int cValues, SPropValue* lpPropArray);
	}
}
