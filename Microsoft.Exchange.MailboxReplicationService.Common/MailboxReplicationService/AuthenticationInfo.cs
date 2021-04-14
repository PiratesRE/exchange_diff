using System;
using System.Security.Principal;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class AuthenticationInfo : IAuthenticationInfo
	{
		public AuthenticationInfo(WindowsIdentity identity, string name)
		{
			this.principal = new WindowsPrincipal(identity);
			this.name = name;
			this.isCertificateAuthentication = false;
		}

		public AuthenticationInfo(bool isCertificateAuthentication)
		{
			this.principal = null;
			this.name = null;
			this.isCertificateAuthentication = isCertificateAuthentication;
		}

		public AuthenticationInfo(SecurityIdentifier sid)
		{
			this.sid = sid;
			this.principal = null;
			this.name = null;
			this.isCertificateAuthentication = false;
		}

		WindowsPrincipal IAuthenticationInfo.WindowsPrincipal
		{
			get
			{
				return this.principal;
			}
		}

		string IAuthenticationInfo.PrincipalName
		{
			get
			{
				return this.name;
			}
		}

		bool IAuthenticationInfo.IsCertificateAuthentication
		{
			get
			{
				return this.isCertificateAuthentication;
			}
		}

		SecurityIdentifier IAuthenticationInfo.Sid
		{
			get
			{
				return this.sid;
			}
		}

		private WindowsPrincipal principal;

		private SecurityIdentifier sid;

		private string name;

		private bool isCertificateAuthentication;
	}
}
