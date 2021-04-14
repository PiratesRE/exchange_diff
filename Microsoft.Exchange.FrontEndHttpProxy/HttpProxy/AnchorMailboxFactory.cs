using System;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class AnchorMailboxFactory
	{
		public static AnchorMailbox CreateFromCaller(IRequestContext requestContext)
		{
			if (requestContext == null)
			{
				throw new ArgumentNullException("requestContext");
			}
			CommonAccessToken commonAccessToken = requestContext.HttpContext.Items["Item-CommonAccessToken"] as CommonAccessToken;
			if (commonAccessToken != null)
			{
				AnchorMailbox anchorMailbox = AnchorMailboxFactory.TryCreateFromCommonAccessToken(commonAccessToken, requestContext);
				if (anchorMailbox != null)
				{
					return anchorMailbox;
				}
			}
			if (requestContext.HttpContext.User == null || requestContext.HttpContext.User.Identity == null)
			{
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "UnauthenticatedRequest-RandomBackEnd");
				return new AnonymousAnchorMailbox(requestContext);
			}
			WindowsIdentity windowsIdentity = requestContext.HttpContext.User.Identity as WindowsIdentity;
			if (windowsIdentity != null && windowsIdentity.User == null)
			{
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "AnonymousRequest-RandomBackEnd");
				return new AnonymousAnchorMailbox(requestContext);
			}
			return AnchorMailboxFactory.CreateFromLogonIdentity(requestContext);
		}

		public static AnchorMailbox TryCreateFromRoutingHint(IRequestContext requestContext, bool tryTargetServerRoutingHint)
		{
			if (requestContext == null)
			{
				throw new ArgumentNullException("requestContext");
			}
			string text = requestContext.HttpContext.Request.Headers[WellKnownHeader.XTargetServer];
			if (!string.IsNullOrEmpty(text) && tryTargetServerRoutingHint)
			{
				requestContext.Logger.Set(HttpProxyMetadata.RoutingHint, "TargetServerHeader");
				return new ServerInfoAnchorMailbox(text, requestContext);
			}
			string text2 = requestContext.HttpContext.Request.Headers[Constants.AnchorMailboxHeaderName];
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			Match match = RegexUtilities.TryMatch(Constants.SidRegex, text2, requestContext.Logger);
			if (match != null && match.Success)
			{
				string text3 = RegexUtilities.ParseIdentifier(match, "${sid}", requestContext.Logger);
				if (!string.IsNullOrEmpty(text3))
				{
					SecurityIdentifier securityIdentifier = null;
					try
					{
						securityIdentifier = new SecurityIdentifier(text3);
					}
					catch (ArgumentException ex)
					{
						requestContext.Logger.AppendGenericError("Ignored Exception", ex.ToString());
					}
					catch (SystemException ex2)
					{
						requestContext.Logger.AppendGenericError("Ignored Exception", ex2.ToString());
					}
					if (securityIdentifier != null)
					{
						requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "AnchorMailboxHeader-SID");
						return new SidAnchorMailbox(securityIdentifier, requestContext);
					}
				}
			}
			match = RegexUtilities.TryMatch(Constants.GuidAtDomainRegex, text2, requestContext.Logger);
			if (match != null && match.Success)
			{
				string text4 = RegexUtilities.ParseIdentifier(match, "${mailboxguid}", requestContext.Logger);
				if (!string.IsNullOrEmpty(text4))
				{
					Guid mailboxGuid = new Guid(text4);
					string text5 = RegexUtilities.ParseIdentifier(match, "${domain}", requestContext.Logger);
					string value = string.Format("AnchorMailboxHeader-MailboxGuid{0}", string.IsNullOrEmpty(text5) ? string.Empty : "WithDomain");
					requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, value);
					MailboxGuidAnchorMailbox mailboxGuidAnchorMailbox = new MailboxGuidAnchorMailbox(mailboxGuid, text5, requestContext);
					if (!string.IsNullOrEmpty(text5))
					{
						mailboxGuidAnchorMailbox.FallbackSmtp = text2;
					}
					return mailboxGuidAnchorMailbox;
				}
			}
			if (SmtpAddress.IsValidSmtpAddress(text2))
			{
				requestContext.Logger.Set(HttpProxyMetadata.RoutingHint, "AnchorMailboxHeader-SMTP");
				return new SmtpAnchorMailbox(text2, requestContext);
			}
			return null;
		}

		public static bool TryCreateFromMailboxGuid(IRequestContext requestContext, string anchorMailboxAddress, out AnchorMailbox anchorMailbox)
		{
			anchorMailbox = null;
			Guid mailboxGuid;
			string domain;
			if (AnchorMailboxFactory.TryGetMailboxGuid(anchorMailboxAddress, out mailboxGuid, out domain, requestContext))
			{
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "URL-MailboxGuidWithDomain");
				anchorMailbox = new MailboxGuidAnchorMailbox(mailboxGuid, domain, requestContext);
				return true;
			}
			return false;
		}

		public static AnchorMailbox CreateFromSamlTokenAddress(string address, IRequestContext requestContext)
		{
			if (string.IsNullOrEmpty(address))
			{
				string text = string.Format("Cannot authenticate user address claim {0}", address);
				requestContext.Logger.AppendGenericError("Invalid Wssecurity address claim.", text);
				throw new HttpProxyException(HttpStatusCode.Unauthorized, HttpProxySubErrorCode.BadSamlToken, text);
			}
			if (SmtpAddress.IsValidSmtpAddress(address))
			{
				requestContext.Logger.Set(HttpProxyMetadata.RoutingHint, "WSSecurityRequest-SMTP");
				return new SmtpAnchorMailbox(address, requestContext)
				{
					FailOnDomainNotFound = false
				};
			}
			Match match = RegexUtilities.TryMatch(Constants.SidOnlyRegex, address, requestContext.Logger);
			if (match != null && match.Success)
			{
				SecurityIdentifier securityIdentifier = null;
				try
				{
					securityIdentifier = new SecurityIdentifier(address);
				}
				catch (ArgumentException ex)
				{
					requestContext.Logger.AppendGenericError("Ignored Exception", ex.ToString());
				}
				catch (SystemException ex2)
				{
					requestContext.Logger.AppendGenericError("Ignored Exception", ex2.ToString());
				}
				if (securityIdentifier != null)
				{
					requestContext.Logger.Set(HttpProxyMetadata.RoutingHint, "WSSecurityRequest-SID");
					return new SidAnchorMailbox(securityIdentifier, requestContext);
				}
			}
			if (SmtpAddress.IsValidDomain(address))
			{
				requestContext.Logger.Set(HttpProxyMetadata.RoutingHint, "WSSecurityRequest-Domain");
				return new DomainAnchorMailbox(address, requestContext);
			}
			throw new HttpProxyException(HttpStatusCode.Unauthorized, HttpProxySubErrorCode.BadSamlToken, string.Format("Cannot authenticate user address claim {0}", address));
		}

		private static bool TryGetMailboxGuid(string anchorMailboxAddress, out Guid mailboxGuid, out string domain, IRequestContext requestContext)
		{
			mailboxGuid = default(Guid);
			domain = null;
			if (anchorMailboxAddress != null)
			{
				Match match = RegexUtilities.TryMatch(Constants.GuidAtDomainRegex, anchorMailboxAddress, requestContext.Logger);
				if (match != null && match.Success)
				{
					string text = RegexUtilities.ParseIdentifier(match, "${mailboxguid}", requestContext.Logger);
					if (!string.IsNullOrEmpty(text))
					{
						mailboxGuid = new Guid(text);
						domain = RegexUtilities.ParseIdentifier(match, "${domain}", requestContext.Logger);
						return true;
					}
				}
			}
			return false;
		}

		private static AnchorMailbox TryCreateFromCommonAccessToken(CommonAccessToken cat, IRequestContext requestContext)
		{
			AccessTokenType accessTokenType = (AccessTokenType)Enum.Parse(typeof(AccessTokenType), cat.TokenType, true);
			if (accessTokenType == AccessTokenType.CompositeIdentity)
			{
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "CommonAccessToken-CompositeIdentity");
				cat = CommonAccessToken.Deserialize(cat.ExtensionData["PrimaryIdentityToken"]);
				accessTokenType = (AccessTokenType)Enum.Parse(typeof(AccessTokenType), cat.TokenType, true);
			}
			switch (accessTokenType)
			{
			case AccessTokenType.Windows:
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "CommonAccessToken-Windows");
				return new SidAnchorMailbox(cat.WindowsAccessToken.UserSid, requestContext);
			case AccessTokenType.LiveId:
			{
				LiveIdFbaTokenAccessor liveIdFbaTokenAccessor = LiveIdFbaTokenAccessor.Attach(cat);
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "CommonAccessToken-LiveId");
				return new SidAnchorMailbox(liveIdFbaTokenAccessor.UserSid, requestContext)
				{
					OrganizationId = liveIdFbaTokenAccessor.OrganizationId,
					SmtpOrLiveId = liveIdFbaTokenAccessor.LiveIdMemberName
				};
			}
			case AccessTokenType.LiveIdBasic:
			{
				LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Attach(cat);
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "CommonAccessToken-LiveIdBasic");
				if (liveIdBasicTokenAccessor.UserSid != null)
				{
					return new SidAnchorMailbox(liveIdBasicTokenAccessor.UserSid, requestContext)
					{
						OrganizationId = liveIdBasicTokenAccessor.OrganizationId,
						SmtpOrLiveId = liveIdBasicTokenAccessor.LiveIdMemberName
					};
				}
				return new PuidAnchorMailbox(liveIdBasicTokenAccessor.Puid, liveIdBasicTokenAccessor.LiveIdMemberName, requestContext);
			}
			case AccessTokenType.LiveIdNego2:
			{
				string sid = cat.ExtensionData["UserSid"];
				string value;
				cat.ExtensionData.TryGetValue("OrganizationName", out value);
				string smtpOrLiveId;
				cat.ExtensionData.TryGetValue("MemberName", out smtpOrLiveId);
				if (!string.IsNullOrEmpty(value) && requestContext.Logger != null)
				{
					requestContext.Logger.ActivityScope.SetProperty(ActivityStandardMetadata.TenantId, value);
				}
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "CommonAccessToken-LiveIdNego2");
				return new SidAnchorMailbox(sid, requestContext)
				{
					SmtpOrLiveId = smtpOrLiveId
				};
			}
			case AccessTokenType.OAuth:
				return null;
			case AccessTokenType.Adfs:
				return null;
			case AccessTokenType.CertificateSid:
			{
				ADRawEntry httpContextADRawEntry = AuthCommon.GetHttpContextADRawEntry(requestContext.HttpContext);
				if (httpContextADRawEntry != null)
				{
					requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "CommonAccessToken-CertificateSid");
					return new UserADRawEntryAnchorMailbox(httpContextADRawEntry, requestContext);
				}
				CertificateSidTokenAccessor certificateSidTokenAccessor = CertificateSidTokenAccessor.Attach(cat);
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "CommonAccessToken-CertificateSid");
				return new SidAnchorMailbox(certificateSidTokenAccessor.UserSid, requestContext)
				{
					PartitionId = certificateSidTokenAccessor.PartitionId
				};
			}
			case AccessTokenType.RemotePowerShellDelegated:
				return null;
			}
			return null;
		}

		private static AnchorMailbox CreateFromLogonIdentity(IRequestContext requestContext)
		{
			HttpContext httpContext = requestContext.HttpContext;
			IPrincipal user = httpContext.User;
			IIdentity identity = httpContext.User.Identity;
			string text = httpContext.Items[Constants.WLIDMemberName] as string;
			OAuthIdentity oauthIdentity = identity as OAuthIdentity;
			if (oauthIdentity != null)
			{
				string text2 = httpContext.Request.Headers[Constants.ExternalDirectoryObjectIdHeaderName];
				if (!string.IsNullOrEmpty(text2))
				{
					requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "OAuthIdentity-ExternalDirectoryObjectId");
					return new ExternalDirectoryObjectIdAnchorMailbox(text2, oauthIdentity.OrganizationId, requestContext);
				}
				if (oauthIdentity.ActAsUser != null)
				{
					requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "OAuthIdentity-ActAsUser");
					return new OAuthActAsUserAnchorMailbox(oauthIdentity.ActAsUser, requestContext);
				}
				requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "OAuthIdentity-AppOrganization");
				return new OrganizationAnchorMailbox(oauthIdentity.OrganizationId, requestContext);
			}
			else
			{
				GenericSidIdentity genericSidIdentity = identity as GenericSidIdentity;
				if (genericSidIdentity != null)
				{
					requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "GenericSidIdentity");
					return new SidAnchorMailbox(genericSidIdentity.Sid, requestContext)
					{
						PartitionId = genericSidIdentity.PartitionId,
						SmtpOrLiveId = text
					};
				}
				DelegatedPrincipal delegatedPrincipal = user as DelegatedPrincipal;
				if (delegatedPrincipal != null && delegatedPrincipal.DelegatedOrganization != null && string.IsNullOrEmpty(text))
				{
					requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "DelegatedPrincipal-DelegatedOrganization");
					return new DomainAnchorMailbox(delegatedPrincipal.DelegatedOrganization, requestContext);
				}
				WindowsIdentity windowsIdentity = identity as WindowsIdentity;
				if (windowsIdentity != null)
				{
					if (string.IsNullOrEmpty(text))
					{
						requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "WindowsIdentity");
					}
					else
					{
						requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "WindowsIdentity-LiveIdMemberName");
					}
					return new SidAnchorMailbox(windowsIdentity.User, requestContext)
					{
						SmtpOrLiveId = text
					};
				}
				try
				{
					SecurityIdentifier securityIdentifier = identity.GetSecurityIdentifier();
					if (!securityIdentifier.Equals(AuthCommon.MemberNameNullSid))
					{
						if (string.IsNullOrEmpty(text))
						{
							requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "SID");
						}
						else
						{
							requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "SID-LiveIdMemberName");
						}
						return new SidAnchorMailbox(securityIdentifier, requestContext)
						{
							SmtpOrLiveId = text
						};
					}
				}
				catch (Exception)
				{
				}
				if (requestContext.AuthBehavior.AuthState != AuthState.FrontEndFullAuth)
				{
					AnchorMailbox anchorMailbox = requestContext.AuthBehavior.CreateAuthModuleSpecificAnchorMailbox(requestContext);
					if (anchorMailbox != null)
					{
						return anchorMailbox;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					requestContext.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "Smtp-LiveIdMemberName");
					return new SmtpAnchorMailbox(text, requestContext);
				}
				throw new InvalidOperationException(string.Format("Unknown idenity {0} with type {1}.", identity.GetSafeName(true), identity.ToString()));
			}
		}
	}
}
