using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common.Sharing;
using Microsoft.Exchange.Net.Claim;
using Microsoft.Exchange.Net.WSSecurity;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Security.PartnerToken;
using Microsoft.Exchange.Security.X509CertAuth;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	public class EWSAuthorizationManager : ServiceAuthorizationManager
	{
		internal bool CheckAccessCore(EwsOperationContextBase operationContext)
		{
			RequestDetailsLogger requestDetailsLogger = RequestDetailsLogger.Current;
			requestDetailsLogger.UpdateLatency(ServiceLatencyMetadata.HttpPipelineLatency, requestDetailsLogger.ActivityScope.TotalMilliseconds);
			if (HttpContext.Current != null)
			{
				if (HttpContext.Current.Items["BackEndAuthenticator"] != null)
				{
					requestDetailsLogger.AppendGenericInfo("BackEndAuthenticator", HttpContext.Current.Items["BackEndAuthenticator"]);
				}
				if (HttpContext.Current.Items["TotalBERehydrationModuleLatency"] != null)
				{
					long num = (long)HttpContext.Current.Items["TotalBERehydrationModuleLatency"];
					requestDetailsLogger.AppendGenericInfo("TotalBERehydrationModuleLatency", HttpContext.Current.Items["TotalBERehydrationModuleLatency"]);
					if (num > 1000L)
					{
						requestDetailsLogger.AppendGenericInfo("RehydrationModuleBreakUp", string.Format("{0}:{1}/{2}:{3}/{4}:{5}", new object[]
						{
							"BEValidateCATRightsLatency",
							HttpContext.Current.Items["BEValidateCATRightsLatency"],
							"CATDeserializationLatency",
							HttpContext.Current.Items["CATDeserializationLatency"],
							"BERehydrationLatency",
							HttpContext.Current.Items["BERehydrationLatency"]
						}));
					}
				}
			}
			return requestDetailsLogger.TrackLatency<bool>(ServiceLatencyMetadata.CheckAccessCoreLatency, () => this.InternalCheckAccessCore(operationContext));
		}

		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			return this.CheckAccessCore(new WrappedWcfOperationContext(operationContext));
		}

		private static bool DoesClaimMatch(Claim claim, string claimTypeToTest, string rightToTest)
		{
			return claim.ClaimType == claimTypeToTest && claim.Right == rightToTest;
		}

		private static bool DoesClaimHaveProperResource<T>(Claim claim, out T resource) where T : class
		{
			resource = default(T);
			if (claim.Resource == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>(0L, "{0}/{1} claim had a null Resource", claim.ClaimType, claim.Right);
				return false;
			}
			resource = (claim.Resource as T);
			if (resource == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug(0L, "{0}/{1} claim had a claim of type {2}, not of type {3}", new object[]
				{
					claim.ClaimType,
					claim.Right,
					claim.Resource.GetType().FullName,
					typeof(T).FullName
				});
				return false;
			}
			return true;
		}

		private static bool? ProcessTrueFalseClaim(Claim claim)
		{
			string text;
			if (!EWSAuthorizationManager.DoesClaimHaveProperResource<string>(claim, out text))
			{
				return null;
			}
			bool value;
			if (string.Equals(text, "TRUE"))
			{
				value = true;
			}
			else
			{
				if (!string.Equals(text, "FALSE"))
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string, string>(0L, "{0}/{1} claim resource was not a known value: {2}", claim.ClaimType, claim.Right, text);
					return null;
				}
				value = false;
			}
			return new bool?(value);
		}

		private static EWSAuthorizationManager.ConsentLevel? ProcessConsentLevelClaim(Claim claim)
		{
			string text;
			if (!EWSAuthorizationManager.DoesClaimHaveProperResource<string>(claim, out text))
			{
				return null;
			}
			EWSAuthorizationManager.ConsentLevel value;
			if (string.Equals(text, "NONE"))
			{
				value = EWSAuthorizationManager.ConsentLevel.None;
			}
			else if (string.Equals(text, "PARTIAL"))
			{
				value = EWSAuthorizationManager.ConsentLevel.Partial;
			}
			else
			{
				if (!string.Equals(text, "FULL"))
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string, string>(0L, "{0}/{1} claim resource was not a known value: {2}", claim.ClaimType, claim.Right, text);
					return null;
				}
				value = EWSAuthorizationManager.ConsentLevel.Full;
			}
			return new EWSAuthorizationManager.ConsentLevel?(value);
		}

		private static bool CheckClaimSets(EwsOperationContextBase operationContext, ReadOnlyCollection<ClaimSet> claimSets)
		{
			HttpContext.Current.Items["AuthType"] = "LiveIdToken";
			claimSets.TraceClaimSets();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			string text = null;
			string text2 = null;
			foreach (ClaimSet claimSet in claimSets)
			{
				foreach (Claim claim in claimSet)
				{
					if (EWSAuthorizationManager.DoesClaimMatch(claim, "http://schemas.xmlsoap.org/claims/PUID", Rights.PossessProperty))
					{
						flag = EWSAuthorizationManager.DoesClaimHaveProperResource<string>(claim, out text);
					}
					else if (EWSAuthorizationManager.DoesClaimMatch(claim, "http://schemas.xmlsoap.org/claims/ConsumerPUID", Rights.PossessProperty))
					{
						flag2 = EWSAuthorizationManager.DoesClaimHaveProperResource<string>(claim, out text2);
					}
					else if (EWSAuthorizationManager.DoesClaimMatch(claim, ClaimTypes.Authentication, Rights.PossessProperty))
					{
						flag3 = true;
					}
					if (flag && flag2 && flag3)
					{
						break;
					}
				}
				if (flag && flag2 && flag3)
				{
					break;
				}
			}
			if (!flag3 || (text == null && text2 == null))
			{
				string reason = string.Format("Did not find all necessary claims. PUID: {0}; ConsumerPUID: {1}; Auth/Possess: {2}", flag, flag2, flag3);
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, reason);
				return false;
			}
			SmtpAddress smtpAddress;
			if (!EWSAuthorizationManager.TryGetEmailAddressInClaimSets(claimSets, out smtpAddress))
			{
				string reason2 = string.Format("Did not find EmailAddress claim for PUID: {0}; ", (text2 == null) ? text : text2);
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, reason2);
				return false;
			}
			RequestDetailsLogger.Current.AppendGenericInfo("CheckClaimSets_SmtpAddress", smtpAddress.ToString());
			HttpContext.Current.Items["LiveIdTokenSmtpClaim"] = smtpAddress.ToString();
			string userId = (text2 == null) ? text : text2;
			RequestDetailsLogger.Current.AppendGenericInfo("UserPUID", userId);
			PropertyDefinition[] propertyDefinitionArrayUPN = new PropertyDefinition[]
			{
				ADUserSchema.UserPrincipalName,
				ADMailboxRecipientSchema.Sid,
				ADObjectSchema.OrganizationId
			};
			ADRawEntry adRawEntry = null;
			Exception ex = null;
			if (!Directory.TryRunADOperationWithErrorHandling(delegate
			{
				ADSessionSettings adsessionSettings = Directory.SessionSettingsFromAddress(smtpAddress.ToString());
				if (!adsessionSettings.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 471, "CheckClaimSets", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\wcf\\EWSAuthorizationManager.cs");
					adRawEntry = tenantRecipientSession.FindUniqueEntryByNetID(userId, propertyDefinitionArrayUPN);
				}
			}, out ex))
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<Exception>(0L, "FindUniqueEntryByNetId threw exception: {0}", ex);
				if (ex is NonUniqueRecipientException)
				{
					EWSAuthorizationManager.AddTokenHashToBadTokenHashCache(operationContext);
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Found more than 1 user by NetID in AD");
					return false;
				}
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, ex.Message);
				return false;
			}
			else
			{
				if (adRawEntry == null)
				{
					EWSAuthorizationManager.AddTokenHashToBadTokenHashCache(operationContext);
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Could not find user by NetID in AD");
					return false;
				}
				string text3 = (string)adRawEntry[ADUserSchema.UserPrincipalName];
				SecurityIdentifier securityIdentifier = (SecurityIdentifier)adRawEntry[ADMailboxRecipientSchema.Sid];
				OrganizationId organizationId = (OrganizationId)adRawEntry[ADObjectSchema.OrganizationId];
				OrganizationProperties organizationProperties = null;
				if (!Directory.TryRunADOperationWithErrorHandling(delegate
				{
					OrganizationPropertyCache.TryGetOrganizationProperties(organizationId, out organizationProperties);
				}, out ex))
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<Exception>(0L, "OrganizationPropertyCache.TryGetOrganizationProperties threw exception: {0}", ex);
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, ex.Message);
					return false;
				}
				if (organizationProperties == null)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError<OrganizationId, string>(0L, "[EWSAuthorizationManager::CheckClaimSets] Logon failed: could not locate org info for organization {0} even though user from this org was found {1}", organizationId, text3);
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Could not find organization info via OrganizationPropertyCache");
					return false;
				}
				if (!organizationProperties.SkipToUAndParentalControlCheck && !EWSAuthorizationManager.CheckClaimSetsForTOUClaims(operationContext, claimSets, true) && !EWSAuthorizationManager.CheckClaimSetsForTOUClaims(operationContext, claimSets, false))
				{
					return false;
				}
				SidBasedIdentity sidBasedIdentity = new SidBasedIdentity(text3, securityIdentifier.ToString(), text3);
				sidBasedIdentity.UserOrganizationId = organizationId;
				EWSSettings.UpnFromClaimSets = text3;
				CallContext.PushUserInfoToHttpContext(HttpContext.Current, text3);
				HttpContext.Current.User = new GenericPrincipal(sidBasedIdentity, null);
				return true;
			}
		}

		private static bool CheckClaimSetsForTOUClaims(EwsOperationContextBase operationContext, ReadOnlyCollection<ClaimSet> claimSets, bool checkConsumerClaims)
		{
			string claimTypeToTest = checkConsumerClaims ? "http://schemas.xmlsoap.org/claims/ConsumerChild" : "http://schemas.xmlsoap.org/claims/Child";
			string claimTypeToTest2 = checkConsumerClaims ? "http://schemas.xmlsoap.org/claims/ConsumerTOUAccepted" : "http://schemas.xmlsoap.org/claims/TOUAccepted";
			string claimTypeToTest3 = checkConsumerClaims ? "http://schemas.xmlsoap.org/claims/ConsumerConsentLevel" : "http://schemas.xmlsoap.org/claims/ConsentLevel";
			EWSAuthorizationManager.ConsentLevel? consentLevel = null;
			bool? flag = null;
			bool? flag2 = null;
			foreach (ClaimSet claimSet in claimSets)
			{
				foreach (Claim claim in claimSet)
				{
					if (EWSAuthorizationManager.DoesClaimMatch(claim, claimTypeToTest, Rights.PossessProperty))
					{
						flag = EWSAuthorizationManager.ProcessTrueFalseClaim(claim);
					}
					else if (EWSAuthorizationManager.DoesClaimMatch(claim, claimTypeToTest2, Rights.PossessProperty))
					{
						flag2 = EWSAuthorizationManager.ProcessTrueFalseClaim(claim);
					}
					else if (EWSAuthorizationManager.DoesClaimMatch(claim, claimTypeToTest3, Rights.PossessProperty))
					{
						consentLevel = EWSAuthorizationManager.ProcessConsentLevelClaim(claim);
					}
					if (flag != null && flag2 != null && (!flag.Value || consentLevel != null))
					{
						break;
					}
				}
				if (flag != null && flag2 != null && (!flag.Value || consentLevel != null))
				{
					break;
				}
			}
			if (checkConsumerClaims && flag == null && flag2 == null && consentLevel == null)
			{
				return false;
			}
			if (flag == null)
			{
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Didn't find child claim");
				return false;
			}
			if (flag2 == null)
			{
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Didn't find TOU claim");
				return false;
			}
			if (flag.Value && consentLevel == null)
			{
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Didn't find consent level claim for child");
				return false;
			}
			if (!flag2.Value)
			{
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "TOU was not accepted");
				return false;
			}
			if (flag.Value && consentLevel.Value == EWSAuthorizationManager.ConsentLevel.None)
			{
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Child with no consent");
				return false;
			}
			return true;
		}

		private static void AddTokenHashToBadTokenHashCache(EwsOperationContextBase operationContext)
		{
			object obj;
			if (operationContext.IncomingMessageProperties.TryGetValue("WSSecurityTokenHash", out obj))
			{
				BadTokenHashCache.Singleton.InsertSliding((string)obj, true, BadTokenHashCache.CacheTimeout, null);
				return;
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug(0L, "Found no WS-Security token hash in the message - not adding to BadTokenHashCache");
		}

		private static bool CheckClaimSetsForExternalUser(AuthorizationContext authorizationContext, EwsOperationContextBase operationContext)
		{
			HttpContext.Current.Items["AuthType"] = "External";
			SamlSecurityToken samlSecurityToken = null;
			foreach (SupportingTokenSpecification supportingTokenSpecification in operationContext.SupportingTokens)
			{
				samlSecurityToken = (supportingTokenSpecification.SecurityToken as SamlSecurityToken);
				if (samlSecurityToken != null)
				{
					break;
				}
			}
			if (samlSecurityToken == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError(0L, "Found no security token in authorization context");
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Cannot find security token in authorization context");
				return false;
			}
			ExternalAuthentication current = ExternalAuthentication.GetCurrent();
			if (!current.Enabled)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError(0L, "Federation is not enabled");
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Federation is not enabled");
				return false;
			}
			TokenValidationResults tokenValidationResults = current.TokenValidator.ValidateToken(samlSecurityToken);
			if (tokenValidationResults.Result != TokenValidationResult.Valid || !SmtpAddress.IsValidSmtpAddress(tokenValidationResults.EmailAddress))
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<TokenValidationResults>(0L, "Validation of security token in WS-Security header failed: {0}", tokenValidationResults);
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Validation of the delegation failed");
				return false;
			}
			SmtpAddress smtpAddress = SmtpAddress.Empty;
			SharingSecurityHeader sharingSecurityHeader = null;
			int num = -1;
			try
			{
				num = operationContext.IncomingMessageHeaders.FindHeader("SharingSecurity", "http://schemas.microsoft.com/exchange/services/2006/types");
			}
			catch (MessageHeaderException ex)
			{
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Exception when looking for SharingSecurity header in request: " + ex.ToString());
				return false;
			}
			if (num < 0)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug(0L, "Request has no SharingSecurity header");
			}
			else
			{
				XmlElement header = operationContext.IncomingMessageHeaders.GetHeader<XmlElement>(num);
				try
				{
					smtpAddress = SharingKeyHandler.Decrypt(header, tokenValidationResults.ProofToken);
				}
				catch (TargetInvocationException ex2)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError<TargetInvocationException>(0L, "TargetInvocationException. Most likely trying to use non-FIPS encryption method in FIPS mode: {0}", ex2);
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "TargetInvocationException. Most likely trying to use non-FIPS encryption method in FIPS mode:" + ex2.ToString());
					return false;
				}
				if (smtpAddress == SmtpAddress.Empty)
				{
					ExTraceGlobals.AuthenticationTracer.TraceError<string>(0L, "SharingSecurity is present but invalid: {0}", header.OuterXml);
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Validation of the SharingSecurity failed");
					return false;
				}
				sharingSecurityHeader = new SharingSecurityHeader(header);
				ExTraceGlobals.AuthenticationTracer.TraceDebug<SmtpAddress>(0L, "SharingSecurity header contains external identity: {0}", smtpAddress);
			}
			XmlElement messageHeaderAsXmlElement = MessageHeaderProcessor.GetMessageHeaderAsXmlElement(operationContext.IncomingMessageHeaders, "Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
			if (messageHeaderAsXmlElement == null)
			{
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "WSSecurity header is missing.");
				return false;
			}
			CallContext.PushUserInfoToHttpContext(HttpContext.Current, tokenValidationResults.EmailAddress);
			using (WindowsIdentity current2 = WindowsIdentity.GetCurrent())
			{
				HttpContext.Current.User = new WindowsPrincipal(new ExternalIdentity(new SmtpAddress(tokenValidationResults.EmailAddress), smtpAddress, WSSecurityHeader.Create(messageHeaderAsXmlElement), sharingSecurityHeader, tokenValidationResults.Offer, current2.Token));
			}
			return true;
		}

		private static bool CheckClaimSetsForPartnerUser(AuthorizationContext authorizationContext, EwsOperationContextBase operationContext)
		{
			HttpContext.Current.Items["AuthType"] = "Partner";
			if (MessageHeaderProcessor.GetMessageHeaderAsXmlElement(operationContext.IncomingMessageHeaders, "ExchangeImpersonation", "http://schemas.microsoft.com/exchange/services/2006/types") == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError(0L, "[EWSAuthorizationManager.CheckClaimSetsForPartnerUser] missing impersonation header.");
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Missing impersonation header.");
				return false;
			}
			PerformanceMonitor.UpdateTotalRequestsReceivedWithPartnerToken();
			ReadOnlyCollection<ClaimSet> claimSets = authorizationContext.ClaimSets;
			claimSets.TraceClaimSets();
			DelegatedPrincipal delegatedPrincipal = null;
			OrganizationId delegatedOrganizationId = null;
			string text = null;
			if (!PartnerToken.TryGetDelegatedPrincipalAndOrganizationId(claimSets, out delegatedPrincipal, out delegatedOrganizationId, out text))
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<string>(0L, "[EWSAuthorizationManager.CheckClaimSetsForPartnerUser] unable to create partner identity, error message: {0}", text);
				PerformanceMonitor.UpdateTotalUnauthorizedRequestsReceivedWithPartnerToken();
				EWSAuthorizationManager.AddTokenHashToBadTokenHashCache(operationContext);
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, text);
				return false;
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<DelegatedPrincipal>(0L, "[EWSAuthorizationManager.CheckClaimSetsForPartnerUser] ws-security header contains partner identity: {0}", delegatedPrincipal);
			string text2 = delegatedPrincipal.ToString();
			EWSSettings.UpnFromClaimSets = text2;
			CallContext.PushUserInfoToHttpContext(HttpContext.Current, text2);
			HttpContext.Current.User = new WindowsPrincipal(PartnerIdentity.Create(delegatedPrincipal, delegatedOrganizationId));
			return true;
		}

		private static bool CheckClaimSetsForX509CertUser(AuthorizationContext authContext, EwsOperationContextBase operationContext)
		{
			HttpContext.Current.Items["AuthType"] = "X509Cert";
			ReadOnlyCollection<ClaimSet> claimSets = authContext.ClaimSets;
			claimSets.TraceClaimSets();
			X509CertUser x509CertUser = null;
			if (!X509CertUser.TryCreateX509CertUser(claimSets, out x509CertUser))
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug(0L, "[EWSAuthorizationManager.CheckClaimSetsForX509CertUser] unable to find create the x509certuser");
				EWSAuthorizationManager.AddTokenHashToBadTokenHashCache(operationContext);
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "unable to create the X509CertUser based on the given claim sets.");
				return false;
			}
			OrganizationId arg;
			SidBasedIdentity identity;
			string arg2;
			if (!x509CertUser.TryGetSidBasedIdentity(out arg, out identity, out arg2))
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<X509CertUser>(0L, "[EWSAuthorizationManager.CheckClaimSetsForX509CertUser] unable to find the AD user for cert user: {0}", x509CertUser);
				EWSAuthorizationManager.AddTokenHashToBadTokenHashCache(operationContext);
				string reason = string.Format("unable to find the AD user for the given cert {0}, reason: {1}", x509CertUser, arg2);
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, reason);
				return false;
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string, OrganizationId>(0L, "[EWSAuthorizationManager.CheckClaimSetsForX509CertUser] ws-security header contains the x509 cert user identity: {0}, upn: {1}, {2}", identity.GetSafeName(true), x509CertUser.UserPrincipalName, arg);
			EWSSettings.UpnFromClaimSets = x509CertUser.UserPrincipalName;
			CallContext.PushUserInfoToHttpContext(HttpContext.Current, x509CertUser.UserPrincipalName);
			HttpContext.Current.User = new GenericPrincipal(identity, null);
			return true;
		}

		private static bool TryGetEmailAddressInClaimSets(ReadOnlyCollection<ClaimSet> claimSets, out SmtpAddress smtpAddress)
		{
			smtpAddress = SmtpAddress.Empty;
			foreach (ClaimSet claimSet in claimSets)
			{
				foreach (Claim claim in claimSet.FindClaims("http://schemas.xmlsoap.org/claims/EmailAddress", Rights.PossessProperty))
				{
					string text = claim.Resource as string;
					if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text))
					{
						smtpAddress = new SmtpAddress(text);
						return true;
					}
				}
			}
			return false;
		}

		private bool IsDelegationToken(ReadOnlyCollection<ClaimSet> claimSets)
		{
			return claimSets.PossessClaimType("http://schemas.microsoft.com/ws/2006/04/identity/claims/ThirdPartyRequested");
		}

		private bool InternalCheckAccessCore(EwsOperationContextBase operationContext)
		{
			HttpContext httpContext = HttpContext.Current;
			HttpApplication applicationInstance = httpContext.ApplicationInstance;
			string text = operationContext.RequestMessage.Headers.Action;
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(text.LastIndexOf('/') + 1);
				RequestDetailsLogger.Current.ActivityScope.Action = text;
			}
			if (httpContext.User is GenericPrincipal && httpContext.User.Identity is GenericIdentity)
			{
				IIdentity identity = httpContext.User.Identity;
				if (!(identity is WindowsIdentity) && !(identity is OAuthIdentity) && !(identity is ClientSecurityContextIdentity))
				{
					EWSAuthorizationManager.Return403ForbiddenResponse(operationContext, "User is a unresolved GenericPrincipal.");
					return false;
				}
			}
			if (httpContext.User is GenericPrincipal && httpContext.User.Identity is OAuthIdentity)
			{
				OAuthIdentity oauthIdentity = httpContext.User.Identity as OAuthIdentity;
				if (oauthIdentity.OAuthApplication.ApplicationType == OAuthApplicationType.V1App && (!oauthIdentity.IsAppOnly || (oauthIdentity.IsAppOnly && !oauthIdentity.OAuthApplication.V1ProfileApp.ContainsFullAccessRole)))
				{
					MSDiagnosticsHeader.AppendInvalidOAuthTokenExceptionToBackendResponse(httpContext, new InvalidOAuthTokenException(OAuthErrors.NotEnoughGrantPresented, null, null));
					EWSAuthorizationManager.Return403ForbiddenResponse(operationContext, oauthIdentity.OAuthApplication.V1ProfileApp.Scope);
					return false;
				}
			}
			if (!httpContext.Request.IsAuthenticated)
			{
				if (ServiceSecurityContext.Current == null)
				{
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "ServiceSecurityContext.Current was null");
					return false;
				}
				AuthorizationContext authorizationContext = ServiceSecurityContext.Current.AuthorizationContext;
				if (authorizationContext == null)
				{
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "authContext was null");
					return false;
				}
				if (authorizationContext.ClaimSets == null)
				{
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "authContext.ClaimSets was null");
					return false;
				}
				if (authorizationContext.ClaimSets.Count == 0)
				{
					EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "authContext.ClaimSets.Count was 0");
					return false;
				}
				Uri localAddressUri = operationContext.LocalAddressUri;
				if (this.IsDelegationToken(authorizationContext.ClaimSets))
				{
					if (!EWSAuthorizationManager.CheckClaimSetsForExternalUser(authorizationContext, operationContext))
					{
						return false;
					}
				}
				else if (EWSSettings.IsWsSecurityX509CertAddress(localAddressUri))
				{
					if (!EWSAuthorizationManager.CheckClaimSetsForX509CertUser(authorizationContext, operationContext))
					{
						return false;
					}
				}
				else
				{
					if (!VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.LogonViaStandardTokens.Enabled)
					{
						EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "No login via standard token on-premises");
						return false;
					}
					if (EWSSettings.IsWsSecuritySymmetricKeyAddress(localAddressUri))
					{
						if (!EWSAuthorizationManager.CheckClaimSetsForPartnerUser(authorizationContext, operationContext))
						{
							return false;
						}
					}
					else
					{
						if (!EWSSettings.IsWsSecurityAddress(localAddressUri))
						{
							return false;
						}
						if (!EWSAuthorizationManager.CheckClaimSets(operationContext, authorizationContext.ClaimSets))
						{
							return false;
						}
					}
				}
			}
			CallContext.PushUserPuidToHttpContext();
			return base.CheckAccessCore(operationContext.BackingOperationContext);
		}

		internal static void Return403ForbiddenResponse(EwsOperationContextBase operationContext, string reason)
		{
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>(0L, "Returning a 403 Forbidden response for reason: {0}", reason);
			EWSAuthorizationManager.ReturnStatusCodeResponse(operationContext, HttpStatusCode.Forbidden, reason);
		}

		internal static void Return401UnauthorizedResponse(EwsOperationContextBase operationContext, string reason)
		{
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>(0L, "Returning a 401 Unauthorized response for reason: {0}", reason);
			EWSAuthorizationManager.ReturnStatusCodeResponse(operationContext, HttpStatusCode.Unauthorized, reason);
		}

		internal static void ReturnStatusCodeResponse(EwsOperationContextBase operationContext, HttpStatusCode httpStatusCode, string reason)
		{
			RequestDetailsLogger.Current.AppendAuthError("AuthError", reason);
			EWSSettings.FaultExceptionDueToAuthorizationManager = true;
			object obj = null;
			if (!operationContext.OutgoingMessageProperties.TryGetValue(HttpResponseMessageProperty.Name, out obj))
			{
				ExTraceGlobals.AuthenticationTracer.TraceError(0L, "OutgoingMessageProperties.TryGetValue(HttpResponseMessageProperty) returned false.");
			}
			HttpResponseMessageProperty httpResponseMessageProperty;
			if (obj == null)
			{
				httpResponseMessageProperty = new HttpResponseMessageProperty();
				operationContext.OutgoingMessageProperties.Add(HttpResponseMessageProperty.Name, httpResponseMessageProperty);
			}
			else
			{
				httpResponseMessageProperty = (HttpResponseMessageProperty)obj;
			}
			httpResponseMessageProperty.StatusCode = httpStatusCode;
			httpResponseMessageProperty.SuppressEntityBody = true;
		}

		internal const string AuthType = "AuthType";

		internal const string X509CertAuth = "X509Cert";

		internal const string ExternalAuth = "External";

		internal const string PartnerAuth = "Partner";

		internal const string LiveIdTokenAuth = "LiveIdToken";

		internal const string LiveIdBasic = "LiveIdBasic";

		internal const string LiveIdTokenSmtpClaim = "LiveIdTokenSmtpClaim";

		private const string SamlEmailAddress = "http://schemas.xmlsoap.org/claims/EmailAddress";

		private const string PUIDClaimType = "http://schemas.xmlsoap.org/claims/PUID";

		private const string ConsumerPUIDClaimType = "http://schemas.xmlsoap.org/claims/ConsumerPUID";

		private const string ChildClaimType = "http://schemas.xmlsoap.org/claims/Child";

		private const string ConsumerChildClaimType = "http://schemas.xmlsoap.org/claims/ConsumerChild";

		private const string ConsentLevelClaimType = "http://schemas.xmlsoap.org/claims/ConsentLevel";

		private const string ConsumerConsentLevelClaimType = "http://schemas.xmlsoap.org/claims/ConsumerConsentLevel";

		private const string TOUAcceptedClaimType = "http://schemas.xmlsoap.org/claims/TOUAccepted";

		private const string ConsumerTOUAcceptedClaimType = "http://schemas.xmlsoap.org/claims/ConsumerTOUAccepted";

		private const string ConsentLevelNone = "NONE";

		private const string ConsentLevelPartial = "PARTIAL";

		private const string ConsentLevelFull = "FULL";

		private const string TrueAllCapitals = "TRUE";

		private const string FalseAllCapitals = "FALSE";

		private enum ConsentLevel
		{
			None,
			Partial,
			Full
		}
	}
}
