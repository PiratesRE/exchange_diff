using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("97FDCA77-B6F2-4718-A1EB-29D0AECE9C03")]
	[ComImport]
	internal interface ICategoryMembershipEntry
	{
		CategoryMembershipEntry AllData { [SecurityCritical] get; }

		IDefinitionIdentity Identity { [SecurityCritical] get; }

		ISection SubcategoryMembership { [SecurityCritical] get; }
	}
}
