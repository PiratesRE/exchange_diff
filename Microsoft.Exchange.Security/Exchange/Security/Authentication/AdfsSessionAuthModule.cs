using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Web.Configuration;

namespace Microsoft.Exchange.Security.Authentication
{
	public class AdfsSessionAuthModule : SessionAuthenticationModule
	{
		protected AdfsSessionAuthModule()
		{
			AdfsFederationAuthModule.InitStaticVariables();
		}

		protected override void InitializeModule(HttpApplication application)
		{
			if (AdfsFederationAuthModule.IsAdfsAuthenticationEnabled)
			{
				ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::InitializeModule]: ADFS auth is enabled. Initializing module.");
				base.InitializeModule(application);
			}
		}

		protected override void InitializePropertiesFromConfiguration(string serviceName)
		{
			if (AdfsFederationAuthModule.IsAdfsAuthenticationEnabled)
			{
				base.InitializePropertiesFromConfiguration(serviceName);
				ServiceElement element = AdfsFederationAuthModule.Section.ServiceElements.GetElement(serviceName);
				CookieHandlerElement cookieHandler = element.FederatedAuthentication.CookieHandler;
				base.CookieHandler.Path = cookieHandler.Path;
			}
		}

		protected override void OnAuthenticateRequest(object sender, EventArgs eventArgs)
		{
			ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::OnAuthenticateRequest]: Entry.");
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			this.InternalOnAuthenticateRequest(sender, eventArgs);
		}

		protected override void OnPostAuthenticateRequest(object sender, EventArgs eventArgs)
		{
		}

		protected override void SetPrincipalFromSessionToken(SessionSecurityToken sessionSecurityToken)
		{
			ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: Entry.");
			if (sessionSecurityToken.ClaimsPrincipal != null)
			{
				ClaimsIdentity claimsIdentity = (ClaimsIdentity)sessionSecurityToken.ClaimsPrincipal.Identity;
				ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: Has found a valid ClaimsIdentity.");
				string text = null;
				string text2 = null;
				List<string> list = null;
				bool isPublicSession = true;
				bool flag = false;
				try
				{
					AdfsSessionAuthModule.GetClaimsFromSecurityToken(claimsIdentity, out text, out text2, out list, out isPublicSession);
					if (!string.IsNullOrWhiteSpace(text))
					{
						ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: Prcoessing identity from user SID '{0}'.", text);
						if (list == null)
						{
							ExTraceGlobals.AdfsAuthModuleTracer.TraceError<string>(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: No group SID claims sent for SID '{0}'.", text);
							throw new AdfsConfigurationException(AdfsConfigErrorReason.GroupSidsClaimMissing, "No group SID claims sent.");
						}
						bool enabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaServer.ShouldSkipAdfsGroupReadOnFrontend.Enabled;
						OrganizationId organizationId;
						List<string> list2;
						AdfsSessionAuthModule.GetRecipientInformationFromAd(text, text2, enabled, out organizationId, out list2);
						if (list2 != null)
						{
							ExTraceGlobals.AdfsAuthModuleTracer.TraceError<string>(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: Adding O365 groups to token for SID '{0}'.", text);
							list.AddRange(list2);
						}
						ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: Setting HttpContext.Current.User and Thread.CurrentPrincipal for SID '{0}'.", text);
						string partitionId = (organizationId == null) ? null : organizationId.PartitionId.ToString();
						AdfsIdentity identity = new AdfsIdentity(text2, text, organizationId, partitionId, list, isPublicSession);
						IPrincipal principal = new GenericPrincipal(identity, null);
						HttpContext.Current.User = principal;
						Thread.CurrentPrincipal = principal;
						ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: Creating the Common Access Token for SID '{0}'.", text);
						AdfsTokenAccessor adfsTokenAccessor = AdfsTokenAccessor.Create(identity);
						HttpContext.Current.Items["Item-CommonAccessToken"] = adfsTokenAccessor.GetToken();
						flag = true;
					}
				}
				catch (AdfsConfigurationException ex)
				{
					ExTraceGlobals.AdfsAuthModuleTracer.TraceError<AdfsConfigurationException>(0L, "[AdfsSessionAuthModule::InternalOnAuthenticateRequest]: Exception occurred: {0}.", ex);
					HttpContext.Current.Response.Redirect(string.Format(CultureInfo.InvariantCulture, "{0}?msg={1}", new object[]
					{
						"/owa/auth/errorfe.aspx",
						ex.Reason
					}), true);
				}
				if (!flag && !Datacenter.IsDatacenterDedicated(true))
				{
					ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: No SID claim present. Authenticating using UPN '{0}'.", text2);
					try
					{
						WindowsIdentity ntIdentity = new WindowsIdentity(text2);
						IPrincipal principal2 = new WindowsPrincipal(ntIdentity);
						HttpContext.Current.User = principal2;
						Thread.CurrentPrincipal = principal2;
					}
					catch (SecurityException arg)
					{
						ExTraceGlobals.AdfsAuthModuleTracer.TraceWarning<SecurityException>(0L, "[AdfsSessionAuthModule::SetPrincipalFromSessionToken]: SecurityException occurred. The user may have been removed from AD: '{0}.", arg);
					}
				}
			}
		}

		private static void GetClaimsFromSecurityToken(ClaimsIdentity claimsIdentity, out string userSid, out string upn, out List<string> groupSids, out bool isPublicSession)
		{
			ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]: Entry.");
			userSid = null;
			upn = null;
			groupSids = new List<string>();
			isPublicSession = true;
			foreach (Claim claim in claimsIdentity.Claims)
			{
				if (claim.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid")
				{
					if (userSid != null)
					{
						ExTraceGlobals.AdfsAuthModuleTracer.TraceError<string, string>(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]: Primary SID claim sent multiple times: '{0}', {1}.", userSid, claim.Value);
						throw new AdfsConfigurationException(AdfsConfigErrorReason.DuplicateClaims, "Primary SID claim sent multiple times.");
					}
					userSid = claim.Value;
					ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]: Primary SID claim found: '{0}'.", userSid);
				}
				else if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")
				{
					if (upn != null)
					{
						ExTraceGlobals.AdfsAuthModuleTracer.TraceError<string, string>(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]: UPN claim sent multiple times: '{0}', {1}.", upn, claim.Value);
						throw new AdfsConfigurationException(AdfsConfigErrorReason.DuplicateClaims, "UPN claim sent multiple times.");
					}
					upn = claim.Value;
					ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string, string>(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]: UPN claim found for SID '{0}': '{1}'.", userSid, upn);
				}
				else if (claim.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid")
				{
					ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string, string>(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]: Group SID claim found for SID '{0}': '{1}'.", userSid, claim.Value);
					groupSids.Add(claim.Value);
				}
				else if (claim.ClaimType == "http://schemas.microsoft.com/ws/2012/01/insidecorporatenetwork")
				{
					bool flag = false;
					if (!bool.TryParse(claim.Value, out flag))
					{
						ExTraceGlobals.AdfsAuthModuleTracer.TraceError(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]:InsideCorpNetworkClaim has been sent but could not be parsed. Treating the call as from an external network.");
						flag = false;
					}
					if (flag)
					{
						ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]:InsideCorpNetworkClaim has been sent. Treating the call as from an internal network.");
						isPublicSession = false;
					}
				}
			}
			if (string.IsNullOrWhiteSpace(upn))
			{
				ExTraceGlobals.AdfsAuthModuleTracer.TraceError<string>(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]: No UPN claim was found for SID: '{0}'.", userSid ?? "<NULL>");
				throw new AdfsConfigurationException(AdfsConfigErrorReason.UpnClaimMissing, "No UPN claim was found.");
			}
			if (string.IsNullOrWhiteSpace(userSid))
			{
				ExTraceGlobals.AdfsAuthModuleTracer.TraceWarning(0L, "[AdfsSessionAuthModule::GetClaimsFromSecurityToken]: No primary SID claim was found on the identity.");
			}
		}

		private static void GetRecipientInformationFromAd(string userAccountSid, string upn, bool skipGroupLookup, out OrganizationId orgId, out List<string> o365GroupSidIds)
		{
			ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::GetRecipientInformationFromAd]: Entry.");
			o365GroupSidIds = null;
			string text = null;
			try
			{
				text = SmtpAddress.Parse(upn).Domain;
			}
			catch (FormatException ex)
			{
				ExTraceGlobals.AdfsAuthModuleTracer.TraceError<FormatException>(0L, "[AdfsSessionAuthModule::GetRecipientInformationFromAd]: Could not parse UPN into SMTP address: {0}", ex);
				throw new AdfsConfigurationException(AdfsConfigErrorReason.InvalidUpn, ex.Message);
			}
			ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string, string>(0L, "[AdfsSessionAuthModule::GetRecipientInformationFromAd]: Using domain '{0}' for SID '{1}'.", text, userAccountSid);
			ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::GetRecipientInformationFromAd]: Constructing IRecpientSession object.");
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, true, ConsistencyMode.FullyConsistent, null, ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(text), 316, "GetRecipientInformationFromAd", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\AdfsAuth\\AdfsSessionAuthModule.cs");
			ADRecipient adrecipient = tenantOrRootOrgRecipientSession.FindBySid(new SecurityIdentifier(userAccountSid));
			orgId = null;
			if (adrecipient != null)
			{
				if (skipGroupLookup)
				{
					ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::GetRecipientInformationFromAd]: Group SID lookup skipped");
				}
				else
				{
					o365GroupSidIds = tenantOrRootOrgRecipientSession.GetTokenSids(adrecipient, AssignmentMethod.S4U);
					if (o365GroupSidIds != null)
					{
						ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsSessionAuthModule::GetRecipientInformationFromAd]: Found O365 Group SIDs: '{0}'", string.Join(", ", o365GroupSidIds));
					}
				}
				orgId = adrecipient.OrganizationId;
				ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string, OrganizationId>(0L, "[AdfsSessionAuthModule::GetRecipientInformationFromAd]: Found ADRecipient '{0}' for SID '{1}'.The OrganizationId is: '{2}'.", adrecipient.DisplayName, orgId);
			}
		}

		private void InternalOnAuthenticateRequest(object sender, EventArgs eventArgs)
		{
			ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::InternalOnAuthenticateRequest]: Entry.");
			SessionSecurityToken principalFromSessionToken = null;
			HttpRequest request = HttpContext.Current.Request;
			if (request.IsAnonymousAuthFolderRequest())
			{
				ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::InternalOnAuthenticateRequest]: Request is auth folder anonymous.");
				return;
			}
			if (request.IsAuthenticatedByAdfs() && base.TryReadSessionTokenFromCookie(ref principalFromSessionToken))
			{
				ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::InternalOnAuthenticateRequest]: Attempting to authenticate using ADFS auth module.");
				this.SetPrincipalFromSessionToken(principalFromSessionToken);
				return;
			}
			if (request.PreferAdfsAuthentication())
			{
				ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsSessionAuthModule::InternalOnAuthenticateRequest]: Attempting to authenticate using using base WIF classes.");
				base.OnAuthenticateRequest(sender, eventArgs);
			}
		}

		private const string InsideCorpNetworkClaim = "http://schemas.microsoft.com/ws/2012/01/insidecorporatenetwork";
	}
}
