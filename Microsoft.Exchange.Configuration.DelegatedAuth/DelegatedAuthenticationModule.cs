using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.DelegatedAuthentication.EventLog;
using Microsoft.Exchange.Configuration.RedirectionModule;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DelegatedAuthentication;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.Compliance;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	public class DelegatedAuthenticationModule : IHttpModule
	{
		static DelegatedAuthenticationModule()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				DelegatedAuthenticationModule.processName = currentProcess.MainModule.ModuleName;
				DelegatedAuthenticationModule.processId = currentProcess.Id;
			}
			DelegatedAuthenticationModule.podRedirectTemplate = ConfigurationManager.AppSettings["PodRedirectTemplate"];
			DelegatedAuthenticationModule.siteRedirectTemplate = ConfigurationManager.AppSettings["SiteRedirectTemplate"];
			int.TryParse(ConfigurationManager.AppSettings["PodSiteStartRange"], out DelegatedAuthenticationModule.podSiteStartRange);
			int.TryParse(ConfigurationManager.AppSettings["PodSiteEndRange"], out DelegatedAuthenticationModule.podSiteEndRange);
			Enum.TryParse<DelegatedAuthenticationModule.Protocol>(ConfigurationManager.AppSettings["DelegatedAutentication.Protocol"], out DelegatedAuthenticationModule.protocol);
			TimeSpan tokenLifetime;
			if (TimeSpan.TryParse(ConfigurationManager.AppSettings["DelegatedAutentication.TokenLifetime"], out tokenLifetime))
			{
				DelegatedSecurityToken.TokenLifetime = tokenLifetime;
			}
			DelegatedAuthenticationModule.appDomainAppVirtualPath = (HttpRuntime.AppDomainAppVirtualPath ?? string.Empty);
			DelegatedAuthenticationModule.isRedirectToLocalServerEnabled = StringComparer.OrdinalIgnoreCase.Equals("True", ConfigurationManager.AppSettings["EnableRedirectToLocalServer"]);
		}

		void IHttpModule.Init(HttpApplication application)
		{
			application.AuthenticateRequest += DelegatedAuthenticationModule.OnAuthenticateRequestHandler;
		}

		void IHttpModule.Dispose()
		{
		}

		internal static byte[] EncryptSecurityToken(string userId, string securityToken)
		{
			byte[] currentSecretKey = GccUtils.DatacenterServerAuthentication.CurrentSecretKey;
			if (currentSecretKey == null)
			{
				throw new CannotResolveCurrentKeyException(true);
			}
			byte[] result;
			using (HMACSHA256Cng hmacsha256Cng = new HMACSHA256Cng(currentSecretKey))
			{
				byte[] key = hmacsha256Cng.ComputeHash(Encoding.UTF8.GetBytes(userId));
				SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create();
				symmetricAlgorithm.Padding = PaddingMode.ISO10126;
				symmetricAlgorithm.Key = key;
				symmetricAlgorithm.IV = GccUtils.DatacenterServerAuthentication.CurrentIVKey;
				MemoryStream memoryStream = new MemoryStream();
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(securityToken);
					cryptoStream.Write(bytes, 0, bytes.Length);
					cryptoStream.Flush();
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		internal static string DecryptSecurityToken(string userId, byte[] encryptedToken, byte[] key, byte[] iv)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (iv == null)
			{
				throw new ArgumentNullException("iv");
			}
			string @string;
			using (HMACSHA256Cng hmacsha256Cng = new HMACSHA256Cng(key))
			{
				byte[] key2 = hmacsha256Cng.ComputeHash(Encoding.UTF8.GetBytes(userId));
				SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create();
				symmetricAlgorithm.Padding = PaddingMode.ISO10126;
				symmetricAlgorithm.Key = key2;
				symmetricAlgorithm.IV = iv;
				MemoryStream memoryStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
				cryptoStream.Write(encryptedToken, 0, encryptedToken.Length);
				cryptoStream.Close();
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		internal static bool IsSecurityTokenPresented(HttpRequest request)
		{
			return DelegatedAuthenticationModule.GetSecurityTokenProperty(request) != null;
		}

		private static void InternalOnAuthenticate(HttpContext context)
		{
			HttpRequest request = context.Request;
			if (DelegatedAuthenticationModule.IsOAuthLinkedAccount(context))
			{
				DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest. OAuth Linked account.");
				return;
			}
			string userId = DelegatedAuthenticationModule.GetUserId(context);
			string targetTenant = DelegatedAuthenticationModule.GetTargetTenant(request);
			DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. uesrId = {0}; targetOrg = {1}", new object[]
			{
				userId,
				targetTenant
			});
			if (string.IsNullOrWhiteSpace(targetTenant))
			{
				if (!DelegatedAuthenticationModule.IsCurrentStackECP())
				{
					DelegatedAuthenticationModule.LogDebug("Target Organization not present for user {0}", new object[]
					{
						userId
					});
				}
				DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest. TargetOrg is empty");
				return;
			}
			if (string.IsNullOrWhiteSpace(userId))
			{
				DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_FailedToResolveCurrentUser, targetTenant, new object[0]);
				DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "UserId is empty");
				return;
			}
			string securityTokenProperty = DelegatedAuthenticationModule.GetSecurityTokenProperty(context.Request);
			bool flag = request.Headers["msExchCafeForceRouteToLogonAccount"] == "1";
			if (!flag && DelegatedAuthenticationModule.ResolvePreAuthenticatedUserFromCache(context, targetTenant, userId, securityTokenProperty))
			{
				DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest. Succeeded to ResolvePreAuthenticatedUserFromCache");
				return;
			}
			byte[] array = null;
			try
			{
				if (!string.IsNullOrEmpty(securityTokenProperty))
				{
					array = Convert.FromBase64String(securityTokenProperty);
				}
			}
			catch (FormatException ex)
			{
				DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_FailedToDecodeBase64SecurityToken, userId, new object[]
				{
					userId,
					request.Url,
					ex
				});
				DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "Failed to decode base64 security token");
				return;
			}
			IPrincipal user = context.User;
			Uri uri;
			if (request.Headers[WellKnownHeader.XIsFromCafe] == "1")
			{
				uri = new Uri(request.Url, request.Headers[WellKnownHeader.MsExchProxyUri]);
			}
			else if (request.Headers["msExchOriginalUrl"] != null)
			{
				uri = new Uri(request.Url, request.Headers["msExchOriginalUrl"]);
			}
			else
			{
				uri = request.Url;
			}
			DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. originalUrl = {0}", new object[]
			{
				uri
			});
			if (array != null)
			{
				DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. Security Token Present on the Uri");
				ExchangeConfigurationUnit exchangeConfigurationUnit;
				if (!DelegatedAuthenticationModule.TryResolveConfigurationUnit(context.Response, userId, targetTenant, out exchangeConfigurationUnit))
				{
					return;
				}
				if (exchangeConfigurationUnit == null)
				{
					DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_FailedToResolveTargetOrganization, userId, new object[]
					{
						targetTenant,
						userId
					});
					DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "TargetOrgCU is empty");
					return;
				}
				string securityToken;
				if (!DelegatedAuthenticationModule.TryDecryptBase64EncryptedSecurityToken(context.Response, userId, targetTenant, array, out securityToken))
				{
					return;
				}
				DelegatedSecurityToken delegatedSecurityToken = DelegatedSecurityToken.Parse(securityToken);
				if (delegatedSecurityToken.IsExpired())
				{
					DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. The security token is expired");
					DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_ExpiredSecurityToken, null, new object[]
					{
						userId,
						delegatedSecurityToken.UTCCreationTime,
						DelegatedAuthenticationModule.IsCurrentStackECP()
					});
					if (DelegatedAuthenticationModule.IsCurrentStackECP() && DelegatedAuthenticationModule.TryRedirectEcpForSecurityTokenRenewal(context, targetTenant, uri))
					{
						return;
					}
					DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "The security token is expired");
					return;
				}
				else
				{
					if (delegatedSecurityToken.PartnerGroupIds == null || delegatedSecurityToken.PartnerGroupIds.Length == 0)
					{
						DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_NoGroupMembershipOnSecurityToken, userId, new object[]
						{
							userId
						});
						DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "No group membership on security token");
						return;
					}
					foreach (string text in delegatedSecurityToken.PartnerGroupIds)
					{
						DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. Member of partner group {0}", new object[]
						{
							text
						});
					}
					DelegatedAuthenticationModule.AddCookie(context, targetTenant, DelegatedAuthenticationModule.securityTokenUriPropertyName, securityTokenProperty);
					DelegatedPrincipal delegatedPrincipal = DelegatedAuthenticationModule.CreateDelegatedPrincipal(user, userId, exchangeConfigurationUnit.ToString(), delegatedSecurityToken);
					context.User = delegatedPrincipal;
					DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest. Authentication succeeded. userid = {0}; targetOrgCU = {1}", new object[]
					{
						userId,
						exchangeConfigurationUnit.DistinguishedName
					});
					if (!DelegatedPrincipalCache.TrySetEntry(targetTenant, userId, securityTokenProperty, new DelegatedPrincipalCacheData(delegatedPrincipal, delegatedSecurityToken.UTCExpirationTime)))
					{
						DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. Delegated Principal Cache Is Full");
						DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_DelegatedPrincipalCacheIsFull, userId, new object[]
						{
							DateTime.UtcNow,
							DelegatedPrincipalCache.NextScheduleCacheCleanUp()
						});
						return;
					}
				}
			}
			else
			{
				DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. Scurity Token Not Present on the Uri");
				if (!(context.Items["Item-CommonAccessToken"] is CommonAccessToken) && !DelegatedAuthenticationModule.IsUserPresentOnForest(context.User))
				{
					DelegatedAuthenticationModule.LogDebug("Authenticated User {0} is not present on the current forest, skipping the DelegatedAuthentication", new object[]
					{
						userId
					});
					return;
				}
				ExchangeConfigurationUnit exchangeConfigurationUnit2 = null;
				if (!DelegatedAuthenticationModule.TryResolveConfigurationUnit(context.Response, userId, targetTenant, out exchangeConfigurationUnit2))
				{
					return;
				}
				Uri uri2 = null;
				if (exchangeConfigurationUnit2 != null && DelegatedAuthenticationModule.ShouldRedirectToLocalServer(context, targetTenant, true))
				{
					uri2 = new Uri(context.Request.Url, uri);
				}
				else if (exchangeConfigurationUnit2 == null || flag)
				{
					if (exchangeConfigurationUnit2 != null)
					{
						uri2 = uri;
						DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. Use original url as forestRedirectUrl = {0}", new object[]
						{
							uri2
						});
					}
					else
					{
						uri2 = RedirectionHelper.GetRedirectUrlForTenantForest(targetTenant, DelegatedAuthenticationModule.podRedirectTemplate, uri, DelegatedAuthenticationModule.podSiteStartRange, DelegatedAuthenticationModule.podSiteEndRange);
						if (uri2 == null)
						{
							DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_CannotResolveForestRedirection, userId, new object[]
							{
								userId,
								targetTenant,
								uri
							});
							DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "forestRedirectionUrl is empty");
							return;
						}
						DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. forestRedirectUrl = {0}", new object[]
						{
							uri2
						});
					}
				}
				ADGroup[] array2 = null;
				ADRawEntry adrawEntry = null;
				ExchangeConfigurationUnit exchangeConfigurationUnit3 = null;
				Exception ex2 = null;
				try
				{
					adrawEntry = DelegatedAuthenticationModule.ResolveADUserFromPrincipal(user);
					if (exchangeConfigurationUnit2 != null && exchangeConfigurationUnit2.OrganizationId.Equals((OrganizationId)adrawEntry[ADObjectSchema.OrganizationId]))
					{
						DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest. the user is requesting a delegated connection to its own organization {0}", new object[]
						{
							exchangeConfigurationUnit2.DistinguishedName
						});
						return;
					}
					array2 = DelegatedAuthenticationModule.ResolveDelegatedGroupForUser(adrawEntry);
					if (OrganizationId.ForestWideOrgId.Equals(adrawEntry[ADObjectSchema.OrganizationId]))
					{
						DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "The connectingUser is in the first org");
						return;
					}
					exchangeConfigurationUnit3 = DelegatedAuthenticationModule.GetADOrganizationById((OrganizationId)adrawEntry[ADObjectSchema.OrganizationId]);
					if (exchangeConfigurationUnit3 == null)
					{
						DelegatedAuthenticationModule.SendServerError(context.Response, userId, new CannotResolveUserTenantException(adrawEntry[ADObjectSchema.OrganizationId].ToString()));
						return;
					}
				}
				catch (DataSourceOperationException ex3)
				{
					ex2 = ex3;
				}
				catch (TransientException ex4)
				{
					ex2 = ex4;
				}
				catch (DataValidationException ex5)
				{
					ex2 = ex5;
				}
				if (ex2 != null)
				{
					DelegatedAuthenticationModule.SendServerError(context.Response, userId, ex2);
					return;
				}
				if (array2 == null || array2.Length == 0)
				{
					DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "the user doesn't belong to any groups.");
					return;
				}
				foreach (ADGroup adgroup in array2)
				{
					DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. The user is a member of group {0}", new object[]
					{
						adgroup
					});
				}
				string[] array4 = DelegatedAuthenticationModule.ExtractDirectoryObjectId(array2);
				if (array4 == null || array4.Length == 0)
				{
					DelegatedAuthenticationModule.SendAccessDenied(context.Response, userId, "cannot find any linked groups");
					return;
				}
				foreach (string text2 in array4)
				{
					DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. Found linked group {0}", new object[]
					{
						text2
					});
				}
				DelegatedSecurityToken delegatedSecurityToken2 = new DelegatedSecurityToken(((string)adrawEntry[ADRecipientSchema.DisplayName]) ?? string.Empty, exchangeConfigurationUnit3.ExternalDirectoryOrganizationId, array4);
				if (uri2 == null)
				{
					DelegatedPrincipal delegatedPrincipal2 = DelegatedAuthenticationModule.CreateDelegatedPrincipal(user, userId, exchangeConfigurationUnit2.ToString(), delegatedSecurityToken2);
					context.User = delegatedPrincipal2;
					DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest. Authentication succeeded. userid = {0}; targetOrgCU = {1}", new object[]
					{
						userId,
						exchangeConfigurationUnit2.DistinguishedName
					});
					if (!DelegatedPrincipalCache.TrySetEntry(targetTenant, userId, securityTokenProperty, new DelegatedPrincipalCacheData(delegatedPrincipal2, delegatedSecurityToken2.UTCExpirationTime)))
					{
						DelegatedAuthenticationModule.LogDebug("In OnAuthenticateRequest. Delegated Principal Cache Is Full");
						DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_DelegatedPrincipalCacheIsFull, userId, new object[]
						{
							DateTime.UtcNow,
							DelegatedPrincipalCache.NextScheduleCacheCleanUp()
						});
						return;
					}
				}
				else
				{
					string encryptedSecurityToken = null;
					if (!DelegatedAuthenticationModule.TryGenerateBase64EncryptedSecurityToken(context.Response, userId, delegatedSecurityToken2, out encryptedSecurityToken))
					{
						return;
					}
					DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest. Forest redirection happends. Redirect to {0}.", new object[]
					{
						uri2
					});
					context.Response.Redirect(DelegatedAuthenticationModule.AppendSecurityTokenToRedirectionUri(uri2, encryptedSecurityToken));
				}
			}
		}

		private static void OnAuthenticateRequest(object source, EventArgs args)
		{
			DelegatedAuthenticationModule.LogDebug("Enter OnAuthenticateRequest");
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			if (!context.Request.IsAuthenticated)
			{
				if (!DelegatedAuthenticationModule.IsCurrentStackECP())
				{
					DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_RequestNotAuthenticated, context.Request.Path, new object[0]);
				}
				DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest. The original incomming request isn't authenticated");
				return;
			}
			DelegatedAuthenticationModule.InternalOnAuthenticate(context);
			DelegatedAuthenticationModule.LogDebug("Exit OnAuthenticateRequest");
		}

		private static bool TryRedirectEcpForSecurityTokenRenewal(HttpContext context, string targetOrg, Uri originalUrl)
		{
			bool result = false;
			DelegatedAuthenticationModule.DeleteCookie(context, targetOrg, DelegatedAuthenticationModule.securityTokenUriPropertyName);
			if (context.Request.RequestType == "GET")
			{
				originalUrl = new Uri(DelegatedAuthenticationModule.RemoveParameterFromUrlForEcp(originalUrl.ToString(), DelegatedAuthenticationModule.securityTokenUriPropertyName));
				Uri uri;
				if (DelegatedAuthenticationModule.ShouldRedirectToLocalServer(context, targetOrg, false))
				{
					string text = originalUrl.ToString();
					if (!text.Contains("redirtolocal="))
					{
						text = DelegatedAuthenticationModule.AppendParameterToUrl(text, "&", "redirtolocal", "1");
					}
					uri = new Uri(text);
				}
				else
				{
					uri = RedirectionHelper.GetRedirectUrlForTenantForest(DelegatedAuthenticationModule.GetUserDomainName(context), DelegatedAuthenticationModule.podRedirectTemplate, originalUrl, DelegatedAuthenticationModule.podSiteStartRange, DelegatedAuthenticationModule.podSiteEndRange);
				}
				if (uri != null)
				{
					DelegatedAuthenticationModule.LogDebug("DelegatedAuthenticationModule::TryRedirectEcpForSecurityTokenRenewal. Redirect to {0}.", new object[]
					{
						uri
					});
					context.Response.Redirect(uri.AbsoluteUri);
					result = true;
				}
				return result;
			}
			throw new DelegatedSecurityTokenExpiredException();
		}

		private static bool ShouldRedirectToLocalServer(HttpContext context, string targetOrg, bool addCookieIfNotPresent)
		{
			bool flag = DelegatedAuthenticationModule.isRedirectToLocalServerEnabled;
			if (flag && context.Request.Cookies["redirtolocal"] == null)
			{
				if (context.Request.QueryString["redirtolocal"] != null)
				{
					if (addCookieIfNotPresent)
					{
						DelegatedAuthenticationModule.AddCookie(context, targetOrg, "redirtolocal", "1");
					}
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		private static void AddCookie(HttpContext context, string targetOrg, string key, string value)
		{
			DelegatedAuthenticationModule.SetCookie(context, targetOrg, key, value, false);
		}

		private static void DeleteCookie(HttpContext context, string targetOrg, string key)
		{
			DelegatedAuthenticationModule.SetCookie(context, targetOrg, key, string.Empty, true);
		}

		private static void SetCookie(HttpContext context, string targetOrg, string key, string value, bool remove)
		{
			HttpCookie httpCookie = new HttpCookie(key, value);
			if (string.IsNullOrEmpty(targetOrg))
			{
				httpCookie.Path = DelegatedAuthenticationModule.appDomainAppVirtualPath;
			}
			else
			{
				httpCookie.Path = string.Format("{0}/@{1}/", DelegatedAuthenticationModule.appDomainAppVirtualPath, targetOrg);
			}
			if (remove)
			{
				httpCookie.Expires = (DateTime)ExDateTime.Now.AddYears(-1);
			}
			context.Response.Cookies.Add(httpCookie);
		}

		private static void SendAccessDenied(HttpResponse response, string userId, string reason)
		{
			DelegatedAuthenticationModule.LogDebug("Sending 401 Access for user {0}. {1}", new object[]
			{
				userId,
				reason
			});
			DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_AccessDenied, userId, new object[]
			{
				userId
			});
			WinRMInfo.SetFailureCategoryInfo(response.Headers, FailureCategory.DelegatedAuth, "AccessDenied");
			response.Clear();
			response.StatusCode = 401;
			if (DelegatedAuthenticationModule.IsCurrentStackECP())
			{
				throw new DelegatedAccessDeniedException();
			}
			response.End();
		}

		private static void SendServerError(HttpResponse response, string userId, Exception error)
		{
			DelegatedAuthenticationModule.LogError("There is a server error: {0}", error);
			DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_ServerError, userId, new object[]
			{
				error,
				userId
			});
			WinRMInfo.SetFailureCategoryInfo(response.Headers, FailureCategory.DelegatedAuth, error.GetType().Name);
			response.Clear();
			response.StatusCode = 500;
			if (DelegatedAuthenticationModule.IsCurrentStackECP())
			{
				throw new DelegatedServerErrorException(error);
			}
			response.End();
		}

		private static bool IsUserPresentOnForest(IPrincipal principal)
		{
			WindowsIdentity windowsIdentity = principal.Identity as WindowsIdentity;
			if (windowsIdentity == null)
			{
				LiveIDIdentity liveIDIdentity = principal.Identity as LiveIDIdentity;
				if (liveIDIdentity != null)
				{
					return true;
				}
			}
			else if (!windowsIdentity.IsSystem)
			{
				return true;
			}
			return false;
		}

		private static string GetUserDomainName(HttpContext context)
		{
			string userId = DelegatedAuthenticationModule.GetUserId(context);
			if (userId != null && SmtpAddress.IsValidSmtpAddress(userId))
			{
				return SmtpAddress.Parse(userId).Domain;
			}
			return string.Empty;
		}

		private static string GetUserId(HttpContext context)
		{
			SidOAuthIdentity sidOAuthIdentity = context.User.Identity as SidOAuthIdentity;
			if (sidOAuthIdentity != null)
			{
				return sidOAuthIdentity.Name;
			}
			object obj;
			if ((obj = context.Items["RPSMemberName"]) == null && (obj = context.Items["WLID-MemberName"]) == null)
			{
				obj = (context.Request.Headers["RPSMemberName"] ?? context.GetMemberName());
			}
			return (string)obj;
		}

		private static bool IsOAuthLinkedAccount(HttpContext context)
		{
			SidOAuthIdentity sidOAuthIdentity = context.User.Identity as SidOAuthIdentity;
			return sidOAuthIdentity != null && sidOAuthIdentity.OAuthAccountType == SidOAuthIdentity.AccountType.OAuthLinkedAccount;
		}

		private static bool ResolvePreAuthenticatedUserFromCache(HttpContext context, string targetOrg, string userId, string securityToken)
		{
			DelegatedPrincipalCache.Cleanup();
			DelegatedPrincipalCacheData entry = DelegatedPrincipalCache.GetEntry(targetOrg, userId, securityToken);
			if (entry == null)
			{
				return false;
			}
			if (entry.IsExpired())
			{
				DelegatedPrincipalCache.RemoveEntry(targetOrg, userId, securityToken);
				DelegatedAuthenticationModule.LogDebug("Principal for key {0}\\{1} is removed from the cache due to expiration.", new object[]
				{
					targetOrg,
					userId
				});
				return false;
			}
			context.User = entry.Principal;
			DelegatedAuthenticationModule.LogDebug("Principal for key {0}\\{1} is authenticated from the cache.", new object[]
			{
				targetOrg,
				userId
			});
			return true;
		}

		private static bool TryResolveConfigurationUnit(HttpResponse response, string userId, string targetOrganization, out ExchangeConfigurationUnit cu)
		{
			cu = null;
			Exception ex = null;
			try
			{
				Guid externalDirectoryId;
				if (GuidHelper.TryParseGuid(targetOrganization, out externalDirectoryId))
				{
					cu = DelegatedAuthenticationModule.GetADOrganizationByExternalDirectoryId(externalDirectoryId);
				}
				else
				{
					cu = DelegatedAuthenticationModule.GetADOrganizationByName(targetOrganization);
				}
			}
			catch (DataSourceOperationException ex2)
			{
				ex = ex2;
			}
			catch (TransientException ex3)
			{
				ex = ex3;
			}
			catch (DataValidationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				DelegatedAuthenticationModule.SendServerError(response, userId, ex);
				return false;
			}
			return true;
		}

		private static bool TryDecryptBase64EncryptedSecurityToken(HttpResponse response, string userId, string targetOrg, byte[] encryptedToken, out string decryptedToken)
		{
			decryptedToken = null;
			if (!DelegatedAuthenticationModule.ValidateSecretKey(true))
			{
				DelegatedAuthenticationModule.SendServerError(response, userId, new CannotResolveCurrentKeyException(true));
				return false;
			}
			try
			{
				decryptedToken = DelegatedAuthenticationModule.DecryptSecurityToken(userId, encryptedToken, GccUtils.DatacenterServerAuthentication.CurrentSecretKey, GccUtils.DatacenterServerAuthentication.CurrentIVKey);
			}
			catch (CryptographicException ex)
			{
				DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_FailedToDecryptSecurityToken, userId, new object[]
				{
					userId,
					targetOrg,
					ex,
					true
				});
			}
			try
			{
				if (decryptedToken == null && GccUtils.DatacenterServerAuthentication.PreviousSecretKey != null)
				{
					if (!DelegatedAuthenticationModule.ValidateSecretKey(false))
					{
						DelegatedAuthenticationModule.SendServerError(response, userId, new CannotResolveCurrentKeyException(false));
						return false;
					}
					decryptedToken = DelegatedAuthenticationModule.DecryptSecurityToken(userId, encryptedToken, GccUtils.DatacenterServerAuthentication.PreviousSecretKey, GccUtils.DatacenterServerAuthentication.PreviousIVKey);
				}
			}
			catch (CryptographicException ex2)
			{
				DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_FailedToDecryptSecurityToken, userId, new object[]
				{
					userId,
					targetOrg,
					ex2,
					false
				});
			}
			if (string.IsNullOrEmpty(decryptedToken))
			{
				DelegatedAuthenticationModule.SendAccessDenied(response, userId, "decryptedToken is empty");
				return false;
			}
			return true;
		}

		private static bool TryGenerateBase64EncryptedSecurityToken(HttpResponse response, string userId, DelegatedSecurityToken securityToken, out string encryptedSecurityToken)
		{
			if (!DelegatedAuthenticationModule.ValidateSecretKey(true))
			{
				encryptedSecurityToken = null;
				DelegatedAuthenticationModule.SendServerError(response, userId, new CannotResolveCurrentKeyException(true));
				return false;
			}
			byte[] array = null;
			Exception error = null;
			try
			{
				array = DelegatedAuthenticationModule.EncryptSecurityToken(userId, securityToken.ToString());
			}
			catch (CannotResolveCurrentKeyException ex)
			{
				error = ex;
			}
			catch (CryptographicException ex2)
			{
				error = ex2;
			}
			if (array == null)
			{
				encryptedSecurityToken = null;
				DelegatedAuthenticationModule.SendServerError(response, userId, error);
				return false;
			}
			encryptedSecurityToken = Convert.ToBase64String(array, 0, array.Length, Base64FormattingOptions.None);
			return true;
		}

		private static bool ValidateSecretKey(bool currentKey)
		{
			if (currentKey)
			{
				try
				{
					GccUtils.RefreshProxySecretKeys();
				}
				catch (InvalidDatacenterProxyKeyException)
				{
					return false;
				}
				return GccUtils.DatacenterServerAuthentication.CurrentSecretKey != null && GccUtils.DatacenterServerAuthentication.CurrentIVKey != null;
			}
			return GccUtils.DatacenterServerAuthentication.PreviousSecretKey != null && GccUtils.DatacenterServerAuthentication.PreviousIVKey != null;
		}

		private static DelegatedPrincipal CreateDelegatedPrincipal(IPrincipal user, string userId, string targetOrg, DelegatedSecurityToken token)
		{
			return new DelegatedPrincipal(DelegatedPrincipal.GetDelegatedIdentity(userId, token.PartnerOrgDirectoryId, targetOrg, token.DisplayName, token.PartnerGroupIds), token.PartnerGroupIds);
		}

		private static string AppendSecurityTokenToRedirectionUri(Uri redirectionUri, string encryptedSecurityToken)
		{
			string url = redirectionUri.ToString();
			encryptedSecurityToken = Uri.EscapeDataString(encryptedSecurityToken);
			return DelegatedAuthenticationModule.AppendParameterToUrl(url, DelegatedAuthenticationModule.IsCurrentStackECP() ? "&" : ";", DelegatedAuthenticationModule.securityTokenUriPropertyName, encryptedSecurityToken);
		}

		private static string AppendParameterToUrl(string url, string ampStr, string key, string value)
		{
			int capacity = url.Length + key.Length + value.Length + 2;
			StringBuilder stringBuilder = new StringBuilder(url, capacity);
			if (!url.EndsWith("&"))
			{
				stringBuilder.Append(url.Contains("?") ? ampStr : "?");
			}
			stringBuilder.Append(key);
			stringBuilder.Append("=");
			stringBuilder.Append(value);
			return stringBuilder.ToString();
		}

		private static string RemoveParameterFromUrlForEcp(string url, string name)
		{
			bool flag = false;
			int num = url.IndexOf('?');
			if (num > 0)
			{
				while (!flag && num < url.Length)
				{
					int num2 = url.IndexOf(name, num, StringComparison.OrdinalIgnoreCase);
					if (num2 <= 0)
					{
						break;
					}
					char c = url[num2 - 1];
					char c2 = (num2 + name.Length < url.Length) ? url[num2 + name.Length] : ' ';
					if ((c == '?' || c == '&') && c2 == '=')
					{
						flag = true;
						int num3 = url.IndexOf('&', num2 + name.Length + 1);
						if (num3 < 0)
						{
							num3 = url.IndexOf('#', num2 + name.Length + 1);
							if (num3 < 0)
							{
								num3 = url.Length;
							}
							num2--;
							num3--;
						}
						url = url.Remove(num2, num3 - num2 + 1);
					}
					else
					{
						num = num2 + name.Length;
					}
				}
			}
			return url;
		}

		private static bool IsCurrentStackECP()
		{
			if (DelegatedAuthenticationModule.protocol == DelegatedAuthenticationModule.Protocol.RWS || DelegatedAuthenticationModule.protocol == DelegatedAuthenticationModule.Protocol.Psws)
			{
				return false;
			}
			if (DelegatedAuthenticationModule.isEcpStack == null)
			{
				Configuration configuration = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.Path);
				string rawXml = configuration.GetSection("appSettings").SectionInformation.GetRawXml();
				string rawXml2 = configuration.GetSection("system.webServer").SectionInformation.GetRawXml();
				if (rawXml != null && rawXml.IndexOf("ProtocolType", StringComparison.OrdinalIgnoreCase) >= 0 && rawXml.IndexOf("RemotePS", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					DelegatedAuthenticationModule.isEcpStack = new bool?(false);
				}
				else if (rawXml2 != null && rawXml2.IndexOf("system.management.wsmanagement.config", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					DelegatedAuthenticationModule.isEcpStack = new bool?(false);
				}
				else
				{
					DelegatedAuthenticationModule.isEcpStack = new bool?(true);
				}
			}
			return DelegatedAuthenticationModule.isEcpStack.Value;
		}

		private static string[] ExtractDirectoryObjectId(ADGroup[] groups)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < groups.Length; i++)
			{
				if (!string.IsNullOrEmpty(groups[i].ExternalDirectoryObjectId) && groups[i].RawCapabilities != null && groups[i].RawCapabilities.Contains(Capability.Partner_Managed))
				{
					list.Add(groups[i].ExternalDirectoryObjectId);
				}
				else
				{
					DelegatedAuthenticationModule.LogDebug("The following group: {0} is excluded from delegated partner permissions.", new object[]
					{
						groups[i].Id
					});
				}
			}
			return list.ToArray();
		}

		private static ADRawEntry ResolveADUserFromPrincipal(IPrincipal user)
		{
			WindowsIdentity windowsIdentity = user.Identity as WindowsIdentity;
			SecurityIdentifier securityIdentifier = null;
			PartitionId partitionId = null;
			OrganizationId organizationId = null;
			if (windowsIdentity != null)
			{
				securityIdentifier = windowsIdentity.User;
			}
			else
			{
				LiveIDIdentity liveIDIdentity = user.Identity as LiveIDIdentity;
				if (liveIDIdentity != null)
				{
					securityIdentifier = liveIDIdentity.Sid;
					organizationId = liveIDIdentity.UserOrganizationId;
				}
				else
				{
					SidOAuthIdentity sidOAuthIdentity = user.Identity as SidOAuthIdentity;
					if (sidOAuthIdentity != null)
					{
						securityIdentifier = sidOAuthIdentity.OAuthIdentity.ActAsUser.Sid;
						organizationId = sidOAuthIdentity.OAuthIdentity.OrganizationId;
					}
					else
					{
						GenericSidIdentity genericSidIdentity = user.Identity as GenericSidIdentity;
						if (genericSidIdentity != null)
						{
							securityIdentifier = genericSidIdentity.Sid;
							if (!string.IsNullOrEmpty(genericSidIdentity.PartitionId))
							{
								PartitionId.TryParse(genericSidIdentity.PartitionId, out partitionId);
							}
						}
					}
				}
			}
			if (securityIdentifier == null)
			{
				throw new CannotResolveWindowsIdentityException();
			}
			ADRawEntry adrawEntry = UserTokenStaticHelper.GetADRawEntry(partitionId, organizationId, securityIdentifier);
			if (adrawEntry == null)
			{
				throw new CannotResolveSidToADAccountException(securityIdentifier.ToString());
			}
			return adrawEntry;
		}

		private static ADGroup[] ResolveDelegatedGroupForUser(ADRawEntry userEntry)
		{
			if (userEntry == null)
			{
				throw new ArgumentNullException("userEntry");
			}
			OrganizationId organizationId = (OrganizationId)userEntry[ADObjectSchema.OrganizationId];
			ADSessionSettings sessionSettings = (organizationId == null) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(organizationId);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 1576, "ResolveDelegatedGroupForUser", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\DelegatedAuthentication\\DelegatedAuthenticationModule.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			List<string> tokenSids = tenantOrRootOrgRecipientSession.GetTokenSids(userEntry, AssignmentMethod.SecurityGroup);
			ADObjectId[] array = tenantOrRootOrgRecipientSession.ResolveSidsToADObjectIds(tokenSids.ToArray());
			Result<ADGroup>[] array2 = tenantOrRootOrgRecipientSession.ReadMultipleADGroups(array);
			StringBuilder stringBuilder = null;
			List<ADGroup> list = new List<ADGroup>(array2.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].Error != null)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					else
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(array[i]);
				}
				else
				{
					list.Add(array2[i].Data);
				}
			}
			if (stringBuilder != null)
			{
				DelegatedAuthenticationModule.LogEvent(TaskEventLogConstants.Tuple_DelegatedAuth_FailedToReadMultiple, (userEntry[IADSecurityPrincipalSchema.Sid] != null) ? userEntry[IADSecurityPrincipalSchema.Sid].ToString() : string.Empty, new object[]
				{
					stringBuilder.ToString(),
					(userEntry[ADObjectSchema.Id] != null) ? userEntry[ADObjectSchema.Id].ToString() : string.Empty
				});
			}
			return list.ToArray();
		}

		private static ExchangeConfigurationUnit GetADOrganizationByName(string orgName)
		{
			ExchangeConfigurationUnit result = null;
			try
			{
				PartitionId partitionIdByAcceptedDomainName = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(orgName);
				ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionIdByAcceptedDomainName);
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 1637, "GetADOrganizationByName", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\DelegatedAuthentication\\DelegatedAuthenticationModule.cs");
				result = tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(orgName);
			}
			catch (CannotResolveTenantNameException)
			{
			}
			return result;
		}

		private static ExchangeConfigurationUnit GetADOrganizationByExternalDirectoryId(Guid externalDirectoryId)
		{
			try
			{
				PartitionId partitionIdByExternalDirectoryOrganizationId = ADAccountPartitionLocator.GetPartitionIdByExternalDirectoryOrganizationId(externalDirectoryId);
				ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionIdByExternalDirectoryOrganizationId);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 1664, "GetADOrganizationByExternalDirectoryId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\DelegatedAuthentication\\DelegatedAuthenticationModule.cs");
				ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId, externalDirectoryId.ToString());
				ExchangeConfigurationUnit[] array = tenantOrTopologyConfigurationSession.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, filter, null, 1);
				if (array.Length > 0)
				{
					return array[0];
				}
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException)
			{
			}
			return null;
		}

		private static ExchangeConfigurationUnit GetADOrganizationById(OrganizationId orgId)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(orgId), 1693, "GetADOrganizationById", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\DelegatedAuthentication\\DelegatedAuthenticationModule.cs");
			return tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(orgId.ConfigurationUnit);
		}

		private static string GetSecurityTokenProperty(HttpRequest request)
		{
			string text;
			if (DelegatedAuthenticationModule.IsCurrentStackECP())
			{
				text = request.QueryString[DelegatedAuthenticationModule.securityTokenUriPropertyName];
			}
			else
			{
				UriBuilder uriBuilder = new UriBuilder(request.Url);
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uriBuilder.Query.Replace(';', '&'));
				text = nameValueCollection[DelegatedAuthenticationModule.securityTokenUriPropertyName];
			}
			if (text != null)
			{
				text = Uri.UnescapeDataString(text);
			}
			else
			{
				HttpCookie httpCookie = request.Cookies[DelegatedAuthenticationModule.securityTokenUriPropertyName];
				if (httpCookie != null)
				{
					text = httpCookie.Value;
				}
			}
			return text;
		}

		private static void LogDebug(string message, params object[] args)
		{
			if (ExTraceGlobals.DelegatedAuthTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.DelegatedAuthTracer.TraceDebug(0L, message, args);
			}
		}

		private static void LogDebug(string message)
		{
			if (ExTraceGlobals.DelegatedAuthTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				DelegatedAuthenticationModule.LogDebug(message, new object[0]);
			}
		}

		private static void LogError(string message, Exception exception)
		{
			if (ExTraceGlobals.DelegatedAuthTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.DelegatedAuthTracer.TraceError<string, Exception>(0L, "{0} - {1}", message, exception);
			}
		}

		private static void LogEvent(ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
			if (messageArguments == null)
			{
				throw new ArgumentNullException("messageArguments");
			}
			object[] array = new object[messageArguments.Length + 2];
			array[0] = DelegatedAuthenticationModule.processName;
			array[1] = DelegatedAuthenticationModule.processId;
			messageArguments.CopyTo(array, 2);
			DelegatedAuthenticationModule.eventLogger.LogEvent(eventInfo, periodicKey, array);
		}

		private static string GetTargetTenant(HttpRequest request)
		{
			string result;
			if (DelegatedAuthenticationModule.protocol == DelegatedAuthenticationModule.Protocol.PowerShell)
			{
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(request.Url.Query.Replace(';', '&'));
				result = nameValueCollection["DelegatedOrg"];
			}
			else if (DelegatedAuthenticationModule.protocol == DelegatedAuthenticationModule.Protocol.RWS)
			{
				result = request.QueryString["DelegatedOrg"];
			}
			else if (DelegatedAuthenticationModule.protocol == DelegatedAuthenticationModule.Protocol.Psws)
			{
				result = request.Headers["msExchTargetTenant"];
			}
			else
			{
				result = request.Headers["msExchTargetTenant"];
			}
			return result;
		}

		private const string CookiePathFormat = "{0}/@{1}/";

		private const string RedirToLocalKey = "redirtolocal";

		private const string RedirToLocalKeyWithTrailingEqual = "redirtolocal=";

		private const string CafeForceRouteToLogonAccountKey = "msExchCafeForceRouteToLogonAccount";

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.DelegatedAuthTracer.Category, "MSExchange Delegated Authentication Module");

		private static readonly EventHandler OnAuthenticateRequestHandler = new EventHandler(DelegatedAuthenticationModule.OnAuthenticateRequest);

		private static readonly string appDomainAppVirtualPath;

		private static readonly bool isRedirectToLocalServerEnabled;

		private static string securityTokenUriPropertyName = "SecurityToken";

		private static int podSiteStartRange = 0;

		private static int podSiteEndRange = 0;

		private static string podRedirectTemplate = null;

		private static string siteRedirectTemplate = null;

		private static string processName;

		private static int processId;

		private static bool? isEcpStack = null;

		private static DelegatedAuthenticationModule.Protocol protocol;

		private enum Protocol
		{
			Unknown,
			RWS,
			ECP,
			PowerShell,
			Psws
		}
	}
}
