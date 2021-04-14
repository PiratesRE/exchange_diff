using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CFA3F59F-334D-46bf-A5A5-5D11BB2D7EBC")]
	[ComImport]
	internal interface IDeploymentMetadataEntry
	{
		DeploymentMetadataEntry AllData { [SecurityCritical] get; }

		string DeploymentProviderCodebase { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string MinimumRequiredVersion { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		ushort MaximumAge { [SecurityCritical] get; }

		byte MaximumAge_Unit { [SecurityCritical] get; }

		uint DeploymentFlags { [SecurityCritical] get; }
	}
}
