using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("00020412-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface ITypeInfo2 : ITypeInfo
	{
		void GetTypeAttr(out IntPtr ppTypeAttr);

		[__DynamicallyInvokable]
		void GetTypeComp(out ITypeComp ppTComp);

		void GetFuncDesc(int index, out IntPtr ppFuncDesc);

		void GetVarDesc(int index, out IntPtr ppVarDesc);

		[__DynamicallyInvokable]
		void GetNames(int memid, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [Out] string[] rgBstrNames, int cMaxNames, out int pcNames);

		[__DynamicallyInvokable]
		void GetRefTypeOfImplType(int index, out int href);

		[__DynamicallyInvokable]
		void GetImplTypeFlags(int index, out IMPLTYPEFLAGS pImplTypeFlags);

		[__DynamicallyInvokable]
		void GetIDsOfNames([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)] [In] string[] rgszNames, int cNames, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [Out] int[] pMemId);

		void Invoke([MarshalAs(UnmanagedType.IUnknown)] object pvInstance, int memid, short wFlags, ref DISPPARAMS pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, out int puArgErr);

		[__DynamicallyInvokable]
		void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);

		void GetDllEntry(int memid, INVOKEKIND invKind, IntPtr pBstrDllName, IntPtr pBstrName, IntPtr pwOrdinal);

		[__DynamicallyInvokable]
		void GetRefTypeInfo(int hRef, out ITypeInfo ppTI);

		void AddressOfMember(int memid, INVOKEKIND invKind, out IntPtr ppv);

		[__DynamicallyInvokable]
		void CreateInstance([MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		[__DynamicallyInvokable]
		void GetMops(int memid, out string pBstrMops);

		[__DynamicallyInvokable]
		void GetContainingTypeLib(out ITypeLib ppTLB, out int pIndex);

		[PreserveSig]
		void ReleaseTypeAttr(IntPtr pTypeAttr);

		[PreserveSig]
		void ReleaseFuncDesc(IntPtr pFuncDesc);

		[PreserveSig]
		void ReleaseVarDesc(IntPtr pVarDesc);

		[__DynamicallyInvokable]
		void GetTypeKind(out TYPEKIND pTypeKind);

		[__DynamicallyInvokable]
		void GetTypeFlags(out int pTypeFlags);

		[__DynamicallyInvokable]
		void GetFuncIndexOfMemId(int memid, INVOKEKIND invKind, out int pFuncIndex);

		[__DynamicallyInvokable]
		void GetVarIndexOfMemId(int memid, out int pVarIndex);

		[__DynamicallyInvokable]
		void GetCustData(ref Guid guid, out object pVarVal);

		[__DynamicallyInvokable]
		void GetFuncCustData(int index, ref Guid guid, out object pVarVal);

		[__DynamicallyInvokable]
		void GetParamCustData(int indexFunc, int indexParam, ref Guid guid, out object pVarVal);

		[__DynamicallyInvokable]
		void GetVarCustData(int index, ref Guid guid, out object pVarVal);

		[__DynamicallyInvokable]
		void GetImplTypeCustData(int index, ref Guid guid, out object pVarVal);

		[LCIDConversion(1)]
		[__DynamicallyInvokable]
		void GetDocumentation2(int memid, out string pbstrHelpString, out int pdwHelpStringContext, out string pbstrHelpStringDll);

		void GetAllCustData(IntPtr pCustData);

		void GetAllFuncCustData(int index, IntPtr pCustData);

		void GetAllParamCustData(int indexFunc, int indexParam, IntPtr pCustData);

		void GetAllVarCustData(int index, IntPtr pCustData);

		void GetAllImplTypeCustData(int index, IntPtr pCustData);
	}
}
