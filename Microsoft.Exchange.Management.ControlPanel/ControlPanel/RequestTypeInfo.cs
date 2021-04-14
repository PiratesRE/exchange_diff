using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal struct RequestTypeInfo
	{
		public bool Need302Redirect;

		public bool NeedRedirectTargetTenant;

		public bool IsEsoRequest;

		public string EsoMailboxSmtpAddress;

		public bool IsDelegatedAdminRequest;

		public bool IsByoidAdmin;

		public string TargetTenant;

		public bool UseImplicitPathRewrite;

		public bool IsSecurityTokenPresented;
	}
}
