using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Metabase
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct COAUTHINFO
	{
		public COAUTHINFO(Authn authnSvc, Authz authzSvc, AuthnLevel AuthnLevel, ImpLevel ImpersonationLevel)
		{
			this.dwAuthnSvc = authnSvc;
			this.dwAuthzSvc = authzSvc;
			this.pwszServerPrincName = null;
			this.dwAuthnLevel = AuthnLevel;
			this.dwImpersonationLevel = ImpersonationLevel;
			this.pAuthIdentityData = IntPtr.Zero;
			this.dwCapabilities = 0;
		}

		public Authn dwAuthnSvc;

		public Authz dwAuthzSvc;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwszServerPrincName;

		public AuthnLevel dwAuthnLevel;

		public ImpLevel dwImpersonationLevel;

		public IntPtr pAuthIdentityData;

		public int dwCapabilities;
	}
}
