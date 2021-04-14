using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[SecurityCritical(SecurityCriticalScope.Everything)]
	[StructLayout(LayoutKind.Sequential)]
	internal class BoundLicenseParams
	{
		internal uint Version;

		internal uint EnablingPrincipalHandle;

		internal uint SecureStoreHandle;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string WszRightsRequested;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string WszRightsGroup;

		internal uint DRMIDuVersion;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string DRMIDIdType;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string DRMIDId;

		internal uint AuthenticatorCount;

		internal IntPtr RghAuthenticators;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string WszDefaultEnablingPrincipalCredentials;

		internal uint DwFlags;
	}
}
