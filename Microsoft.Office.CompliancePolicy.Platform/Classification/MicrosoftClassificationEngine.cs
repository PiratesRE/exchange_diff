using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[ClassInterface(ClassInterfaceType.None)]
	[TypeLibType(TypeLibTypeFlags.FCanCreate)]
	[Guid("9ACB63F5-A24E-4FDF-80F5-EC909334F2D2")]
	[ComImport]
	public class MicrosoftClassificationEngine : IMicrosoftClassificationEngine, ICAClassificationEngine
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void Init([MarshalAs(UnmanagedType.Interface)] [In] [Optional] IPropertyBag engineSettings, [MarshalAs(UnmanagedType.Interface)] [In] [Optional] IRulePackageLoader rulePackageLoader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public virtual extern ICAClassificationDefinitionCollection GetClassificationDefinitions([ComAliasName("Microsoft.Mce.Interop.Api.RULE_PACKAGE_DETAILS")] [In] ref RULE_PACKAGE_DETAILS rulePackageDetails);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public virtual extern ICAClassificationResultCollection ClassifyTextStream([MarshalAs(UnmanagedType.Interface)] [In] IStream stream, [In] uint rulePackageSize, [ComAliasName("Microsoft.Mce.Interop.Api.RULE_PACKAGE_DETAILS")] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] RULE_PACKAGE_DETAILS[] rulePackageDetails);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public virtual extern ICAClassificationSession GetClassificationSession([In] uint rulePackageSize, [ComAliasName("Microsoft.Mce.Interop.Api.RULE_PACKAGE_DETAILS")] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] RULE_PACKAGE_DETAILS[] rulePackageDetails);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void ClearRulePackageCache([MarshalAs(UnmanagedType.BStr)] [In] string rulePackageSetID, [MarshalAs(UnmanagedType.BStr)] [In] string rulePackageID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: ComAliasName("Microsoft.Mce.Interop.Api.VERSION_INFORMATION_DETAILS")]
		public virtual extern VERSION_INFORMATION_DETAILS GetEngineVersion();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.BStr)]
		public virtual extern string GetPerformanceDiagnostics([In] PerformanceDiagnosticsType performanceDiagnosticsType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public virtual extern ICAClassificationStreamSession GetClassificationStreamSession([In] uint rulePackageSize, [ComAliasName("Microsoft.Mce.Interop.Api.RULE_PACKAGE_DETAILS")] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [In] RULE_PACKAGE_DETAILS[] rulePackageDetails);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MicrosoftClassificationEngine();
	}
}
