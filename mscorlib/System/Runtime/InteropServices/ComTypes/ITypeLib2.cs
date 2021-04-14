using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("00020411-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface ITypeLib2 : ITypeLib
	{
		[__DynamicallyInvokable]
		[PreserveSig]
		int GetTypeInfoCount();

		[__DynamicallyInvokable]
		void GetTypeInfo(int index, out ITypeInfo ppTI);

		[__DynamicallyInvokable]
		void GetTypeInfoType(int index, out TYPEKIND pTKind);

		[__DynamicallyInvokable]
		void GetTypeInfoOfGuid(ref Guid guid, out ITypeInfo ppTInfo);

		void GetLibAttr(out IntPtr ppTLibAttr);

		[__DynamicallyInvokable]
		void GetTypeComp(out ITypeComp ppTComp);

		[__DynamicallyInvokable]
		void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);

		[__DynamicallyInvokable]
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal);

		[__DynamicallyInvokable]
		void FindName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal, [MarshalAs(UnmanagedType.LPArray)] [Out] ITypeInfo[] ppTInfo, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] rgMemId, ref short pcFound);

		[PreserveSig]
		void ReleaseTLibAttr(IntPtr pTLibAttr);

		[__DynamicallyInvokable]
		void GetCustData(ref Guid guid, out object pVarVal);

		[LCIDConversion(1)]
		[__DynamicallyInvokable]
		void GetDocumentation2(int index, out string pbstrHelpString, out int pdwHelpStringContext, out string pbstrHelpStringDll);

		void GetLibStatistics(IntPtr pcUniqueNames, out int pcchUniqueNames);

		void GetAllCustData(IntPtr pCustData);
	}
}
