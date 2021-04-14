using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Misc
{
	[Guid("3127CA40-446E-11CE-8135-00AA004BB851")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IErrorLog
	{
		void AddError([MarshalAs(UnmanagedType.LPWStr)] [In] string pszPropName, [In] ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
	}
}
