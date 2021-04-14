using System;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class UserContextUtilities
	{
		public static bool IsHexChar(char c)
		{
			return char.IsDigit(c) || (char.ToUpperInvariant(c) >= 'A' && char.ToUpperInvariant(c) <= 'F');
		}

		public static string GetNewGuid()
		{
			return Guid.NewGuid().ToString("N");
		}

		public static bool IsValidGuid(string guid)
		{
			if (guid == null || guid.Length != 32)
			{
				return false;
			}
			for (int i = 0; i < 32; i++)
			{
				if (!UserContextUtilities.IsHexChar(guid[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static byte[] ValidTokenBase64Decode(string tokenValidBase64String)
		{
			if (tokenValidBase64String == null)
			{
				throw new ArgumentNullException("tokenValidBase64String");
			}
			long num = (long)tokenValidBase64String.Length;
			if (tokenValidBase64String.Length % 4 != 0)
			{
				num += (long)(4 - tokenValidBase64String.Length % 4);
			}
			char[] array = new char[num];
			tokenValidBase64String.CopyTo(0, array, 0, tokenValidBase64String.Length);
			for (long num2 = 0L; num2 < (long)tokenValidBase64String.Length; num2 += 1L)
			{
				checked
				{
					if (array[(int)((IntPtr)num2)] == '-')
					{
						array[(int)((IntPtr)num2)] = '\\';
					}
				}
			}
			for (long num3 = (long)tokenValidBase64String.Length; num3 < (long)array.Length; num3 += 1L)
			{
				array[(int)(checked((IntPtr)num3))] = '=';
			}
			return Convert.FromBase64CharArray(array, 0, array.Length);
		}

		public static string ValidTokenBase64Encode(byte[] byteArray)
		{
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			int num = (int)(1.3333333333333333 * (double)byteArray.Length);
			if (num % 4 != 0)
			{
				num += 4 - num % 4;
			}
			char[] array = new char[num];
			Convert.ToBase64CharArray(byteArray, 0, byteArray.Length, array, 0);
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '\\')
				{
					array[i] = '-';
				}
				else if (array[i] == '=')
				{
					num2++;
				}
			}
			return new string(array, 0, array.Length - num2);
		}

		public static bool IsDifferentMailbox(HttpContext httpContext)
		{
			return !string.IsNullOrEmpty(UserContextUtilities.GetExplicitLogonUser(httpContext));
		}

		public static string GetExplicitLogonUser(HttpContext httpContext)
		{
			string text = HttpUtility.UrlDecode(httpContext.Request.Headers["X-OWA-ExplicitLogonUser"]);
			if (text == null)
			{
				text = string.Empty;
			}
			return text;
		}

		public static bool IsFlagSet(int valueToTest, int flag)
		{
			return (valueToTest & flag) == flag;
		}

		internal static void DisconnectStoreSession(StoreSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (session.IsConnected)
			{
				session.Disconnect();
			}
		}

		internal static bool ReconnectStoreSession(StoreSession session, IMailboxContext userContext)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			bool flag = false;
			MailboxSession mailboxSession = session as MailboxSession;
			if (!session.IsConnected && mailboxSession != null)
			{
				flag = mailboxSession.ConnectWithStatus();
				if (flag && userContext.NotificationManager != null)
				{
					userContext.NotificationManager.HandleConnectionDroppedNotification();
				}
			}
			return flag;
		}

		internal static ADSessionSettings CreateScopedSessionSettings(string domainName, OrganizationId orgId)
		{
			ADSessionSettings result;
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				try
				{
					if (orgId != null)
					{
						result = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId);
					}
					else
					{
						if (domainName == null)
						{
							throw new ArgumentNullException("domainName");
						}
						result = ADSessionSettings.FromTenantAcceptedDomain(domainName);
					}
					return result;
				}
				catch (CannotResolveTenantNameException)
				{
					string arg = (domainName != null) ? domainName.ToString() : "<null>";
					string arg2 = (orgId != null) ? orgId.ToString() : "<null>";
					string message = string.Format("Could not resolve recipient session for domain:'{0}', organization id:'{1}'", arg, arg2);
					ExTraceGlobals.UserContextTracer.TraceError(0L, message);
					throw new OwaADObjectNotFoundException(message);
				}
				catch (DataSourceOperationException innerException)
				{
					string arg3 = (domainName != null) ? domainName.ToString() : "<null>";
					string arg4 = (orgId != null) ? orgId.ToString() : "<null>";
					string message2 = string.Format("DataSourceException for domain:'{0}', organization id:'{1}'", arg3, arg4);
					ExTraceGlobals.UserContextTracer.TraceError(0L, message2);
					throw new OwaADObjectNotFoundException(message2, innerException);
				}
			}
			result = ADSessionSettings.FromRootOrgScopeSet();
			return result;
		}

		internal static IRecipientSession CreateScopedRecipientSession(bool readOnly, ConsistencyMode consistencyMode, string domain, OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = UserContextUtilities.CreateScopedSessionSettings(domain, orgId);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, readOnly, consistencyMode, null, sessionSettings, 350, "CreateScopedRecipientSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\common\\UserContextUtilities.cs");
		}

		internal static IConfigurationSession CreateADSystemConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, UserContext userContext, IBudget budget)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			adsessionSettings.AccountingObject = budget;
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(readOnly, consistencyMode, adsessionSettings, 379, "CreateADSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\common\\UserContextUtilities.cs");
		}

		internal static IRecipientSession CreateADRecipientSession(int lcid, bool readOnly, ConsistencyMode consistencyMode, bool useDirectorySearchRoot, UserContext userContext, bool scopeToGal, IBudget budget)
		{
			ADSessionSettings adsessionSettings;
			if (scopeToGal)
			{
				adsessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(userContext.ExchangePrincipal.MailboxInfo.OrganizationId, (userContext.GetGlobalAddressList(budget) != null) ? userContext.GetGlobalAddressList(budget).Id : null);
			}
			else if (userContext.ExchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy != null)
			{
				adsessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(userContext.ExchangePrincipal.MailboxInfo.OrganizationId, userContext.GlobalAddressListId);
			}
			else
			{
				adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
			}
			adsessionSettings.AccountingObject = budget;
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, useDirectorySearchRoot ? userContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN : null, lcid, readOnly, consistencyMode, null, adsessionSettings, 436, "CreateADRecipientSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\common\\UserContextUtilities.cs");
		}

		public static bool IsPublicRequest(HttpRequest request)
		{
			if (VdirConfiguration.Instance.FormsAuthenticationEnabled)
			{
				HttpCookie httpCookie = request.Cookies["PrivateComputer"];
				if (httpCookie == null || string.Compare(httpCookie.Value, "true", StringComparison.OrdinalIgnoreCase) != 0)
				{
					return true;
				}
			}
			else if (request.Headers["X-LogonType"] != null)
			{
				return string.CompareOrdinal(request.Headers["X-LogonType"], "Public") == 0;
			}
			return false;
		}

		public static bool IsPublicComputerSession(HttpContext httpContext)
		{
			LiveIDIdentity liveIDIdentity = httpContext.User.Identity as LiveIDIdentity;
			AdfsIdentity adfsIdentity = httpContext.User.Identity as AdfsIdentity;
			bool result;
			if (liveIDIdentity != null)
			{
				string text = httpContext.Request.Headers["X-LoginAttributes"];
				if (string.IsNullOrWhiteSpace(text))
				{
					if (liveIDIdentity.LoginAttributes != null)
					{
						result = !liveIDIdentity.LoginAttributes.IsInsideCorpnetSession;
						ExTraceGlobals.CoreTracer.TraceError<uint, bool>(0L, "[UserContextUtilities::IsPublicComputerSession] session is a live identity session. LoginAttributes header is NULL. LoginAttributes in the identity is {0}, IsInsideCorpnetSession = {1}.", liveIDIdentity.LoginAttributes.Value, liveIDIdentity.LoginAttributes.IsInsideCorpnetSession);
					}
					else
					{
						result = true;
						ExTraceGlobals.CoreTracer.TraceError(0L, "[UserContextUtilities::IsPublicComputerSession] session is a live identity session. LoginAttributes header is NULL and identity.LoginAttributes is also NULL. Defaulting to public.");
					}
				}
				else
				{
					LiveIdLoginAttributes liveIdLoginAttributes = new LiveIdLoginAttributes(Convert.ToUInt32(text));
					result = !liveIdLoginAttributes.IsInsideCorpnetSession;
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "[UserContextUtilities::IsPublicComputerSession] session is a live identity session. LoginAttributes header value is {0}, LoginAttributes = {1}, IsInsideCorpnetSession = {2}, LoginAttributes in the identity is {3}, IsInsideCorpnetSession = {4}", new object[]
					{
						text,
						liveIdLoginAttributes.Value,
						liveIdLoginAttributes.IsInsideCorpnetSession,
						(liveIDIdentity.LoginAttributes != null) ? liveIDIdentity.LoginAttributes.Value.ToString() : string.Empty,
						(liveIDIdentity.LoginAttributes != null) ? liveIDIdentity.LoginAttributes.IsInsideCorpnetSession.ToString() : string.Empty
					});
					httpContext.Response.AppendToLog("&loginAttributesBE=" + text);
				}
			}
			else if (adfsIdentity != null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<bool>(0L, "[UserContextUtilities::IsPublicComputerSession] session is a ADFS identity session is public computer: {0}.", adfsIdentity.IsPublicSession);
				result = adfsIdentity.IsPublicSession;
			}
			else
			{
				result = true;
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "[SessionSettingsType::SetPublicComputerSession] session is NOT a live identity nor an ADFS identity session. Hence, defaulting to public computer session.");
			}
			return result;
		}

		public static bool IsPublicLogon(OrganizationId organizationId, HttpContext httpContext)
		{
			bool result;
			try
			{
				OwaOrgConfigData orgConfigTypeFromAd = UserContextUtilities.GetOrgConfigTypeFromAd(organizationId);
				result = (UserContextUtilities.IsPublicRequest(httpContext.Request) || (orgConfigTypeFromAd.PublicComputersDetectionEnabled && UserContextUtilities.IsPublicComputerSession(httpContext)));
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<string, string>(0L, "Failed to Get IsPublicLogon. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
				throw;
			}
			return result;
		}

		public static OwaOrgConfigData GetOrgConfigTypeFromAd(OrganizationId organizationId)
		{
			OwaOrgConfigData configuration = new OwaOrgConfigData
			{
				MailTipsLargeAudienceThreshold = 25U,
				PublicComputersDetectionEnabled = false
			};
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 608, "GetOrgConfigTypeFromAd", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\common\\UserContextUtilities.cs");
				ADOrganizationConfig[] array = tenantOrTopologyConfigurationSession.Find<ADOrganizationConfig>(null, QueryScope.SubTree, null, null, 1);
				if (array != null && array.Length > 0)
				{
					configuration.MailTipsLargeAudienceThreshold = array[0].MailTipsLargeAudienceThreshold;
					configuration.PublicComputersDetectionEnabled = array[0].PublicComputersDetectionEnabled;
				}
			}, 3);
			return configuration;
		}

		internal static bool IsSharePointAppRequest(HttpRequest request)
		{
			bool result = false;
			if (request != null)
			{
				bool.TryParse(request.QueryString["sharepointapp"], out result);
			}
			return result;
		}

		internal static bool TryParseUrlReferrer(this HttpRequest request, out Uri referrer)
		{
			referrer = null;
			if (request != null)
			{
				try
				{
					referrer = request.UrlReferrer;
				}
				catch (UriFormatException)
				{
				}
			}
			return referrer != null;
		}

		internal static T ReadAggregatedType<T>(UserConfigurationManager.IAggregationContext ctx, string key, Func<T> factory) where T : SerializableDataBase
		{
			return (ctx != null) ? ctx.ReadType<T>(key, factory) : factory();
		}

		private const int GuidLength = 32;
	}
}
