using System;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Security.PartnerToken;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class ImpersonationProcessorBase
	{
		internal ImpersonationProcessorBase(AuthZClientInfo impersonatingClientInfo, IIdentity impersonatingIdentity)
		{
			this.impersonatingClientInfo = impersonatingClientInfo;
			this.impersonatingIdentity = impersonatingIdentity;
		}

		internal AuthZClientInfo CreateAuthZClientInfo()
		{
			ADRecipientSessionContext adrecipientSessionContext = this.impersonatingClientInfo.GetADRecipientSessionContext();
			bool flag = true;
			if (EWSSettings.IsPartnerHostedOnly || EWSSettings.IsMultiTenancyEnabled)
			{
				flag = (this.impersonatingClientInfo.ClientSecurityContext != null && ExchangeRunspaceConfiguration.IsAllowedOrganizationForPartnerAccounts(adrecipientSessionContext.OrganizationId));
			}
			ADRecipientSessionContext adRecipientSessionContext;
			if (flag)
			{
				adRecipientSessionContext = this.CreateADRecipientSessionContext();
			}
			else
			{
				adRecipientSessionContext = adrecipientSessionContext;
			}
			UserIdentity impersonatedUserIdentity = this.GetImpersonatedUserIdentity(adRecipientSessionContext);
			if (this.impersonatingIdentity != null)
			{
				OAuthIdentity oauthIdentity = this.impersonatingIdentity as OAuthIdentity;
				if (oauthIdentity != null && oauthIdentity.OAuthApplication.ApplicationType == OAuthApplicationType.V1App && oauthIdentity.IsAppOnly)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<V1ProfileAppInfo>((long)this.GetHashCode(), "ImpersonationProcessorBase.GetAuthZClientInfo. skipping validating the impersonation right given that admin approves this caller which is V1Profile AppOnly", oauthIdentity.OAuthApplication.V1ProfileApp);
				}
				else
				{
					this.ValidateImpersonationRights(impersonatedUserIdentity.ADUser, flag);
				}
			}
			else
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ADUser>((long)this.GetHashCode(), "ImpersonationProcessorBase.GetAuthZClientInfo. impersonatingWindowsPrincipal is null. Impersonation rights for {0} were validated at the CAS that proxied this request.", impersonatedUserIdentity.ADUser);
			}
			return AuthZClientInfo.FromUserIdentity(impersonatedUserIdentity);
		}

		protected abstract ADRecipientSessionContext CreateADRecipientSessionContext();

		protected abstract UserIdentity GetImpersonatedUserIdentity(ADRecipientSessionContext adRecipientSessionContext);

		protected abstract ServicePermanentException CreateUserIdentitySearchFailedException(Exception innerException);

		private void ValidateImpersonationRights(ADUser impersonatedADUser, bool isFirstOrgPartnerCaller)
		{
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration;
			try
			{
				OrganizationId impersonatedOrganizationId = isFirstOrgPartnerCaller ? impersonatedADUser.OrganizationId : null;
				exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(this.impersonatingIdentity, impersonatedOrganizationId, false);
			}
			catch (CmdletAccessDeniedException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<IIdentity, CmdletAccessDeniedException>(0L, "Exception thrown from IsApplicationImpersonationAllowed. User: {0} Exception: {1}", this.impersonatingIdentity, ex);
				if (isFirstOrgPartnerCaller)
				{
					throw this.CreateUserIdentitySearchFailedException(ex);
				}
				throw new ImpersonateUserDeniedException(ex);
			}
			if (!exchangeRunspaceConfiguration.IsApplicationImpersonationAllowed(impersonatedADUser))
			{
				throw new ImpersonateUserDeniedException();
			}
			if (this.impersonatingIdentity is PartnerIdentity)
			{
				IAccountValidationContext accountValidationContext = new AccountValidationContextBySID(impersonatedADUser.Sid, impersonatedADUser.OrganizationId, ExDateTime.UtcNow, "Microsoft.Exchange.WebServices");
				switch (accountValidationContext.CheckAccount())
				{
				case AccountState.AccountDisabled:
					RequestDetailsLogger.Current.AppendGenericError("AccountDisabled", impersonatedADUser.UserPrincipalName);
					throw new ImpersonateUserDeniedException();
				case AccountState.PasswordExpired:
					break;
				case AccountState.AccountDeleted:
					RequestDetailsLogger.Current.AppendGenericError("AccountDeleted", impersonatedADUser.UserPrincipalName);
					throw new ImpersonateUserDeniedException();
				default:
					return;
				}
			}
		}

		private IIdentity impersonatingIdentity;

		private AuthZClientInfo impersonatingClientInfo;
	}
}
