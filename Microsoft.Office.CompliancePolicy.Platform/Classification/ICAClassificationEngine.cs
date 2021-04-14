using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[TypeLibType(TypeLibTypeFlags.FNonExtensible)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("6475565D-98F7-4362-8B1A-BB0AD3D3EB33")]
	[ComImport]
	public interface ICAClassificationEngine
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void Init([MarshalAs(UnmanagedType.Interface)] [In] [Optional] IPropertyBag engineSettings, [MarshalAs(UnmanagedType.Interface)] [In] [Optional] IRulePackageLoader rulePackageLoader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ICAClassificationDefinitionCollection GetClassificationDefinitions([ComAliasName("Microsoft.Mce.Interop.Api.RULE_PACKAGE_DETAILS")] [In] ref RULE_PACKAGE_DETAILS rulePackageDetails);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ICAClassificationResultCollection ClassifyTextStream([MarshalAs(UnmanagedType.Interface)] [In] IStream stream, [In] uint rulePackageSize, [ComAliasName("Microsoft.Mce.Interop.Api.RULE_PACKAGE_DETAILS")] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] RULE_PACKAGE_DETAILS[] rulePackageDetails);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ICAClassificationSession GetClassificationSession([In] uint rulePackageSize, [ComAliasName("Microsoft.Mce.Interop.Api.RULE_PACKAGE_DETAILS")] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] RULE_PACKAGE_DETAILS[] rulePackageDetails);

		[MethodImpl(MethodImplOptions.InternalCall)]
		void ClearRulePackageCache([MarshalAs(UnmanagedType.BStr)] [In] string rulePackageSetID, [MarshalAs(UnmanagedType.BStr)] [In] string rulePackageID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: ComAliasName("Microsoft.Mce.Interop.Api.VERSION_INFORMATION_DETAILS")]
		VERSION_INFORMATION_DETAILS GetEngineVersion();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string GetPerformanceDiagnostics([In] PerformanceDiagnosticsType performanceDiagnosticsType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ICAClassificationStreamSession GetClassificationStreamSession([In] uint rulePackageSize, [ComAliasName("Microsoft.Mce.Interop.Api.RULE_PACKAGE_DETAILS")] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] RULE_PACKAGE_DETAILS[] rulePackageDetails);
	}
}
