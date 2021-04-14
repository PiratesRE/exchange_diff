using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class CompositeIdentityAuthenticationHelper
	{
		private CompositeIdentityAuthenticationHelper(CommonAccessToken orgIdCat)
		{
			if (orgIdCat == null)
			{
				throw new ArgumentNullException("orgIdCat", "You must specify the previously obtained ORGID CommonAccessToken (must not be NULL)!");
			}
			this.orgIdCat = orgIdCat;
			this.IsMsaIdentityRequired = false;
			this.MsaMemberName = null;
		}

		public static CompositeIdentityAuthenticationHelper GetInstance(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext", "You must specify the current HTTP context. This parameter must not be NULL.");
			}
			if (!httpContext.Request.IsAuthenticated || httpContext.Items["Item-CommonAccessToken"] == null)
			{
				return null;
			}
			CompositeIdentityAuthenticationHelper.SelectPrimaryIdentityMode primaryIdentitySelection = CompositeIdentityAuthenticationHelper.GetPrimaryIdentitySelection(httpContext);
			CompositeIdentityAuthenticationHelper compositeIdentityAuthenticationHelper = new CompositeIdentityAuthenticationHelper(httpContext.Items["Item-CommonAccessToken"] as CommonAccessToken);
			string msaMemberName;
			if (CompositeIdentityAuthenticationHelper.TryReadMsaUserHintCookie(httpContext, out msaMemberName))
			{
				compositeIdentityAuthenticationHelper.MsaMemberName = msaMemberName;
			}
			compositeIdentityAuthenticationHelper.IsMsaIdentityRequired = (primaryIdentitySelection == CompositeIdentityAuthenticationHelper.SelectPrimaryIdentityMode.ANY_MSA);
			return compositeIdentityAuthenticationHelper;
		}

		public string MsaMemberName { get; private set; }

		public bool IsMsaIdentityRequired { get; private set; }

		public static bool IsCompositeIdentityEnabled()
		{
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.CompositeIdentity.Enabled;
		}

		public CommonAccessToken GetCompositeIdentityCat(CommonAccessToken msaCat)
		{
			msaCat.ExtensionData["AuthenticationAuthority"] = AuthenticationAuthority.MSA.ToString();
			CommonAccessToken commonAccessToken = new CommonAccessToken(AccessTokenType.CompositeIdentity);
			commonAccessToken.ExtensionData["PrimaryIdentityToken"] = (this.IsMsaIdentityRequired ? msaCat : this.orgIdCat).Serialize();
			commonAccessToken.ExtensionData["SecondaryIdentityTokensCount"] = Convert.ToInt32(this.IsMsaIdentityRequired).ToString(CultureInfo.InvariantCulture);
			commonAccessToken.ExtensionData["CanaryIdentityIndex"] = (this.IsMsaIdentityRequired ? "0" : -1.ToString(CultureInfo.InvariantCulture));
			if (this.IsMsaIdentityRequired)
			{
				commonAccessToken.ExtensionData[string.Format(CultureInfo.InvariantCulture, "SecondaryIdentityToken{0}", new object[]
				{
					0
				})] = this.orgIdCat.Serialize();
			}
			return commonAccessToken;
		}

		public void SetMsaUserHintCookie(HttpContext httpContext, CommonAccessToken msaCat, string memberName)
		{
			HttpCookie httpCookie = new HttpCookie("MsaUserHint");
			httpCookie.HttpOnly = true;
			httpCookie.Path = "/";
			httpCookie.Value = memberName;
			httpCookie.Domain = CompositeIdentityAuthenticationHelper.GetOutlookDotComDomain(httpContext.Request.Url.Host);
			httpContext.Response.Cookies.Add(httpCookie);
		}

		public void FixLiveIdBasicIdentity(HttpContext httpContext, CommonAccessToken msaCat)
		{
			LiveIdBasicTokenAccessor liveIdBasicTokenAccessor = LiveIdBasicTokenAccessor.Attach(msaCat);
			if (liveIdBasicTokenAccessor.TokenType == AccessTokenType.LiveIdBasic)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromTenantMSAUser(liveIdBasicTokenAccessor.Puid);
				ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 217, "FixLiveIdBasicIdentity", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\CompositeIdentityAuthenticationHelper.cs");
				ADRawEntry adrawEntry = tenantRecipientSession.FindByNetID(liveIdBasicTokenAccessor.Puid, new PropertyDefinition[]
				{
					ADMailboxRecipientSchema.Sid
				}).FirstOrDefault<ADRawEntry>();
				if (adrawEntry != null)
				{
					SecurityIdentifier securityIdentifier = adrawEntry[ADMailboxRecipientSchema.Sid] as SecurityIdentifier;
					OrganizationId organizationId = (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
					httpContext.User = new GenericPrincipal(new GenericSidIdentity(securityIdentifier.ToString(), "LiveIdBasic", securityIdentifier, organizationId.PartitionId.ToString()), null);
				}
			}
		}

		public static bool IsUnifiedMailboxRequest(HttpContext httpContext)
		{
			return CompositeIdentityAuthenticationHelper.GetPrimaryIdentitySelection(httpContext) == CompositeIdentityAuthenticationHelper.SelectPrimaryIdentityMode.ANY_MSA;
		}

		private static bool TryReadMsaUserHintCookie(HttpContext httpContext, out string msaMemberName)
		{
			msaMemberName = null;
			HttpCookie httpCookie = httpContext.Request.Cookies["MsaUserHint"];
			if (httpCookie != null)
			{
				msaMemberName = httpCookie.Value;
				if (!string.IsNullOrWhiteSpace(msaMemberName))
				{
					return true;
				}
				msaMemberName = null;
				CompositeIdentityAuthenticationHelper.DeleteCookie(httpContext, "MsaUserHint");
			}
			return false;
		}

		private static void DeleteCookie(HttpContext httpContext, string cookieName)
		{
			if (string.IsNullOrEmpty(cookieName))
			{
				throw new ArgumentException("Cookie name can not be null or empty string", "cookieName");
			}
			bool flag = false;
			if (httpContext.Response != null && httpContext.Response.Cookies != null)
			{
				for (int i = 0; i < httpContext.Response.Cookies.Count; i++)
				{
					HttpCookie httpCookie = httpContext.Response.Cookies[i];
					if (httpCookie.Name != null && string.Equals(httpCookie.Name, cookieName, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				httpContext.Response.Cookies.Add(new HttpCookie(cookieName, string.Empty));
			}
			httpContext.Response.Cookies[cookieName].Expires = DateTime.UtcNow.AddYears(-30);
		}

		private static string GetOutlookDotComDomain(string hostName)
		{
			if (string.IsNullOrEmpty(hostName))
			{
				return string.Empty;
			}
			int num = hostName.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
			if (num < 0)
			{
				return hostName;
			}
			int num2 = hostName.LastIndexOf(".", num - 1, StringComparison.InvariantCultureIgnoreCase);
			if (num2 < 0)
			{
				return hostName;
			}
			return hostName.Substring(num2 + 1);
		}

		private static CompositeIdentityAuthenticationHelper.SelectPrimaryIdentityMode GetPrimaryIdentitySelection(HttpContext httpContext)
		{
			string text = httpContext.Request.Headers["X-PrimaryIdentityId"];
			if (!string.IsNullOrWhiteSpace(text) && string.Compare(text, "MSA", true) == 0)
			{
				return CompositeIdentityAuthenticationHelper.SelectPrimaryIdentityMode.ANY_MSA;
			}
			return CompositeIdentityAuthenticationHelper.SelectPrimaryIdentityMode.ANY_ORGID;
		}

		private const string MsaUserHintCookieName = "MsaUserHint";

		private const string PrimaryIdentityIdHeaderName = "X-PrimaryIdentityId";

		private readonly CommonAccessToken orgIdCat;

		private enum SelectPrimaryIdentityMode
		{
			ANY_MSA,
			ANY_ORGID
		}
	}
}
