using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Misc
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("22F55882-280B-11D0-A8A9-00A0C90C2004")]
	internal interface IPropertyBag2
	{
		void Read([In] int cProperties, [In] ref PROPBAG2 pPropBag, [MarshalAs(UnmanagedType.Interface)] [In] IErrorLog pErrLog, [MarshalAs(UnmanagedType.Struct)] out object pvarValue, [MarshalAs(UnmanagedType.Error)] [In] [Out] ref int phrError);

		void Write([In] int cProperties, [In] ref PROPBAG2 pPropBag, [MarshalAs(UnmanagedType.Struct)] [In] ref object pvarValue);

		void CountProperties(out int pcProperties);

		void GetPropertyInfo([In] int iProperty, [In] int cProperties, out PROPBAG2 pPropBag, out int pcProperties);

		void LoadObject([MarshalAs(UnmanagedType.LPWStr)] [In] string pstrName, [In] int dwHint, [MarshalAs(UnmanagedType.IUnknown)] [In] object pUnkObject, [MarshalAs(UnmanagedType.Interface)] [In] IErrorLog pErrLog);
	}
}
