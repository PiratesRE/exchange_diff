using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class EcpLogonInformation
	{
		internal EcpLogonInformation(SecurityIdentifier logonMailboxSid, IIdentity logonUserIdentity, IIdentity impersonatedUserIdentity)
		{
			if (logonMailboxSid == null)
			{
				throw new ArgumentNullException("logonMailboxSid");
			}
			if (logonUserIdentity == null)
			{
				throw new ArgumentNullException("LogonUserIdentity");
			}
			this.LogonMailboxSid = logonMailboxSid;
			this.LogonUser = logonUserIdentity;
			this.ImpersonatedUser = impersonatedUserIdentity;
		}

		public bool Impersonated
		{
			get
			{
				return this.ImpersonatedUser != null;
			}
		}

		public string Name
		{
			get
			{
				string safeName = this.LogonUser.GetSafeName(true);
				if (this.Impersonated)
				{
					return Strings.OnbehalfOf(safeName, this.ImpersonatedUser.GetSafeName(true));
				}
				return safeName;
			}
		}

		public static EcpLogonInformation Create(string logonAccountSddlSid, string logonMailboxSddlSid, string targetMailboxSddlSid, SerializedAccessToken proxySecurityAccessToken)
		{
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(logonMailboxSddlSid);
			IIdentity logonUserIdentity = (proxySecurityAccessToken != null) ? new SerializedIdentity(proxySecurityAccessToken) : new GenericSidIdentity(logonMailboxSddlSid, string.Empty, securityIdentifier);
			IIdentity impersonatedUserIdentity = (string.IsNullOrEmpty(targetMailboxSddlSid) || logonMailboxSddlSid == targetMailboxSddlSid) ? null : new GenericSidIdentity(targetMailboxSddlSid, string.Empty, new SecurityIdentifier(targetMailboxSddlSid));
			return new EcpLogonInformation(securityIdentifier, logonUserIdentity, impersonatedUserIdentity);
		}

		public readonly SecurityIdentifier LogonMailboxSid;

		public readonly IIdentity LogonUser;

		public readonly IIdentity ImpersonatedUser;
	}
}
