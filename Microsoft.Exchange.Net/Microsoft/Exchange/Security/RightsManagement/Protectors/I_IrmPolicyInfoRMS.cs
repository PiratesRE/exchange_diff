using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.Protectors
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("175EF0A4-8EB8-49ac-9049-F40EC69EC0A7")]
	[ComImport]
	internal interface I_IrmPolicyInfoRMS
	{
		[PreserveSig]
		int HrGetICrypt([MarshalAs(UnmanagedType.Interface)] out object piic);

		[PreserveSig]
		int HrGetSignedIL([MarshalAs(UnmanagedType.BStr)] out string pbstrIL);

		[PreserveSig]
		int HrGetServerId([MarshalAs(UnmanagedType.BStr)] out string pbstrServerId);

		[PreserveSig]
		int HrGetEULs([In] IntPtr rgbstrEUL, [In] IntPtr rgbstrId, [MarshalAs(UnmanagedType.U4)] out uint pcbEULs);

		[PreserveSig]
		int HrSetSignedIL([MarshalAs(UnmanagedType.BStr)] [In] string bstrIL);

		[PreserveSig]
		int HrSetServerEUL([MarshalAs(UnmanagedType.BStr)] [In] string bstrEUL);

		[PreserveSig]
		int HrGetRightsTemplate([MarshalAs(UnmanagedType.BStr)] out string pbstrRightsTemplate);

		[PreserveSig]
		int HrGetListGuid([MarshalAs(UnmanagedType.BStr)] out string pbstrListGuid);
	}
}
