using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("DCBB456B-FBDA-4c0c-BCF2-90EEF6BDCC07")]
	[ComImport]
	internal interface IExRpcConnection
	{
		[PreserveSig]
		int OpenMsgStore(int ulFlags, long ullOpenFlags, [MarshalAs(UnmanagedType.LPStr)] string lpszMailboxDN, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbMailboxGuid, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbMdbGuid, IMsgStore lpMDBPrivate, out SafeExMemoryHandle lppszWrongServerDN, IntPtr hToken, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pSidUser, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pSidPrimaryGroup, [MarshalAs(UnmanagedType.LPStr)] string lpszUserDN, int ulLcidString, int ulLcidSort, int ulCpid, [MarshalAs(UnmanagedType.LPStr)] string lpszApplicationId, out IMsgStore iMsgStore);

		[PreserveSig]
		int SendAuxBuffer(int ulFlags, int cbAuxBuffer, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbAuxBuffer, int fForceSend);

		[PreserveSig]
		int FlushRPCBuffer([MarshalAs(UnmanagedType.Bool)] bool fForceSend);

		[PreserveSig]
		int GetServerVersion(out int pulVersionMajor, out int pulVersionMinor, out int pulBuildMajor, out int pulBuildMinor);

		[PreserveSig]
		int IsDead([MarshalAs(UnmanagedType.Bool)] out bool pfDead);

		[PreserveSig]
		int RpcSentToServer([MarshalAs(UnmanagedType.Bool)] out bool pfRpcSent);

		[PreserveSig]
		int IsMapiMT([MarshalAs(UnmanagedType.Bool)] out bool pfMapiMT);

		[PreserveSig]
		int IsConnectedToMapiServer([MarshalAs(UnmanagedType.Bool)] out bool pfConnectedToMapiServer);
	}
}
