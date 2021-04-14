using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[Guid("2FBDB1F0-90B0-4008-9F43-FA5FFCAAF9A2")]
	[TypeLibType(TypeLibTypeFlags.FNonExtensible)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface ICAClassificationDefinition
	{
		string ID { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

		string PublisherID { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

		ICAAttributeDefinitionCollection AttributeDefinitions { [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.Interface)] get; }

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: ComAliasName("Microsoft.Mce.Interop.Api.CLASSIFICATION_DEFINITION_DETAILS")]
		CLASSIFICATION_DEFINITION_DETAILS GetLocalizableDetails([MarshalAs(UnmanagedType.BStr)] [In] string localeName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: ComAliasName("Microsoft.Mce.Interop.Api.VERSION_INFORMATION_DETAILS")]
		VERSION_INFORMATION_DETAILS GetRulePackageVersion();
	}
}
