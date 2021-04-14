using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("f75abfa0-d0e0-11cd-80fc-00aa004bba0b")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComImport]
	internal interface IExchangeImportContentsChanges
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		int Config(IStream pIStream, int ulFlags);

		[PreserveSig]
		int UpdateState(IStream pIStream);

		[PreserveSig]
		unsafe int ImportMessageChange(int cpvalChanges, SPropValue* ppvalChanges, int ulFlags, out IMessage message);

		[PreserveSig]
		unsafe int ImportMessageDeletion(int ulFlags, _SBinaryArray* lpSrcEntryList);

		[PreserveSig]
		unsafe int ImportPerUserReadStateChange(int cElements, _ReadState* lpReadState);

		[PreserveSig]
		int ImportMessageMove(int cbSourceKeySrcFolder, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbSourceKeySrcFolder, int cbSourceKeySrcMessage, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbSourceKeySrcMessage, int cbPCLMessage, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbPCLMessage, int cbSourceKeyDestMessage, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbSourceKeyDestMessage, int cbChangeNumDestMessage, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbChangeNumDestMessage);
	}
}
