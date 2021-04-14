using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.ITypeInfo instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Guid("00020401-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface UCOMITypeInfo
	{
		void GetTypeAttr(out IntPtr ppTypeAttr);

		void GetTypeComp(out UCOMITypeComp ppTComp);

		void GetFuncDesc(int index, out IntPtr ppFuncDesc);

		void GetVarDesc(int index, out IntPtr ppVarDesc);

		void GetNames(int memid, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [Out] string[] rgBstrNames, int cMaxNames, out int pcNames);

		void GetRefTypeOfImplType(int index, out int href);

		void GetImplTypeFlags(int index, out int pImplTypeFlags);

		void GetIDsOfNames([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)] [In] string[] rgszNames, int cNames, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [Out] int[] pMemId);

		void Invoke([MarshalAs(UnmanagedType.IUnknown)] object pvInstance, int memid, short wFlags, ref DISPPARAMS pDispParams, out object pVarResult, out EXCEPINFO pExcepInfo, out int puArgErr);

		void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);

		void GetDllEntry(int memid, INVOKEKIND invKind, out string pBstrDllName, out string pBstrName, out short pwOrdinal);

		void GetRefTypeInfo(int hRef, out UCOMITypeInfo ppTI);

		void AddressOfMember(int memid, INVOKEKIND invKind, out IntPtr ppv);

		void CreateInstance([MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		void GetMops(int memid, out string pBstrMops);

		void GetContainingTypeLib(out UCOMITypeLib ppTLB, out int pIndex);

		void ReleaseTypeAttr(IntPtr pTypeAttr);

		void ReleaseFuncDesc(IntPtr pFuncDesc);

		void ReleaseVarDesc(IntPtr pVarDesc);
	}
}
