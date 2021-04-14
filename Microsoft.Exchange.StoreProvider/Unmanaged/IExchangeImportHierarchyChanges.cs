using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("85a66cf0-d0e0-11cd-80fc-00aa004bba0b")]
	[ComImport]
	internal interface IExchangeImportHierarchyChanges
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		int Config(IStream pIStream, int ulFlags);

		[PreserveSig]
		int UpdateState(IStream pIStream);

		[PreserveSig]
		unsafe int ImportFolderChange(int cpvalChanges, SPropValue* ppvalChanges);

		[PreserveSig]
		unsafe int ImportFolderDeletion(int ulFlags, _SBinaryArray* lpSrcEntryList);
	}
}
