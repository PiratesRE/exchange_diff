using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
	[Guid("a5c62f6d-5e3e-4cd9-b345-6b281d7a1d1e")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IStore
	{
		[SecurityCritical]
		void Transact([In] IntPtr cOperation, [MarshalAs(UnmanagedType.LPArray)] [In] StoreTransactionOperation[] rgOperations, [MarshalAs(UnmanagedType.LPArray)] [Out] uint[] rgDispositions, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] rgResults);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object BindReferenceToAssembly([In] uint Flags, [In] IReferenceIdentity ReferenceIdentity, [In] uint cDeploymentsToIgnore, [MarshalAs(UnmanagedType.LPArray)] [In] IDefinitionIdentity[] DefinitionIdentity_DeploymentsToIgnore, [In] ref Guid riid);

		[SecurityCritical]
		void CalculateDelimiterOfDeploymentsBasedOnQuota([In] uint dwFlags, [In] IntPtr cDeployments, [MarshalAs(UnmanagedType.LPArray)] [In] IDefinitionAppId[] rgpIDefinitionAppId_Deployments, [In] ref StoreApplicationReference InstallerReference, [In] ulong ulonglongQuota, [In] [Out] ref IntPtr Delimiter, [In] [Out] ref ulong SizeSharedWithExternalDeployment, [In] [Out] ref ulong SizeConsumedByInputDeploymentArray);

		[SecurityCritical]
		IntPtr BindDefinitions([In] uint Flags, [MarshalAs(UnmanagedType.SysInt)] [In] IntPtr Count, [MarshalAs(UnmanagedType.LPArray)] [In] IDefinitionIdentity[] DefsToBind, [In] uint DeploymentsToIgnore, [MarshalAs(UnmanagedType.LPArray)] [In] IDefinitionIdentity[] DefsToIgnore);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object GetAssemblyInformation([In] uint Flags, [In] IDefinitionIdentity DefinitionIdentity, [In] ref Guid riid);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumAssemblies([In] uint Flags, [In] IReferenceIdentity ReferenceIdentity_ToMatch, [In] ref Guid riid);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumFiles([In] uint Flags, [In] IDefinitionIdentity DefinitionIdentity, [In] ref Guid riid);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumInstallationReferences([In] uint Flags, [In] IDefinitionIdentity DefinitionIdentity, [In] ref Guid riid);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string LockAssemblyPath([In] uint Flags, [In] IDefinitionIdentity DefinitionIdentity, out IntPtr Cookie);

		[SecurityCritical]
		void ReleaseAssemblyPath([In] IntPtr Cookie);

		[SecurityCritical]
		ulong QueryChangeID([In] IDefinitionIdentity DefinitionIdentity);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumCategories([In] uint Flags, [In] IReferenceIdentity ReferenceIdentity_ToMatch, [In] ref Guid riid);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumSubcategories([In] uint Flags, [In] IDefinitionIdentity CategoryId, [MarshalAs(UnmanagedType.LPWStr)] [In] string SubcategoryPathPattern, [In] ref Guid riid);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumCategoryInstances([In] uint Flags, [In] IDefinitionIdentity CategoryId, [MarshalAs(UnmanagedType.LPWStr)] [In] string SubcategoryPath, [In] ref Guid riid);

		[SecurityCritical]
		void GetDeploymentProperty([In] uint Flags, [In] IDefinitionAppId DeploymentInPackage, [In] ref StoreApplicationReference Reference, [In] ref Guid PropertySet, [MarshalAs(UnmanagedType.LPWStr)] [In] string pcwszPropertyName, out BLOB blob);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string LockApplicationPath([In] uint Flags, [In] IDefinitionAppId ApId, out IntPtr Cookie);

		[SecurityCritical]
		void ReleaseApplicationPath([In] IntPtr Cookie);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumPrivateFiles([In] uint Flags, [In] IDefinitionAppId Application, [In] IDefinitionIdentity DefinitionIdentity, [In] ref Guid riid);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumInstallerDeploymentMetadata([In] uint Flags, [In] ref StoreApplicationReference Reference, [In] IReferenceAppId Filter, [In] ref Guid riid);

		[SecurityCritical]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object EnumInstallerDeploymentMetadataProperties([In] uint Flags, [In] ref StoreApplicationReference Reference, [In] IDefinitionAppId Filter, [In] ref Guid riid);
	}
}
