using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Mapi.Unmanaged
{
	[Guid("00020306-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComImport]
	internal interface IMsgStore : IMAPIProp
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		int SaveChanges(int ulFlags);

		[PreserveSig]
		int GetProps([MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags, out int lpcValues, [PointerType("SPropValue*")] out SafeExLinkedMemoryHandle lppPropArray);

		[PreserveSig]
		int GetPropList(int ulFlags, out SafeExLinkedMemoryHandle propList);

		[PreserveSig]
		int OpenProperty(int propTag, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, int interfaceOptions, int ulFlags, [MarshalAs(UnmanagedType.IUnknown)] out object obj);

		[PreserveSig]
		unsafe int SetProps(int cValues, SPropValue* lpPropArray, [PointerType("SPropProblemArray*")] out SafeExLinkedMemoryHandle lppProblems);

		[PreserveSig]
		int DeleteProps([MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, [PointerType("SPropProblemArray*")] out SafeExLinkedMemoryHandle lppProblems);

		[PreserveSig]
		int CopyTo(int ciidExclude, [MarshalAs(UnmanagedType.LPArray)] [In] Guid[] rgiidExclude, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps, IntPtr ulUiParam, IntPtr lpProgress, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, IMAPIProp lpDestObj, int ulFlags, [PointerType("SPropProblemArray*")] out SafeExLinkedMemoryHandle lppProblems);

		[PreserveSig]
		int CopyProps([MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, IntPtr ulUIParam, IntPtr lpProgress, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, IMAPIProp lpDestObj, int ulFlags, [PointerType("SPropProblemArray*")] out SafeExLinkedMemoryHandle lppProblems);

		[PreserveSig]
		unsafe int GetNamesFromIDs(int** lppPropTagArray, Guid* lpGuid, int ulFlags, ref int cPropNames, [PointerType("SNameId**")] out SafeExLinkedMemoryHandle lppNames);

		[PreserveSig]
		unsafe int GetIDsFromNames(int cPropNames, SNameId** lppPropNames, int ulFlags, [PointerType("int*")] out SafeExLinkedMemoryHandle lpPropIDs);

		[PreserveSig]
		int Advise(int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, AdviseFlags ulEventMask, IMAPIAdviseSink lpAdviseSink, out IntPtr piConnection);

		[PreserveSig]
		int Unadvise(IntPtr iConnection);

		[PreserveSig]
		int Slot10();

		[PreserveSig]
		int OpenEntry(int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, int ulFlags, out int lpulObjType, [MarshalAs(UnmanagedType.IUnknown)] out object obj);

		[PreserveSig]
		int SetReceiveFolder([MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszMessageClass, int ulFlags, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID);

		[PreserveSig]
		int GetReceiveFolder([MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszMessageClass, int ulFlags, out int lpcbEntryId, out SafeExLinkedMemoryHandle lppEntryId, out SafeExLinkedMemoryHandle lppszExplicitClass);

		[PreserveSig]
		int Slot14();

		[PreserveSig]
		int StoreLogoff(ref int ulFlags);

		[PreserveSig]
		int AbortSubmit(int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, int ulFlags);

		[PreserveSig]
		int Slot17();

		[PreserveSig]
		int Slot18();

		[PreserveSig]
		int Slot19();

		[PreserveSig]
		int Slot1a();
	}
}
