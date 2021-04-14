using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[Guid("3127CA40-446E-11CE-8135-00AA004BB851")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IErrorLog
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void AddError([MarshalAs(UnmanagedType.LPWStr)] [In] string propertyName, [In] ref System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
	}
}
