using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("E9972C72-4A7D-464c-9350-ADD5ABABF6D8")]
	[ComImport]
	internal interface IExRpcFolder
	{
		[PreserveSig]
		int IsContentAvailable([MarshalAs(UnmanagedType.Bool)] out bool isContentAvailable);

		[PreserveSig]
		int GetReplicaServers(out uint numberOfServers, out SafeExLinkedMemoryHandle servers);

		[PreserveSig]
		int SetMessageFlags(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, uint ulStatus, uint ulMask);

		[PreserveSig]
		unsafe int CopyMessagesEx(_SBinaryArray* sbinArray, IMAPIFolder destFolder, int ulFlags, int cValues, SPropValue* lpPropArray);

		[PreserveSig]
		unsafe int SetPropsConditional([In] SRestriction* lpRes, int cValues, SPropValue* lpPropArray, [PointerType("SPropProblemArray*")] out SafeExLinkedMemoryHandle lppProblems);

		[PreserveSig]
		unsafe int CopyMessagesEID(_SBinaryArray* sbinArray, IMAPIFolder destFolder, int ulFlags, int cValues, SPropValue* lpPropArray, [PointerType("_SBinaryArray*")] out SafeExLinkedMemoryHandle lppEntryIds, [PointerType("_SBinaryArray*")] out SafeExLinkedMemoryHandle lppChangeNumbers);

		[PreserveSig]
		int CreateFolderEx(int ulFolderType, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszFolderName, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszFolderComment, int cbEntryId, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] byte[] lpEntryId, IntPtr lpInterface, int ulFlags, [MarshalAs(UnmanagedType.Interface)] out IMAPIFolder iMAPIFolder);
	}
}
