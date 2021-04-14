using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("0002030c-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IMAPIFolder : IMAPIContainer, IMAPIProp
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
		int GetContentsTable(int ulFlags, out IMAPITable iMAPITable);

		[PreserveSig]
		int GetHierarchyTable(int ulFlags, out IMAPITable iMAPITable);

		[PreserveSig]
		int OpenEntry(int cbEntryID, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryID, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid lpInterface, int ulFlags, out int lpulObjType, [MarshalAs(UnmanagedType.IUnknown)] out object obj);

		[PreserveSig]
		unsafe int SetSearchCriteria([In] SRestriction* lpRestriction, _SBinaryArray* lpContainerList, int ulSearchFlags);

		[PreserveSig]
		int GetSearchCriteria(int ulFlags, [PointerType("SRestriction*")] out SafeExLinkedMemoryHandle lpRestriction, [PointerType("_SBinaryArray*")] out SafeExLinkedMemoryHandle lpContainerList, out int ulSearchState);

		[PreserveSig]
		int CreateMessage(IntPtr lpInterface, int ulFlags, [MarshalAs(UnmanagedType.Interface)] out IMessage iMessage);

		[PreserveSig]
		unsafe int CopyMessages(_SBinaryArray* sbinArray, IntPtr lpInterface, IMAPIFolder destFolder, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[PreserveSig]
		unsafe int DeleteMessages(_SBinaryArray* sbinArray, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[PreserveSig]
		int CreateFolder(int ulFolderType, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszFolderName, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszFolderComment, IntPtr lpInterface, int ulFlags, [MarshalAs(UnmanagedType.Interface)] out IMAPIFolder iMAPIFolder);

		[PreserveSig]
		int CopyFolder(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, IntPtr lpInterface, IMAPIFolder destFolder, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwszNewFolderName, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[PreserveSig]
		int DeleteFolder(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[PreserveSig]
		unsafe int SetReadFlags(_SBinaryArray* sbinArray, IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);

		[PreserveSig]
		int GetMessageStatus(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, int ulFlags, out MessageStatus pulMessageStatus);

		[PreserveSig]
		int SetMessageStatus(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] lpEntryId, MessageStatus ulNewStatus, MessageStatus ulNewStatusMask, out MessageStatus pulOldStatus);

		[PreserveSig]
		int Slot1c();

		[PreserveSig]
		int EmptyFolder(IntPtr ulUIParam, IntPtr lpProgress, int ulFlags);
	}
}
