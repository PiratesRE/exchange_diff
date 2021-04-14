using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Management.Metabase
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("70b51430-b6ca-11d0-b9b9-00a0c922e750")]
	[ComImport]
	internal interface IMSAdminBase
	{
		[PreserveSig]
		int AddKey(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path);

		[PreserveSig]
		int DeleteKey(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path);

		void DeleteChildKeys(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path);

		[PreserveSig]
		int EnumKeys(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, StringBuilder Buffer, int EnumKeyIndex);

		void CopyKey(SafeMetadataHandle source, [MarshalAs(UnmanagedType.LPWStr)] string SourcePath, SafeMetadataHandle dest, [MarshalAs(UnmanagedType.LPWStr)] string DestPath, [MarshalAs(UnmanagedType.Bool)] bool OverwriteFlag, [MarshalAs(UnmanagedType.Bool)] bool CopyFlag);

		void RenameKey(SafeMetadataHandle key, [MarshalAs(UnmanagedType.LPWStr)] string path, [MarshalAs(UnmanagedType.LPWStr)] string newName);

		[PreserveSig]
		int SetData(SafeMetadataHandle key, [MarshalAs(UnmanagedType.LPWStr)] string path, MetadataRecord data);

		[PreserveSig]
		int GetData(SafeMetadataHandle key, [MarshalAs(UnmanagedType.LPWStr)] string path, MetadataRecord data, out int RequiredDataLen);

		[PreserveSig]
		int DeleteData(SafeMetadataHandle key, [MarshalAs(UnmanagedType.LPWStr)] string path, int Identifier, MBDataType DataType);

		[PreserveSig]
		int EnumData(SafeMetadataHandle key, [MarshalAs(UnmanagedType.LPWStr)] string path, MetadataRecord data, int EnumDataIndex, out int RequiredDataLen);

		[PreserveSig]
		int GetAllData(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, MBAttributes Attributes, MBUserType UserType, MBDataType DataType, out int NumDataEntries, out int DataSetNumber, int BufferSize, SafeHGlobalHandle buffer, out int RequiredBufferSize);

		void DeleteAllData(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, MBUserType UserType, MBDataType DataType);

		[PreserveSig]
		int CopyData(SafeMetadataHandle sourcehandle, [MarshalAs(UnmanagedType.LPWStr)] string SourcePath, SafeMetadataHandle desthandle, [MarshalAs(UnmanagedType.LPWStr)] string DestPath, int Attributes, MBUserType UserType, MBDataType DataType, [MarshalAs(UnmanagedType.Bool)] bool CopyFlag);

		[PreserveSig]
		int GetDataPaths(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, MBIdentifier Identifier, MBDataType DataType, int BufferSize, [MarshalAs(UnmanagedType.LPArray)] char[] buffer, out int RequiredBufferSize);

		[PreserveSig]
		int OpenKey(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, [MarshalAs(UnmanagedType.U4)] MBKeyAccess AccessRequested, int TimeOut, out IntPtr _newHandle);

		[PreserveSig]
		int CloseKey(IntPtr _handle);

		void ChangePermissions(SafeMetadataHandle handle, int TimeOut, [MarshalAs(UnmanagedType.U4)] MBKeyAccess AccessRequested);

		int SaveData();

		[PreserveSig]
		void GetHandleInfo(SafeMetadataHandle handle, [Out] METADATA_HANDLE_INFO Info);

		[PreserveSig]
		void GetSystemChangeNumber(out int SystemChangeNumber);

		[PreserveSig]
		void GetDataSetNumber(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, out int DataSetNumber);

		[PreserveSig]
		void SetLastChangeTime(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, out System.Runtime.InteropServices.ComTypes.FILETIME LastChangeTime, [MarshalAs(UnmanagedType.Bool)] bool LocalTime);

		[PreserveSig]
		int GetLastChangeTime(SafeMetadataHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string Path, out System.Runtime.InteropServices.ComTypes.FILETIME LastChangeTime, [MarshalAs(UnmanagedType.Bool)] bool LocalTime);

		[PreserveSig]
		int KeyExchangePhase1();

		[PreserveSig]
		int KeyExchangePhase2();

		[PreserveSig]
		int Backup([MarshalAs(UnmanagedType.LPWStr)] string Location, int Version, int Flags);

		[PreserveSig]
		int Restore([MarshalAs(UnmanagedType.LPWStr)] string Location, int Version, int Flags);

		[PreserveSig]
		void EnumBackups([MarshalAs(UnmanagedType.LPWStr)] out string Location, out int Version, out System.Runtime.InteropServices.ComTypes.FILETIME BackupTime, int EnumIndex);

		[PreserveSig]
		void DeleteBackup([MarshalAs(UnmanagedType.LPWStr)] string Location, int Version);

		[Obsolete]
		[PreserveSig]
		int UnmarshalInterface(out IMSAdminBase interf);

		[Obsolete]
		[PreserveSig]
		int GetServerGuid();
	}
}
