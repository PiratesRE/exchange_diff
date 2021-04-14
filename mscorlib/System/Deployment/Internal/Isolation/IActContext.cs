using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
	[Guid("0af57545-a72a-4fbe-813c-8554ed7d4528")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IActContext
	{
		[SecurityCritical]
		void GetAppId([MarshalAs(UnmanagedType.Interface)] out object AppId);

		[SecurityCritical]
		void EnumCategories([In] uint Flags, [In] IReferenceIdentity CategoryToMatch, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object EnumOut);

		[SecurityCritical]
		void EnumSubcategories([In] uint Flags, [In] IDefinitionIdentity CategoryId, [MarshalAs(UnmanagedType.LPWStr)] [In] string SubcategoryPattern, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object EnumOut);

		[SecurityCritical]
		void EnumCategoryInstances([In] uint Flags, [In] IDefinitionIdentity CategoryId, [MarshalAs(UnmanagedType.LPWStr)] [In] string Subcategory, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object EnumOut);

		[SecurityCritical]
		void ReplaceStringMacros([In] uint Flags, [MarshalAs(UnmanagedType.LPWStr)] [In] string Culture, [MarshalAs(UnmanagedType.LPWStr)] [In] string ReplacementPattern, [MarshalAs(UnmanagedType.LPWStr)] out string Replaced);

		[SecurityCritical]
		void GetComponentStringTableStrings([In] uint Flags, [MarshalAs(UnmanagedType.SysUInt)] [In] IntPtr ComponentIndex, [MarshalAs(UnmanagedType.SysUInt)] [In] IntPtr StringCount, [MarshalAs(UnmanagedType.LPArray)] [Out] string[] SourceStrings, [MarshalAs(UnmanagedType.LPArray)] out string[] DestinationStrings, [MarshalAs(UnmanagedType.SysUInt)] [In] IntPtr CultureFallbacks);

		[SecurityCritical]
		void GetApplicationProperties([In] uint Flags, [In] UIntPtr cProperties, [MarshalAs(UnmanagedType.LPArray)] [In] string[] PropertyNames, [MarshalAs(UnmanagedType.LPArray)] out string[] PropertyValues, [MarshalAs(UnmanagedType.LPArray)] out UIntPtr[] ComponentIndicies);

		[SecurityCritical]
		void ApplicationBasePath([In] uint Flags, [MarshalAs(UnmanagedType.LPWStr)] out string ApplicationPath);

		[SecurityCritical]
		void GetComponentManifest([In] uint Flags, [In] IDefinitionIdentity ComponentId, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ManifestInteface);

		[SecurityCritical]
		void GetComponentPayloadPath([In] uint Flags, [In] IDefinitionIdentity ComponentId, [MarshalAs(UnmanagedType.LPWStr)] out string PayloadPath);

		[SecurityCritical]
		void FindReferenceInContext([In] uint dwFlags, [In] IReferenceIdentity Reference, [MarshalAs(UnmanagedType.Interface)] out object MatchedDefinition);

		[SecurityCritical]
		void CreateActContextFromCategoryInstance([In] uint dwFlags, [In] ref CATEGORY_INSTANCE CategoryInstance, [MarshalAs(UnmanagedType.Interface)] out object ppCreatedAppContext);

		[SecurityCritical]
		void EnumComponents([In] uint dwFlags, [MarshalAs(UnmanagedType.Interface)] out object ppIdentityEnum);

		[SecurityCritical]
		void PrepareForExecution([MarshalAs(UnmanagedType.SysInt)] [In] IntPtr Inputs, [MarshalAs(UnmanagedType.SysInt)] [In] IntPtr Outputs);

		[SecurityCritical]
		void SetApplicationRunningState([In] uint dwFlags, [In] uint ulState, out uint ulDisposition);

		[SecurityCritical]
		void GetApplicationStateFilesystemLocation([In] uint dwFlags, [In] UIntPtr Component, [MarshalAs(UnmanagedType.SysInt)] [In] IntPtr pCoordinateList, [MarshalAs(UnmanagedType.LPWStr)] out string ppszPath);

		[SecurityCritical]
		void FindComponentsByDefinition([In] uint dwFlags, [In] UIntPtr ComponentCount, [MarshalAs(UnmanagedType.LPArray)] [In] IDefinitionIdentity[] Components, [MarshalAs(UnmanagedType.LPArray)] [Out] UIntPtr[] Indicies, [MarshalAs(UnmanagedType.LPArray)] [Out] uint[] Dispositions);

		[SecurityCritical]
		void FindComponentsByReference([In] uint dwFlags, [In] UIntPtr Components, [MarshalAs(UnmanagedType.LPArray)] [In] IReferenceIdentity[] References, [MarshalAs(UnmanagedType.LPArray)] [Out] UIntPtr[] Indicies, [MarshalAs(UnmanagedType.LPArray)] [Out] uint[] Dispositions);
	}
}
