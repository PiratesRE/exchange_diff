using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class CommonAccessTokenHelper
	{
		public static CommonAccessToken CreateLiveId(string emailAddress)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			IRecipientSession recipientSession = null;
			ADUser adRawEntry = CommonAccessTokenHelper.ResolveTenantUser(emailAddress, out recipientSession);
			LiveIdFbaTokenAccessor liveIdFbaTokenAccessor = LiveIdFbaTokenAccessor.Create(adRawEntry);
			return liveIdFbaTokenAccessor.GetToken();
		}

		public static CommonAccessToken CreateLiveIdBasic(string emailAddress)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			IRecipientSession recipientSession = null;
			ADUser adRawEntry = CommonAccessTokenHelper.ResolveTenantUser(emailAddress, out recipientSession);
			LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Create(adRawEntry);
			return liveIdBasicTokenAccessor.GetToken();
		}

		public static CommonAccessToken CreateCertificateSid(string emailAddress)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			IRecipientSession recipientSession = null;
			ADUser adRawEntry;
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				adRawEntry = CommonAccessTokenHelper.ResolveTenantUser(emailAddress, out recipientSession);
			}
			else
			{
				adRawEntry = CommonAccessTokenHelper.ResolveRootOrgUser(emailAddress, out recipientSession);
			}
			CertificateSidTokenAccessor certificateSidTokenAccessor = CertificateSidTokenAccessor.Create(adRawEntry);
			return certificateSidTokenAccessor.GetToken();
		}

		public static CommonAccessToken CreateWindows(string userPrincipalName)
		{
			if (string.IsNullOrEmpty(userPrincipalName))
			{
				throw new ArgumentNullException("userPrincipalName");
			}
			CommonAccessToken result;
			using (WindowsIdentity windowsIdentity = new WindowsIdentity(userPrincipalName))
			{
				result = CommonAccessTokenHelper.CreateWindows(windowsIdentity);
			}
			return result;
		}

		public static CommonAccessToken CreateWindows(WindowsIdentity windowsIdentity)
		{
			if (windowsIdentity == null)
			{
				throw new ArgumentNullException("windowsIdentity");
			}
			WindowsTokenAccessor windowsTokenAccessor = WindowsTokenAccessor.Create(windowsIdentity);
			return windowsTokenAccessor.GetToken();
		}

		public static CommonAccessToken CreateWindows(string username, string password)
		{
			CommonAccessToken result;
			using (AuthenticationContext authenticationContext = new AuthenticationContext())
			{
				SecurityStatus securityStatus = authenticationContext.LogonUser(username, password.ConvertToSecureString());
				if (securityStatus != SecurityStatus.OK)
				{
					throw new ApplicationException(string.Format("monitoring mailbox {0} logon failed, SecurityStatus={1}", username, securityStatus));
				}
				WindowsIdentity identity = authenticationContext.Identity;
				result = CommonAccessTokenHelper.CreateWindows(identity);
			}
			return result;
		}

		public static ADUser ResolveTenantUser(string emailAddress, out IRecipientSession recipientSession)
		{
			if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
			{
				throw new ArgumentException(string.Format("'{0}' is not a valid SMTP address", emailAddress), "emailAddress");
			}
			string domain = SmtpAddress.Parse(emailAddress).Domain;
			ADSessionSettings sessionSettings;
			try
			{
				sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(domain);
			}
			catch (CannotResolveTenantNameException ex)
			{
				throw new ArgumentException(string.Format("'{0}' cannot resolve as a tenant domain/r/nOriginal Exception: {1}", domain, ex.Message), "emailAddress");
			}
			recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 186, "ResolveTenantUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\CommonAccessTokenHelper.cs");
			ADUser aduser = recipientSession.FindByProxyAddress<ADUser>(new SmtpProxyAddress(emailAddress, true));
			if (aduser == null)
			{
				throw new ApplicationException(string.Format("monitoring mailbox {0} not found!", emailAddress));
			}
			return aduser;
		}

		public static ADUser ResolveRootOrgUser(string emailAddress, out IRecipientSession recipientSession)
		{
			if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
			{
				throw new ArgumentException(string.Format("'{0}' is not a valid SMTP address", emailAddress), "emailAddress");
			}
			string domain = SmtpAddress.Parse(emailAddress).Domain;
			recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 220, "ResolveRootOrgUser", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\CommonAccessTokenHelper.cs");
			ADUser aduser = recipientSession.FindByProxyAddress<ADUser>(new SmtpProxyAddress(emailAddress, true));
			if (aduser == null)
			{
				throw new ApplicationException(string.Format("monitoring mailbox {0} not found!", emailAddress));
			}
			return aduser;
		}
	}
}
