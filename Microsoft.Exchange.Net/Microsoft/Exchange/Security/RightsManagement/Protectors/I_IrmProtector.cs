using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement.Protectors
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("FCFBC0AC-672B-452D-80E5-40652503D96E")]
	[ComImport]
	internal interface I_IrmProtector
	{
		[PreserveSig]
		int HrInit([MarshalAs(UnmanagedType.BStr)] out string pbstrProduct, [MarshalAs(UnmanagedType.U4)] out int pdwVersion, [MarshalAs(UnmanagedType.BStr)] out string pbstrExtentions, [MarshalAs(UnmanagedType.Bool)] out bool pfUseRMS);

		[PreserveSig]
		int HrIsProtected([MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbInput, [MarshalAs(UnmanagedType.I4)] out MsoIpiResult pdwResult);

		[PreserveSig]
		int HrSetLangId([MarshalAs(UnmanagedType.U2)] [In] ushort langid);

		[PreserveSig]
		int HrProtectRMS([MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbInput, [MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbOutput, [MarshalAs(UnmanagedType.Interface)] [In] I_IrmPolicyInfoRMS piid, [MarshalAs(UnmanagedType.I4)] out MsoIpiStatus pdwStatus);

		[PreserveSig]
		int HrUnprotectRMS([MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbInput, [MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbOutput, [MarshalAs(UnmanagedType.Interface)] [In] I_IrmPolicyInfoRMS piid, [MarshalAs(UnmanagedType.I4)] out MsoIpiStatus pdwStatus);

		[PreserveSig]
		int HrProtect([MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbInput, [MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbOutput, [MarshalAs(UnmanagedType.Interface)] [In] object piid, [MarshalAs(UnmanagedType.I4)] out MsoIpiStatus pdwStatus);

		[PreserveSig]
		int HrUnprotect([MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbInput, [MarshalAs(UnmanagedType.Interface)] [In] ILockBytes pilbOutput, [MarshalAs(UnmanagedType.Interface)] [In] object piid, [MarshalAs(UnmanagedType.I4)] out MsoIpiStatus pdwStatus);
	}
}
