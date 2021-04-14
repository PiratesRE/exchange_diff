using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class RequestDispatcher
	{
		public static void DispatchRequest(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.DispatchRequest");
			HttpContext httpContext = owaContext.HttpContext;
			UrlUtilities.RewriteFederatedDomainInURL(httpContext);
			RequestDispatcher.DispatchStepResult arg = RequestDispatcher.InternalDispatchRequest(owaContext);
			ExTraceGlobals.CoreCallTracer.TraceDebug<RequestDispatcher.DispatchStepResult>(0L, "Last dispatch step result = {0}", arg);
			switch (arg)
			{
			case RequestDispatcher.DispatchStepResult.RedirectToUrl:
				if (owaContext.RequestType != OwaRequestType.Logoff)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Redirecting response to {0}", owaContext.DestinationUrl);
					httpContext.Response.Redirect(owaContext.DestinationUrl, false);
				}
				httpContext.ApplicationInstance.CompleteRequest();
				return;
			case RequestDispatcher.DispatchStepResult.RewritePath:
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Rewritting path to {0}", owaContext.DestinationUrl);
				if (string.IsNullOrEmpty(owaContext.DestinationUrlQueryString))
				{
					httpContext.RewritePath(owaContext.DestinationUrl);
					return;
				}
				httpContext.RewritePath(owaContext.DestinationUrl, null, owaContext.DestinationUrlQueryString);
				return;
			case RequestDispatcher.DispatchStepResult.RewritePathToError:
				if (owaContext.RequestType == OwaRequestType.WebPart)
				{
					owaContext.ErrorInformation = new ErrorInformation(owaContext.ErrorString);
					ExTraceGlobals.WebPartRequestTracer.TraceDebug<string>(0L, "Write error to invalid web part request: '{0}' so that web request can handle error response.", owaContext.ErrorInformation.Message);
					Utilities.WriteErrorToWebPart(owaContext);
					return;
				}
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Rewriting path to error page");
				Utilities.RewritePathToError(owaContext, owaContext.ErrorString);
				return;
			case RequestDispatcher.DispatchStepResult.WriteErrorToWebPart:
				ExTraceGlobals.WebPartRequestTracer.TraceDebug<string>(0L, "Write error to invalid web part request: '{0}' so that web request can handle error response.", owaContext.ErrorInformation.Message);
				Utilities.WriteErrorToWebPart(owaContext);
				break;
			case RequestDispatcher.DispatchStepResult.EndResponse:
				ExTraceGlobals.CoreTracer.TraceDebug<int>(0L, "Ending response with statusCode={0}", (int)owaContext.HttpStatusCode);
				Utilities.EndResponse(owaContext.HttpContext, owaContext.HttpStatusCode);
				return;
			case RequestDispatcher.DispatchStepResult.Stop:
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Stopped the dispatching of this request");
				return;
			case RequestDispatcher.DispatchStepResult.Continue:
				break;
			default:
				return;
			}
		}

		private static RequestDispatcher.DispatchStepResult InternalDispatchRequest(OwaContext owaContext)
		{
			HttpRequest request = owaContext.HttpContext.Request;
			owaContext.RequestType = Utilities.GetRequestType(request);
			OwaRequestType requestType = owaContext.RequestType;
			if (requestType <= OwaRequestType.LanguagePage)
			{
				switch (requestType)
				{
				case OwaRequestType.Invalid:
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "The request is of an invalid or unrecognized type, sending HTTP 400");
					owaContext.HttpStatusCode = HttpStatusCode.BadRequest;
					return RequestDispatcher.DispatchStepResult.EndResponse;
				case OwaRequestType.Authorize:
					break;
				default:
					switch (requestType)
					{
					case OwaRequestType.ProxyPing:
						RequestDispatcherUtilities.RespondProxyPing(owaContext);
						return RequestDispatcher.DispatchStepResult.EndResponse;
					case OwaRequestType.LanguagePage:
						break;
					default:
						goto IL_C6;
					}
					break;
				}
				owaContext.HttpContext.Response.AppendHeader("X-OWA-Version", Globals.ApplicationVersion);
				return RequestDispatcher.DispatchStepResult.Stop;
			}
			if (requestType == OwaRequestType.Resource)
			{
				return RequestDispatcher.DispatchStepResult.Stop;
			}
			switch (requestType)
			{
			case OwaRequestType.HealthPing:
				owaContext.HttpContext.Response.AppendHeader("X-OWA-Version", Globals.ApplicationVersion);
				return RequestDispatcher.DispatchStepResult.Continue;
			case OwaRequestType.ServiceRequest:
				return RequestDispatcher.DispatchStepResult.Continue;
			}
			IL_C6:
			if (!owaContext.HttpContext.Request.IsAuthenticated)
			{
				owaContext.HttpStatusCode = HttpStatusCode.Unauthorized;
				return RequestDispatcher.DispatchStepResult.EndResponse;
			}
			ServerVersion proxyCasVersion = null;
			Uri proxyCasUri = null;
			SecurityIdentifier proxyUserSid = null;
			bool isProxyWebPart = false;
			bool isFromCafe = false;
			OwaDiagnostics.TracePfd(23177, "Determining if this is a proxy request.", new object[0]);
			owaContext.IsProxyRequest = RequestDispatcher.GetIsProxyRequest(request, out proxyCasVersion, out proxyCasUri, out proxyUserSid, out isProxyWebPart, out isFromCafe);
			owaContext.ProxyCasVersion = proxyCasVersion;
			owaContext.ProxyCasUri = proxyCasUri;
			owaContext.ProxyUserSid = proxyUserSid;
			owaContext.IsProxyWebPart = isProxyWebPart;
			owaContext.IsFromCafe = isFromCafe;
			RequestDispatcher.DispatchStepResult dispatchStepResult;
			if (owaContext.IsProxyRequest)
			{
				dispatchStepResult = RequestDispatcher.CheckPermissionsForProxying(owaContext);
				if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
				{
					return dispatchStepResult;
				}
			}
			OwaIdentity logonIdentity = null;
			OwaIdentity mailboxIdentity = null;
			bool isExplicitLogon = false;
			bool isAlternateMailbox = false;
			ExchangePrincipal logonExchangePrincipal = null;
			OwaDiagnostics.TracePfd(31369, "Determining logon and mailbox context based upon request type.", new object[0]);
			dispatchStepResult = RequestDispatcher.GetUserIdentities(owaContext, out logonIdentity, out mailboxIdentity, out isExplicitLogon, out isAlternateMailbox, out logonExchangePrincipal);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			owaContext.LogonIdentity = logonIdentity;
			owaContext.MailboxIdentity = mailboxIdentity;
			owaContext.IsExplicitLogon = isExplicitLogon;
			owaContext.IsAlternateMailbox = isAlternateMailbox;
			owaContext.LogonExchangePrincipal = logonExchangePrincipal;
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			bool flag = Utilities.IsOwaUrl(request.Url, OwaUrl.ProxyEws, false, false);
			if (flag && owaContext.TimeZoneId != null)
			{
				owaContext.RequestType = OwaRequestType.ProxyToEwsEventHandler;
			}
			else
			{
				dispatchStepResult = RequestDispatcher.AcquireAndPreprocessUserContext(owaContext, request);
				if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
				{
					return dispatchStepResult;
				}
			}
			string name;
			if (owaContext.RequestExecution == RequestExecution.Proxy)
			{
				name = "X-OWA-ProxyVersion";
			}
			else
			{
				name = "X-OWA-Version";
			}
			owaContext.HttpContext.Response.AppendHeader(name, Globals.ApplicationVersion);
			RequestDispatcher.SetTimeoutForRequest(owaContext.HttpContext, owaContext.RequestType);
			return RequestDispatcher.DoFinalDispatch(owaContext);
		}

		private static RequestDispatcher.DispatchStepResult AcquireAndPreprocessUserContext(OwaContext owaContext, HttpRequest request)
		{
			RequestDispatcher.DispatchStepResult dispatchStepResult = RequestDispatcher.DispatchStepResult.Continue;
			UserContext userContext = null;
			OwaDiagnostics.TracePfd(17033, "Attempting to locate user in cache.", new object[0]);
			UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(owaContext);
			if (userContextCookie != null)
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<UserContextCookie>(0L, "Found cookie in the request: {0}", userContextCookie);
				Utilities.DeleteCookie(owaContext.HttpContext.Response, userContextCookie.CookieName);
				UserContextCookie userContextCookie2 = userContextCookie.CloneWithNewCanary();
				owaContext.HttpContext.Response.Cookies.Set(userContextCookie2.HttpCookie);
				userContextCookie = userContextCookie2;
				UserContextKey userContextKey = UserContextKey.CreateFromCookie(userContextCookie);
				userContext = UserContextManager.TryGetUserContextFromCache(userContextKey);
				bool flag = false;
				if (userContext == null)
				{
					OwaRWLockWrapper userContextKeyLock = UserContextManager.GetUserContextKeyLock(userContextKey.ToString());
					try
					{
						userContextKeyLock.LockWriterElastic(Globals.UserContextLockTimeout);
						userContext = UserContextManager.TryGetUserContextFromCache(userContextKey);
						if (userContext == null)
						{
							ExTraceGlobals.CoreTracer.TraceDebug(0L, "User context was not found in the cache");
							if (OwaEventHandlerBase.ShouldIgnoreRequest(owaContext, userContext))
							{
								owaContext.HttpStatusCode = HttpStatusCode.BadRequest;
								return RequestDispatcher.DispatchStepResult.EndResponse;
							}
							dispatchStepResult = RequestDispatcher.PrepareRequestWithoutSession(owaContext, userContextCookie);
							flag = true;
						}
					}
					finally
					{
						if ((!flag || owaContext.UserContext == null || !owaContext.UserContext.LockedByCurrentThread()) && userContextKeyLock.IsWriterLockHeld)
						{
							userContextKeyLock.ReleaseWriterLock();
						}
					}
				}
				if (!flag)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<UserContext>(0L, "User context was found in the cache, User context instance={0}", userContext);
					dispatchStepResult = RequestDispatcher.PrepareRequestWithSession(owaContext, userContext);
				}
				if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
				{
					return dispatchStepResult;
				}
				userContext = owaContext.UserContext;
				if (userContext == null)
				{
					throw new OwaInvalidOperationException("Expected to have user context");
				}
			}
			else
			{
				ExTraceGlobals.UserContextTracer.TraceDebug(0L, "No user context cookie found in the request, creating new session...");
				dispatchStepResult = RequestDispatcher.PrepareRequestWithoutSession(owaContext, userContextCookie);
				if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
				{
					return dispatchStepResult;
				}
			}
			userContext = owaContext.UserContext;
			if (userContext.RequestExecution == RequestExecution.Local)
			{
				owaContext.AcquireBudgetAndStartTiming();
			}
			string str = (request.HttpMethod != null) ? request.HttpMethod : "<null>";
			string str2 = (request.Url.PathAndQuery != null) ? request.Url.PathAndQuery : "<null>";
			userContext.LogBreadcrumb(str + " " + str2);
			if (!userContext.IsOWAEnabled && !owaContext.IsMowa)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "OWA Disabled: redirecting to error page");
				owaContext.ErrorString = string.Format(LocalizedStrings.GetNonEncoded(-1030227755), userContext.MailboxIdentity.SafeGetRenderableName());
				return RequestDispatcher.DispatchStepResult.RewritePathToError;
			}
			dispatchStepResult = RequestDispatcher.PreProcessUserInitiatedRequests(owaContext);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			userContext.Configuration = OwaConfigurationManager.Configuration;
			UserSettings settingToReload = (UserSettings)0U;
			if (RequestDispatcher.TryReadUpdatedUserSettingsCookie(owaContext, out settingToReload))
			{
				if (owaContext.RequestExecution == RequestExecution.Local)
				{
					userContext.ReloadUserSettings(owaContext, settingToReload);
				}
				if (!userContext.IsProxy)
				{
					owaContext.TimeZoneId = userContext.TimeZone.Id;
				}
				Utilities.DeleteCookie(owaContext.HttpContext.Response, "UpdatedUserSettings");
			}
			return dispatchStepResult;
		}

		private static RequestDispatcher.DispatchStepResult PrepareRequestWithSession(OwaContext owaContext, UserContext userContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.PrepareRequestWithSession");
			if (!owaContext.LogonIdentity.IsEqualsTo(userContext.LogonIdentity))
			{
				throw new OwaInvalidOperationException("Identities in userContext and owaContext don't match.");
			}
			if (!owaContext.MailboxIdentity.IsEqualsTo(userContext.MailboxIdentity))
			{
				throw new OwaInvalidOperationException("Identities in userContext and owaContext don't match.");
			}
			owaContext.UserContext = userContext;
			owaContext.ExchangePrincipal = userContext.ExchangePrincipal;
			owaContext.RequestExecution = userContext.RequestExecution;
			owaContext.ProxyUriQueue = userContext.ProxyUriQueue;
			OwaDiagnostics.CheckAndSetThreadTracing(userContext.ExchangePrincipal.LegacyDn);
			if (owaContext.RequestExecution != RequestExecution.Local)
			{
				owaContext.SecondCasUri = userContext.ProxyUriQueue.Head;
				ExTraceGlobals.CoreTracer.TraceDebug<ProxyUri>(0L, "SecondCasUri={0}", owaContext.SecondCasUri);
			}
			if (owaContext.RequestType == OwaRequestType.ProxyLogon)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "We are receiving a proxy logon request but we already have a user context, returning HTTP 241 (nothing to do)");
				owaContext.HttpStatusCode = (HttpStatusCode)241;
				return RequestDispatcher.DispatchStepResult.EndResponse;
			}
			RequestDispatcher.DispatchStepResult dispatchStepResult = RequestDispatcher.DispatchIfLogoffRequest(owaContext);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			dispatchStepResult = RequestDispatcher.DispatchIfLanguagePost(owaContext);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			OwaDiagnostics.TracePfd(17673, "Determining the culture setting of the user.", new object[0]);
			dispatchStepResult = RequestDispatcher.ValidateAndSetThreadCulture(owaContext);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			if (owaContext.RequestExecution == RequestExecution.Redirect)
			{
				return RequestDispatcher.SetRedirectDestinationUrl(owaContext);
			}
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static RequestDispatcher.DispatchStepResult PrepareRequestWithoutSession(OwaContext owaContext, UserContextCookie userContextCookie)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.PrepareRequestWithoutSession");
			if (owaContext.IsExplicitLogon && owaContext.MailboxIdentity.IsPartial)
			{
				OwaMiniRecipientIdentity owaMiniRecipientIdentity = owaContext.MailboxIdentity as OwaMiniRecipientIdentity;
				try
				{
					owaMiniRecipientIdentity.UpgradePartialIdentity();
				}
				catch (DataValidationException ex)
				{
					PropertyValidationError propertyValidationError = ex.Error as PropertyValidationError;
					if (propertyValidationError == null || propertyValidationError.PropertyDefinition != MiniRecipientSchema.Languages)
					{
						throw;
					}
					OWAMiniRecipient owaminiRecipient = owaContext.MailboxIdentity.FixCorruptOWAMiniRecipientCultureEntry();
					if (owaminiRecipient != null)
					{
						owaContext.MailboxIdentity = OwaMiniRecipientIdentity.CreateFromOWAMiniRecipient(owaminiRecipient);
					}
				}
			}
			ExchangePrincipal exchangePrincipal = null;
			RequestDispatcher.DispatchStepResult dispatchStepResult = RequestDispatcher.GetExchangePrincipal(owaContext, out exchangePrincipal);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			owaContext.ExchangePrincipal = exchangePrincipal;
			OwaDiagnostics.CheckAndSetThreadTracing(exchangePrincipal.LegacyDn);
			owaContext.RequestExecution = RequestExecution.Local;
			dispatchStepResult = RequestDispatcher.DispatchIfLogoffRequest(owaContext);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			dispatchStepResult = RequestDispatcher.DispatchIfProxyLogon(owaContext);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			if (owaContext.RequestExecution == RequestExecution.Local)
			{
				dispatchStepResult = RequestDispatcher.DispatchIfLanguagePost(owaContext);
				if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
				{
					return dispatchStepResult;
				}
			}
			OwaDiagnostics.TracePfd(32009, "Determining the culture setting of the user.", new object[0]);
			dispatchStepResult = RequestDispatcher.ValidateAndSetThreadCulture(owaContext);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			if (owaContext.LogonIdentity.IsPartial)
			{
				if (owaContext.ReceivedSerializedClientSecurityContext == null)
				{
					owaContext.HttpStatusCode = (HttpStatusCode)441;
					return RequestDispatcher.DispatchStepResult.EndResponse;
				}
				ClientSecurityContext clientSecurityContext = new ClientSecurityContext(owaContext.ReceivedSerializedClientSecurityContext, AuthzFlags.AuthzSkipTokenGroups);
				OwaClientSecurityContextIdentity owaClientSecurityContextIdentity = owaContext.LogonIdentity as OwaClientSecurityContextIdentity;
				owaClientSecurityContextIdentity.UpgradePartialIdentity(clientSecurityContext, owaContext.ReceivedSerializedClientSecurityContext.LogonName, owaContext.ReceivedSerializedClientSecurityContext.AuthenticationType);
			}
			UserContext userContext = null;
			UserContextKey userContextKey = null;
			if (userContextCookie != null)
			{
				userContextKey = UserContextKey.CreateFromCookie(userContextCookie);
			}
			OwaDiagnostics.TracePfd(32393, "Creating a new user context.", new object[0]);
			dispatchStepResult = RequestDispatcher.CreateUserContext(owaContext, userContextKey, out userContext);
			if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
			{
				return dispatchStepResult;
			}
			if (userContextCookie == null)
			{
				string cookieId = null;
				if (owaContext.IsDifferentMailbox)
				{
					cookieId = Utilities.GetNewGuid();
				}
				userContextCookie = UserContextCookie.CreateFromKey(cookieId, userContext.Key);
			}
			owaContext.HttpContext.Response.Cookies.Add(userContextCookie.HttpCookie);
			if (!userContext.IsProxy)
			{
				owaContext.TimeZoneId = userContext.TimeZone.Id;
			}
			ExTraceGlobals.UserContextTracer.TraceDebug<UserContextCookie>(0L, "Sending a set-cookie header to the client: {0}", userContextCookie);
			if (owaContext.RequestExecution == RequestExecution.Local)
			{
				userContext.CanActAsOwner = userContext.MailboxSession.CanActAsOwner;
				userContext.OnPostLoadUserContext();
			}
			owaContext.UserContext = userContext;
			if (owaContext.RequestExecution == RequestExecution.Proxy)
			{
				dispatchStepResult = RequestDispatcher.DispatchIfLanguagePost(owaContext);
				if (dispatchStepResult != RequestDispatcher.DispatchStepResult.Continue)
				{
					return dispatchStepResult;
				}
			}
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static RequestDispatcher.DispatchStepResult SetRedirectDestinationUrl(OwaContext owaContext)
		{
			if (string.Equals(owaContext.SecondCasUri.Uri.Host, owaContext.HttpContext.Request.Url.Host, StringComparison.OrdinalIgnoreCase))
			{
				if (owaContext.SecondCasUri.FailbackUrl == null)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "No failback URL set, show an error message to let user close all browser window and try again ");
					throw new WrongCASServerBecauseOfOutOfDateDNSCacheException();
				}
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending the client to the failback URL - temporary redirect");
				owaContext.SecondCasUri = ProxyUri.Create(owaContext.SecondCasUri.FailbackUrl.ToString());
				owaContext.SecondCasUri.NeedVdirValidation = false;
				owaContext.SecondCasUri.Parse();
				owaContext.IsTemporaryRedirection = true;
				owaContext.CanAccessUsualAddressInAnHour = true;
				owaContext.DestinationUrl = OwaUrl.CasRedirectPage.ImplicitUrl;
				return RequestDispatcher.DispatchStepResult.RewritePath;
			}
			else
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.RedirectToServer.Enabled)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Data center silent redirect of client");
					owaContext.DestinationUrl = Utilities.RedirectionUrl(owaContext);
					string text = owaContext.HttpContext.Request.QueryString.ToString();
					if (!string.IsNullOrEmpty(text))
					{
						owaContext.DestinationUrl = new UriBuilder(owaContext.DestinationUrl.EndsWith("/") ? owaContext.DestinationUrl : (owaContext.DestinationUrl + "/"))
						{
							Query = text
						}.Uri.AbsoluteUri;
					}
					return RequestDispatcher.DispatchStepResult.RedirectToUrl;
				}
				ITopologyConfigurationSession topologyConfigurationSession = Utilities.CreateTopologyConfigurationSessionScopedToRootOrg(true, ConsistencyMode.IgnoreInvalid);
				Server server = topologyConfigurationSession.FindLocalServer();
				MiniClientAccessServerOrArray miniClientAccessServerOrArray = null;
				bool flag = true;
				if (owaContext.ExchangePrincipal != null)
				{
					if (!topologyConfigurationSession.TryFindByExchangeLegacyDN(owaContext.ExchangePrincipal.MailboxInfo.Location.RpcClientAccessServerLegacyDn, RequestDispatcher.ServerSitePropertyDefinition, out miniClientAccessServerOrArray))
					{
						miniClientAccessServerOrArray = null;
					}
					else if (server != null)
					{
						flag = ServerVersion.IsEqualMajorVersion(server.VersionNumber, owaContext.ExchangePrincipal.MailboxInfo.Location.ServerVersion);
					}
				}
				if (flag && server != null && miniClientAccessServerOrArray != null && server.ServerSite.Equals(miniClientAccessServerOrArray.ServerSite))
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending the client to the temporary CAS redirection page");
					owaContext.DestinationUrl = OwaUrl.CasRedirectPage.ImplicitUrl;
					owaContext.IsTemporaryRedirection = true;
					owaContext.CanAccessUsualAddressInAnHour = false;
					return RequestDispatcher.DispatchStepResult.RewritePath;
				}
				if (owaContext.IsManualRedirect)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending the client to the CAS redirection page");
					owaContext.DestinationUrl = OwaUrl.CasRedirectPage.ImplicitUrl;
					return RequestDispatcher.DispatchStepResult.RewritePath;
				}
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending the client to the CAS directly - silent redirect");
				owaContext.DestinationUrl = Utilities.RedirectionUrl(owaContext);
				return RequestDispatcher.DispatchStepResult.RedirectToUrl;
			}
		}

		private static RequestDispatcher.DispatchStepResult CheckPermissionsForProxying(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.CheckPermissionsForProxying");
			try
			{
				using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(owaContext.HttpContext.Request.LogonUserIdentity))
				{
					if (!LocalServer.AllowsTokenSerializationBy(clientSecurityContext))
					{
						owaContext.HttpStatusCode = HttpStatusCode.Forbidden;
						return RequestDispatcher.DispatchStepResult.EndResponse;
					}
				}
			}
			catch (DataSourceOperationException exception)
			{
				RequestDispatcher.HandleLocalServerCallFailure(owaContext, exception);
			}
			catch (DataSourceTransientException exception2)
			{
				RequestDispatcher.HandleLocalServerCallFailure(owaContext, exception2);
			}
			catch (DataValidationException exception3)
			{
				RequestDispatcher.HandleLocalServerCallFailure(owaContext, exception3);
			}
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "The calling security principal has permissions for proxying");
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static void HandleLocalServerCallFailure(OwaContext owaContext, LocalizedException exception)
		{
			string text = exception.GetType().ToString() + ": " + exception.ToString();
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorAccessCheck, owaContext.LocalHostName, new object[]
			{
				owaContext.ProxyCasUri,
				owaContext.LocalHostName,
				owaContext.HttpContext.Request.LogonUserIdentity.Name,
				text
			});
			throw new OwaProxyException("Token serialization auth check failed", LocalizedStrings.GetNonEncoded(-2090003338));
		}

		private static RequestDispatcher.DispatchStepResult PreProcessUserInitiatedRequests(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.HandleUserInitiatedRequest");
			if (owaContext.RequestExecution == RequestExecution.Local && RequestDispatcher.IsUserInitiatedRequest(owaContext.HttpContext.Request))
			{
				owaContext.UserContext.UpdateLastUserRequestTime();
			}
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static RequestDispatcher.DispatchStepResult ValidateAndSetThreadCulture(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.ValidateAndSetThreadCulture");
			CultureInfo cultureInfo;
			if (owaContext.UserContext != null)
			{
				cultureInfo = owaContext.UserContext.UserCulture;
			}
			else if (owaContext.LanguagePostUserCulture != null)
			{
				cultureInfo = owaContext.LanguagePostUserCulture;
			}
			else
			{
				cultureInfo = Culture.GetPreferredCulture(owaContext.ExchangePrincipal, owaContext.UserContext);
				if (cultureInfo == null)
				{
					if (owaContext.RequestExecution != RequestExecution.Local)
					{
						cultureInfo = Culture.GetDefaultCulture(owaContext);
					}
					else
					{
						if (owaContext.IsProxyRequest)
						{
							owaContext.HttpStatusCode = (HttpStatusCode)442;
							return RequestDispatcher.DispatchStepResult.EndResponse;
						}
						if (owaContext.RequestType == OwaRequestType.WebPart)
						{
							Culture.InternalSetThreadCulture(Culture.GetDefaultCulture(owaContext));
							ExTraceGlobals.WebPartRequestTracer.TraceDebug(0L, "User doesn't have a culture set.  Web part error response with link to full OWA");
							owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(1683066013), OwaEventHandlerErrorCode.WebPartFirstAccessError);
							return RequestDispatcher.DispatchStepResult.WriteErrorToWebPart;
						}
						if (string.IsNullOrEmpty(owaContext.DestinationUrlQueryString))
						{
							owaContext.DestinationUrlQueryString = owaContext.HttpContext.Request.QueryString.ToString();
						}
						owaContext.DestinationUrl = OwaUrl.LanguagePage.ImplicitUrl;
						return RequestDispatcher.DispatchStepResult.RewritePath;
					}
				}
			}
			Culture.InternalSetThreadCulture(cultureInfo, owaContext);
			if (owaContext.UserContext != null && !owaContext.UserContext.IsProxy && owaContext.UserContext.UserOptions != null)
			{
				cultureInfo.DateTimeFormat.ShortTimePattern = owaContext.UserContext.UserOptions.TimeFormat;
				cultureInfo.DateTimeFormat.ShortDatePattern = owaContext.UserContext.UserOptions.DateFormat;
			}
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static bool TryReadUpdatedUserSettingsCookie(OwaContext owaContext, out UserSettings settings)
		{
			settings = (UserSettings)0U;
			HttpCookie httpCookie = owaContext.HttpContext.Request.Cookies["UpdatedUserSettings"];
			if (httpCookie == null)
			{
				return false;
			}
			string value = httpCookie.Value;
			bool result;
			try
			{
				settings = (UserSettings)Convert.ToUInt32(value);
				result = true;
			}
			catch (FormatException)
			{
				throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "The value of the cookie {0} is invalid.", new object[]
				{
					"UpdatedUserSettings"
				}));
			}
			catch (OverflowException)
			{
				throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "The value of the cookie {0} is invalid.", new object[]
				{
					"UpdatedUserSettings"
				}));
			}
			return result;
		}

		private static bool GetIsProxyRequest(HttpRequest request, out ServerVersion proxyCasVersion, out Uri proxyCasUri, out SecurityIdentifier proxyUserSid, out bool isProxyWebPart, out bool isFromCafe)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.GetIsProxyRequest");
			proxyCasVersion = null;
			proxyCasUri = null;
			proxyUserSid = null;
			isFromCafe = false;
			string text = request.Headers["X-OWA-ProxyVersion"];
			if (text != null)
			{
				proxyCasVersion = ServerVersion.CreateFromVersionString(text);
				if (proxyCasVersion == null)
				{
					throw new OwaInvalidRequestException(string.Format("The format of the header {0}is wrong", "X-OWA-ProxyVersion"));
				}
			}
			string text2 = request.Headers["X-OWA-ProxyUri"];
			if (text2 != null)
			{
				proxyCasUri = ProxyUtilities.TryCreateCasUri(text2, true);
				if (proxyCasUri == null)
				{
					throw new OwaInvalidRequestException(string.Format("The format of the header {0}is wrong", "X-OWA-ProxyUri"));
				}
			}
			string text3 = request.Headers["X-OWA-ProxySid"];
			if (text3 != null)
			{
				try
				{
					proxyUserSid = new SecurityIdentifier(text3);
				}
				catch (ArgumentException innerException)
				{
					throw new OwaInvalidRequestException(string.Format("The format of the {0} header is wrong", "X-OWA-ProxySid"), innerException);
				}
			}
			isFromCafe = string.Equals(request.Headers["X-IsFromCafe"], "1", StringComparison.OrdinalIgnoreCase);
			if (proxyCasVersion != null && proxyCasUri != null && proxyUserSid != null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "The request is a proxy request");
				ExTraceGlobals.CoreDataTracer.TraceDebug<ServerVersion, Uri, SecurityIdentifier>(0L, "proxyCasVersion={0}, proxyCasUri={1}, proxyUserSid={2}", proxyCasVersion, proxyCasUri, proxyUserSid);
				isProxyWebPart = (0 == string.CompareOrdinal(request.Headers["X-OWA-ProxyWebPart"], "1"));
				return true;
			}
			if (proxyCasVersion != null || proxyCasUri != null || proxyUserSid != null)
			{
				throw new OwaInvalidRequestException("Some required proxy headers are missing");
			}
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "The request is not a proxy request");
			isProxyWebPart = false;
			return false;
		}

		internal static bool IsUserInitiatedRequest(HttpRequest request)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.IsUserInitiatedRequest");
			string text = request.Headers["X-UserActivity"];
			if (text != null && text == "0")
			{
				return false;
			}
			string queryStringParameter = Utilities.GetQueryStringParameter(request, "UA", false);
			return queryStringParameter == null || !(queryStringParameter == "0");
		}

		private static RequestDispatcher.DispatchStepResult GetUserIdentities(OwaContext owaContext, out OwaIdentity logonIdentity, out OwaIdentity mailboxIdentity, out bool isExplicitLogon, out bool isAlternateMailbox, out ExchangePrincipal logonExchangePrincipal)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.GetUserIdentities");
			logonIdentity = null;
			mailboxIdentity = null;
			isExplicitLogon = false;
			isAlternateMailbox = false;
			logonExchangePrincipal = null;
			IIdentity identity = owaContext.HttpContext.User.Identity;
			if (identity is LiveIDIdentity)
			{
				logonIdentity = OwaLiveIDIdentity.CreateFromLiveIDIdentity(identity as LiveIDIdentity);
				ExTraceGlobals.CoreTracer.TraceDebug<IIdentity>(0L, "Created full logon identity of type OwaLiveIDIdentity with LiveIDIdentity {0}", identity);
			}
			else if (identity is ClientSecurityContextIdentity)
			{
				logonIdentity = OwaClientSecurityContextIdentity.CreateFromClientSecurityContextIdentity(identity as ClientSecurityContextIdentity);
				ExTraceGlobals.CoreTracer.TraceDebug<IIdentity>(0L, "Created full logon identity of type OwaClientSecurityContextIdentity with ClientSecurityContextIdentity {0}", identity);
			}
			else if (owaContext.IsProxyRequest)
			{
				logonIdentity = OwaClientSecurityContextIdentity.CreateFromSecurityIdentifier(owaContext.ProxyUserSid);
				ExTraceGlobals.CoreTracer.TraceDebug<SecurityIdentifier>(0L, "Created partial logon identity of type OwaClientSecurityContextIdentity from proxy user SID={0}", owaContext.ProxyUserSid);
			}
			else
			{
				WindowsIdentity windowsIdentity = identity as WindowsIdentity;
				if (windowsIdentity == null)
				{
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_WebConfigAuthenticationIncorrect, string.Empty, new object[]
					{
						HttpRuntime.AppDomainAppPath
					});
					owaContext.ErrorString = LocalizedStrings.GetNonEncoded(-1556449487);
					return RequestDispatcher.DispatchStepResult.RewritePathToError;
				}
				if (windowsIdentity.IsAnonymous)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Anonymous request received: redirecting to error page");
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_VDirAnonymous);
					owaContext.ErrorString = LocalizedStrings.GetNonEncoded(-1556449487);
					return RequestDispatcher.DispatchStepResult.RewritePathToError;
				}
				logonIdentity = OwaWindowsIdentity.CreateFromWindowsIdentity(windowsIdentity);
				ExTraceGlobals.CoreTracer.TraceDebug<SecurityIdentifier>(0L, "Created full logon identity of type OwaWindowsIdentity from windows identity with SID={0}", windowsIdentity.User);
			}
			if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>((long)owaContext.GetHashCode(), "Request logonIdentity={0}", logonIdentity.SafeGetRenderableName());
			}
			HttpRequest request = owaContext.HttpContext.Request;
			string text = HttpUtility.UrlDecode(request.Headers["X-OWA-ExplicitLogonUser"]);
			if (!string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Created partial mailbox identity from SMTP address={0}", text);
				mailboxIdentity = OwaIdentity.CreateOwaIdentityFromSmtpAddress(logonIdentity, text, out logonExchangePrincipal, out isExplicitLogon, out isAlternateMailbox);
			}
			else
			{
				mailboxIdentity = logonIdentity;
				isExplicitLogon = false;
			}
			if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>((long)owaContext.GetHashCode(), "Request mailboxIdentity={0}", mailboxIdentity.SafeGetRenderableName());
			}
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static RequestDispatcher.DispatchStepResult GetExchangePrincipal(OwaContext owaContext, out ExchangePrincipal exchangePrincipal)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.GetExchangePrincipal");
			exchangePrincipal = null;
			if (owaContext.LogonIdentity == owaContext.MailboxIdentity && owaContext.LogonExchangePrincipal != null)
			{
				exchangePrincipal = owaContext.LogonExchangePrincipal;
			}
			else
			{
				bool flag = false;
				try
				{
					exchangePrincipal = owaContext.MailboxIdentity.CreateExchangePrincipal();
				}
				catch (ObjectNotFoundException)
				{
					flag = true;
				}
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.RedirectToServer.Enabled && (flag || (exchangePrincipal != null && exchangePrincipal.MailboxInfo.IsRemote)))
				{
					OWAMiniRecipient owaminiRecipient = owaContext.MailboxIdentity.GetOWAMiniRecipient();
					if (owaminiRecipient != null)
					{
						ProxyUri owaUrlForExternalMailbox = RequestDispatcher.GetOwaUrlForExternalMailbox(owaminiRecipient);
						if (owaUrlForExternalMailbox != null)
						{
							owaContext.SecondCasUri = owaUrlForExternalMailbox;
							return RequestDispatcher.SetRedirectDestinationUrl(owaContext);
						}
						if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "User {0} doesn't have a mailbox, sending client to error page", owaContext.MailboxIdentity.SafeGetRenderableName());
						}
					}
					else
					{
						ExTraceGlobals.CoreTracer.TraceDebug<SecurityIdentifier>(0L, "Cannot find OWAMiniRecipient object for {0} to get OWA URL of external Mailbox user.", owaContext.MailboxIdentity.UserSid);
					}
				}
			}
			if (exchangePrincipal == null)
			{
				owaContext.ErrorString = string.Format(LocalizedStrings.GetNonEncoded(-765910865), owaContext.MailboxIdentity.SafeGetRenderableName());
				return RequestDispatcher.DispatchStepResult.RewritePathToError;
			}
			if (string.IsNullOrEmpty(exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()))
			{
				string text = owaContext.MailboxIdentity.SafeGetRenderableName();
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "User {0} doesn't have a mailbox, sending client to error page", text);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_PrimarySmtpAddressUnavailable, string.Empty, new object[]
				{
					text,
					text,
					text
				});
				owaContext.ErrorString = LocalizedStrings.GetNonEncoded(992237313);
				return RequestDispatcher.DispatchStepResult.RewritePathToError;
			}
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static ProxyUri GetOwaUrlForExternalMailbox(OWAMiniRecipient owaMiniRecipient)
		{
			if (!(owaMiniRecipient.ExternalEmailAddress != null))
			{
				return null;
			}
			SmtpProxyAddress smtpProxyAddress = owaMiniRecipient.ExternalEmailAddress as SmtpProxyAddress;
			if (!(smtpProxyAddress != null) || string.IsNullOrEmpty(smtpProxyAddress.AddressString))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Not checking organization relationship for external email address for {1} because it is not SMTP address or it is not present.", owaMiniRecipient.Alias);
				return null;
			}
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(owaMiniRecipient.OrganizationId);
			string domain = ((SmtpAddress)smtpProxyAddress).Domain;
			OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(domain);
			if (organizationRelationship == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Cannot find OrganizationRelationship object for {0}", domain);
				throw new OwaURLIsOutOfDateException();
			}
			if (organizationRelationship.TargetOwaURL == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<ObjectId>(0L, "The TargetOwaURL of OrganizationRelationship {0} is null", organizationRelationship.Identity);
				throw new OwaURLIsOutOfDateException();
			}
			string text = organizationRelationship.TargetOwaURL.ToString();
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<ObjectId>(0L, "The TargetOwaURL of OrganizationRelationship {0} is empty", organizationRelationship.Identity);
				throw new OwaURLIsOutOfDateException();
			}
			ProxyUri proxyUri = ProxyUri.Create(text);
			proxyUri.Parse();
			if (!proxyUri.IsValid)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, ObjectId>(0L, "The TargetOwaURL {0} of OrganizationRelationship {1} is an invalid URL for OWA", text, organizationRelationship.Identity);
				throw new OwaURLIsOutOfDateException();
			}
			return proxyUri;
		}

		private static RequestDispatcher.DispatchStepResult CreateUserContext(OwaContext owaContext, UserContextKey userContextKey, out UserContext userContext)
		{
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "RequestDispatcher.CreateUserContext");
			userContext = null;
			if (userContextKey == null)
			{
				userContextKey = UserContextKey.CreateNew(owaContext);
				ExTraceGlobals.UserContextTracer.TraceDebug<UserContextKey>(0L, "Creating new user context key: {0}", userContextKey);
			}
			else
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<UserContextKey>(0L, "Reusing user context key: {0}", userContextKey);
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			userContext = null;
			try
			{
				userContext = new UserContext(userContextKey);
				userContext.UserCulture = Culture.InternalGetThreadCulture(owaContext);
				if (owaContext.RequestExecution == RequestExecution.Local)
				{
					UserContextLoadResult userContextLoadResult = userContext.Load(owaContext);
					if (userContextLoadResult == UserContextLoadResult.Success)
					{
						ExTraceGlobals.UserContextTracer.TraceDebug<UserContext>(0L, "User context was succesfully loaded. User context instance={0}", userContext);
						userContext.TraceObject();
						if (owaContext.IsProxyRequest)
						{
							userContext.ShouldDisableUncAndWssFeatures = true;
						}
					}
					else
					{
						flag4 = true;
						ExTraceGlobals.UserContextTracer.TraceDebug<UserContextLoadResult>(0L, "User context failed to load. Result = {0}", userContextLoadResult);
						if (userContextLoadResult == UserContextLoadResult.InvalidTimeZoneKeyName)
						{
							if (owaContext.IsProxyRequest)
							{
								owaContext.HttpStatusCode = (HttpStatusCode)442;
								return RequestDispatcher.DispatchStepResult.EndResponse;
							}
							if (owaContext.RequestType == OwaRequestType.WebPart)
							{
								ExTraceGlobals.WebPartRequestTracer.TraceDebug(0L, "User doesn't have a valid timezone set.  Web part error response with link to full OWA");
								owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(1683066013), OwaEventHandlerErrorCode.WebPartFirstAccessError);
								return RequestDispatcher.DispatchStepResult.WriteErrorToWebPart;
							}
							owaContext.DestinationUrl = OwaUrl.LanguagePage.ImplicitUrl;
							return RequestDispatcher.DispatchStepResult.RewritePath;
						}
					}
				}
				if (!userContext.LockedByCurrentThread())
				{
					userContext.Lock();
					flag = true;
				}
				ExTraceGlobals.UserContextTracer.TraceDebug<UserContext>(0L, "Locking of user context succeeded, User context instance={0}", userContext);
				UserContextManager.RecordUserContextCreation(owaContext.LogonIdentity, userContext);
				flag2 = true;
				userContext.LogonIdentity = owaContext.LogonIdentity;
				userContext.MailboxIdentity = owaContext.MailboxIdentity;
				owaContext.LogonIdentity = null;
				owaContext.MailboxIdentity = null;
				userContext.ExchangePrincipal = owaContext.ExchangePrincipal;
				userContext.RequestExecution = owaContext.RequestExecution;
				userContext.ProxyUriQueue = owaContext.ProxyUriQueue;
				userContext.MailboxIdentity.GetOWAMiniRecipient();
				UserContextManager.InsertIntoCache(userContext);
				flag3 = true;
				ExTraceGlobals.UserContextTracer.TraceDebug<UserContext>(0L, "Insertion of user context in cache succeeded, User context instance={0}", userContext);
				userContext.UpdateLastAccessedTime();
			}
			finally
			{
				if (!flag3)
				{
					try
					{
						if (flag2)
						{
							UserContextManager.RecordUserContextDeletion(owaContext.LogonIdentity, userContext);
						}
					}
					finally
					{
						try
						{
							if (!flag && !userContext.LockedByCurrentThread())
							{
								userContext.Lock();
								flag = true;
							}
							userContext.Dispose();
						}
						finally
						{
							if (flag)
							{
								userContext.Unlock();
							}
							if (flag4)
							{
								userContext = null;
							}
						}
					}
				}
			}
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static RequestDispatcher.DispatchStepResult DoFinalDispatch(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.FinalDispatch");
			HttpContext httpContext = owaContext.HttpContext;
			if (owaContext.RequestExecution == RequestExecution.Proxy)
			{
				RequestDispatcherUtilities.SetProxyRequestUrl(owaContext);
				owaContext.DestinationUrl = OwaUrl.ProxyHandler.ImplicitUrl;
				return RequestDispatcher.DispatchStepResult.RewritePath;
			}
			if (owaContext.RequestType == OwaRequestType.ProxyLogon)
			{
				owaContext.HttpStatusCode = (HttpStatusCode)241;
				return RequestDispatcher.DispatchStepResult.EndResponse;
			}
			if (owaContext.RequestType == OwaRequestType.LanguagePost)
			{
				if (owaContext.FailedToSaveUserCulture)
				{
					owaContext.DestinationUrl = OwaUrl.InfoFailedToSaveCulture.GetExplicitUrl(owaContext);
				}
				else if (string.IsNullOrEmpty(owaContext.DestinationUrl))
				{
					owaContext.DestinationUrl = OwaUrl.ApplicationRoot.GetExplicitUrl(owaContext);
				}
				return RequestDispatcher.DispatchStepResult.RedirectToUrl;
			}
			if (owaContext.RequestType == OwaRequestType.Form15 && !RequestDispatcherUtilities.IsDownLevelClient(owaContext.HttpContext, false))
			{
				owaContext.DestinationUrl = OwaUrl.Default15Page.ImplicitUrl;
				if (owaContext.UserContext != null)
				{
					int num;
					owaContext.UserContext.DangerousBeginUnlockedAction(false, out num);
				}
				return RequestDispatcher.DispatchStepResult.RewritePath;
			}
			if (!RequestDispatcherUtilities.IsDownLevelClient(owaContext.HttpContext, false) && Utilities.IsOwa15Url(owaContext.HttpContext.Request))
			{
				if (owaContext.UserContext != null)
				{
					int num2;
					owaContext.UserContext.DangerousBeginUnlockedAction(false, out num2);
				}
				return RequestDispatcher.DispatchStepResult.Continue;
			}
			if (RequestDispatcherUtilities.ShouldDoBasicRegistryLookup(owaContext))
			{
				HttpRequest request = httpContext.Request;
				UserContext userContext = owaContext.UserContext;
				ApplicationElement applicationElement = RequestDispatcherUtilities.GetApplicationElement(request);
				string text = Utilities.GetQueryStringParameter(request, "t", false);
				string state = Utilities.GetQueryStringParameter(request, "s", false);
				string action = Utilities.GetQueryStringParameter(request, "a", false);
				if (Utilities.IsPostRequest(request) && (userContext.IsBasicExperience || string.CompareOrdinal("AttachFileDialog", text) == 0 || string.CompareOrdinal("SetDisplayPicture", text) == 0))
				{
					UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(owaContext);
					Utilities.VerifyCanary(userContextCookie, request);
				}
				owaContext.FormsRegistryContext = new FormsRegistryContext(applicationElement, text, state, action);
				FormValue formValue = null;
				if (owaContext.RequestType == OwaRequestType.WebPart)
				{
					text = null;
					state = null;
					action = null;
					WebPartPreFormAction webPartPreFormAction = new WebPartPreFormAction();
					ExTraceGlobals.WebPartRequestTracer.TraceDebug(0L, "Executing web part pre-form action");
					PreFormActionResponse preFormActionResponse = webPartPreFormAction.Execute(owaContext, out applicationElement, out text, out state, out action);
					if (preFormActionResponse != null)
					{
						applicationElement = preFormActionResponse.ApplicationElement;
						text = preFormActionResponse.Type;
						state = preFormActionResponse.State;
						action = preFormActionResponse.Action;
						owaContext.DestinationUrlQueryString = preFormActionResponse.GetUrl(false);
						formValue = RequestDispatcherUtilities.DoFormsRegistryLookup(owaContext.UserContext, applicationElement, text, action, state);
						if (formValue == null)
						{
							owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-1242252459), OwaEventHandlerErrorCode.WebPartSegmentationError);
						}
					}
				}
				else
				{
					OwaDiagnostics.TracePfd(31369, "The request is for a form. Performing forms registry lookup.", new object[0]);
					formValue = RequestDispatcherUtilities.DoFormsRegistryLookup(owaContext.UserContext, applicationElement, text, action, state);
					if (applicationElement == ApplicationElement.PreFormAction)
					{
						if (formValue == null)
						{
							throw new OwaInvalidRequestException("Lookup for preform action failed");
						}
						text = null;
						state = null;
						action = null;
						IPreFormAction preFormAction = null;
						try
						{
							preFormAction = (IPreFormAction)Activator.CreateInstance((Type)formValue.Value);
						}
						catch (InvalidCastException)
						{
							throw new OwaInvalidRequestException("No pre form type found");
						}
						catch (TypeLoadException)
						{
							throw new OwaInvalidRequestException("No pre form type found");
						}
						catch (TargetInvocationException)
						{
							throw new OwaInvalidRequestException("No pre form type found");
						}
						catch (MemberAccessException)
						{
							throw new OwaInvalidRequestException("No pre form type found");
						}
						catch (ArgumentException)
						{
							throw new OwaInvalidRequestException("No pre form type found");
						}
						ExTraceGlobals.CoreTracer.TraceDebug(0L, "Executing pre-form action");
						PreFormActionResponse preFormActionResponse2 = preFormAction.Execute(owaContext, out applicationElement, out text, out state, out action);
						if (preFormActionResponse2 != null)
						{
							applicationElement = preFormActionResponse2.ApplicationElement;
							text = preFormActionResponse2.Type;
							state = preFormActionResponse2.State;
							action = preFormActionResponse2.Action;
							owaContext.DestinationUrlQueryString = preFormActionResponse2.GetUrl();
						}
						formValue = RequestDispatcherUtilities.DoFormsRegistryLookup(owaContext.UserContext, applicationElement, text, action, state);
					}
				}
				owaContext.FormsRegistryContext.ApplicationElement = applicationElement;
				owaContext.FormsRegistryContext.Type = text;
				owaContext.FormsRegistryContext.State = state;
				owaContext.FormsRegistryContext.Action = action;
				if (formValue == null)
				{
					if (owaContext.RequestType == OwaRequestType.WebPart)
					{
						ExTraceGlobals.WebPartRequestTracer.TraceDebug(0L, "Invalid web part request");
						return RequestDispatcher.DispatchStepResult.WriteErrorToWebPart;
					}
					owaContext.ErrorString = LocalizedStrings.GetNonEncoded(-884316585);
					return RequestDispatcher.DispatchStepResult.RewritePathToError;
				}
				else
				{
					owaContext.LoadedByFormsRegistry = true;
					string text2 = (string)formValue.Value;
					if (owaContext.OwaPerformanceData != null)
					{
						owaContext.OwaPerformanceData.SetFormRequestType(text2);
					}
					if (RequestDispatcher.IsUserControl(text2))
					{
						string fileName = Path.GetFileName(text2);
						bool flag = Utilities.IsSubPageContentRequest(owaContext);
						if (flag)
						{
							text2 = "forms/premium/SubPageEventHandler.aspx";
						}
						else
						{
							text2 = "forms/premium/SubPageContainer.aspx";
						}
						if (string.IsNullOrEmpty(owaContext.DestinationUrlQueryString))
						{
							owaContext.DestinationUrlQueryString = httpContext.Request.QueryString.ToString();
						}
						owaContext.DestinationUrlQueryString = string.Format("{0}{1}subpage={2}", owaContext.DestinationUrlQueryString, string.IsNullOrEmpty(owaContext.DestinationUrlQueryString) ? string.Empty : "&", Utilities.UrlEncode(fileName));
					}
					Uri uri = Utilities.TryParseUri(text2);
					if (formValue.IsCustomForm)
					{
						if (string.IsNullOrEmpty(owaContext.DestinationUrlQueryString))
						{
							owaContext.DestinationUrlQueryString = httpContext.Request.QueryString.ToString();
						}
						owaContext.DestinationUrlQueryString = owaContext.DestinationUrlQueryString + (string.IsNullOrEmpty(owaContext.DestinationUrlQueryString) ? string.Empty : "&") + "ea=" + Utilities.UrlEncode(owaContext.UserContext.MailboxIdentity.GetOWAMiniRecipient().PrimarySmtpAddress.ToString());
					}
					if (uri != null)
					{
						owaContext.DestinationUrl = text2;
						if (!string.IsNullOrEmpty(owaContext.DestinationUrlQueryString))
						{
							owaContext.DestinationUrl = owaContext.DestinationUrl + (string.IsNullOrEmpty(uri.Query) ? "?" : "&") + owaContext.DestinationUrlQueryString;
						}
						return RequestDispatcher.DispatchStepResult.RedirectToUrl;
					}
					owaContext.DestinationUrl = OwaUrl.ApplicationRoot.ImplicitUrl + text2;
					return RequestDispatcher.DispatchStepResult.RewritePath;
				}
			}
			else
			{
				if (owaContext.RequestType == OwaRequestType.KeepAlive)
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Executing a KeepAlive request.");
					owaContext.HttpStatusCode = HttpStatusCode.OK;
					return RequestDispatcher.DispatchStepResult.EndResponse;
				}
				return RequestDispatcher.DispatchStepResult.Continue;
			}
		}

		private static void SetTimeoutForRequest(HttpContext httpContext, OwaRequestType requestType)
		{
			bool flag = false;
			if (requestType == OwaRequestType.Oeh)
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(httpContext.Request, "ns");
				string queryStringParameter2 = Utilities.GetQueryStringParameter(httpContext.Request, "ev");
				if (string.CompareOrdinal("DocumentLibrary", queryStringParameter) == 0 && string.CompareOrdinal("SendByEmail", queryStringParameter2) == 0)
				{
					flag = true;
				}
				else if (string.CompareOrdinal("EditMessage", queryStringParameter) == 0 && (string.CompareOrdinal("SendMime", queryStringParameter2) == 0 || string.CompareOrdinal("SaveMime", queryStringParameter2) == 0))
				{
					flag = true;
				}
				else if (string.CompareOrdinal("SMime", queryStringParameter) == 0 && string.CompareOrdinal("UploadEmbeddedItem", queryStringParameter2) == 0)
				{
					flag = true;
				}
			}
			else if (requestType == OwaRequestType.Form14)
			{
				string queryStringParameter3 = Utilities.GetQueryStringParameter(httpContext.Request, "t", false);
				string queryStringParameter4 = Utilities.GetQueryStringParameter(httpContext.Request, "a", false);
				if (string.IsNullOrEmpty(queryStringParameter3))
				{
					return;
				}
				ApplicationElement applicationElement = RequestDispatcherUtilities.GetApplicationElement(httpContext.Request);
				if (applicationElement == ApplicationElement.Dialog && string.CompareOrdinal("POST", httpContext.Request.HttpMethod) == 0 && (string.CompareOrdinal("AttachFileDialog", queryStringParameter3) == 0 || string.CompareOrdinal("SetDisplayPicture", queryStringParameter3) == 0 || (string.CompareOrdinal("Attach", queryStringParameter3) == 0 && string.CompareOrdinal("Add", queryStringParameter4) == 0)))
				{
					flag = true;
				}
			}
			else if (requestType == OwaRequestType.Attachment)
			{
				flag = true;
			}
			if (flag)
			{
				httpContext.Server.ScriptTimeout = 3600;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "Request timeout is going to be: {0}", httpContext.Server.ScriptTimeout);
		}

		private static RequestDispatcher.DispatchStepResult DispatchIfLogoffRequest(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.DispatchIfLogoffRequest");
			if (owaContext.RequestType != OwaRequestType.Logoff)
			{
				return RequestDispatcher.DispatchStepResult.Continue;
			}
			OwaDiagnostics.TracePfd(21769, "Dispatching logoff request.", new object[0]);
			if (owaContext.UserContext != null)
			{
				RequestDispatcher.DoLogoff(owaContext);
			}
			return RequestDispatcher.DispatchStepResult.RedirectToUrl;
		}

		private static void DoLogoff(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.DoLogoff");
			HttpContext httpContext = owaContext.HttpContext;
			UserContext userContext = owaContext.UserContext;
			UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(owaContext);
			UserContextKey userContextKey = UserContextKey.CreateFromCookie(userContextCookie);
			if (!userContext.IsProxy && userContext.IsBasicExperience)
			{
				if (userContext.RequestExecution == RequestExecution.Proxy)
				{
					Utilities.VerifyCanary(userContext.ProxyUserContextCookie, httpContext.Request);
				}
				else if (userContext.RequestExecution == RequestExecution.Local)
				{
					Utilities.VerifyCanary(userContextCookie, httpContext.Request);
				}
			}
			try
			{
				if (userContext.RequestExecution == RequestExecution.Proxy)
				{
					RequestDispatcher.SendRemoteLogoff(owaContext, userContext);
				}
				ExTraceGlobals.UserContextTracer.TraceDebug<UserContext>(0L, "Found user context in the cache, User context instance={0}", userContext);
				userContext.State = UserContextState.MarkedForLogoff;
				if (!userContext.IsProxy)
				{
					if (userContext.UserOptions.EmptyDeletedItemsOnLogoff)
					{
						ExTraceGlobals.UserContextTracer.TraceDebug(0L, "Emptying deleted items folder.");
						userContext.MailboxSession.DeleteAllObjects(DeleteItemFlags.SoftDelete, userContext.GetDeletedItemsFolderId(userContext.MailboxSession).StoreObjectId);
					}
					userContext.DisconnectMailboxSession();
				}
			}
			finally
			{
				Utilities.DeleteCookie(httpContext.Response, userContextCookie.CookieName);
				if (userContextKey.UserContextId == null)
				{
					ExTraceGlobals.UserContextTracer.TraceDebug(0L, "User context id couldn't be retrieved. Logoff can't be performed");
				}
				else
				{
					object obj = HttpRuntime.Cache.Remove(userContextKey.ToString());
					ExTraceGlobals.UserContextTracer.TraceDebug(0L, (obj == null) ? "The userContext was already deleted from the cache" : "Context succesfully deleted from the cache");
				}
			}
		}

		private static void SendRemoteLogoff(OwaContext owaContext, UserContext userContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.SendRemoteLogoff");
			if (userContext.ProxyUserContextCookie == null)
			{
				return;
			}
			HttpWebResponse httpWebResponse = null;
			try
			{
				UriBuilder uriBuilder = new UriBuilder(owaContext.SecondCasUri.Uri);
				uriBuilder.Path = OwaUrl.Logoff.GetExplicitUrl(owaContext);
				string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "canary", false);
				if (queryStringParameter != null)
				{
					uriBuilder.Query = "canary=" + queryStringParameter;
				}
				Uri uri = uriBuilder.Uri;
				HttpWebRequest proxyRequestInstance = ProxyUtilities.GetProxyRequestInstance(owaContext.HttpContext.Request, owaContext, uri);
				proxyRequestInstance.Method = "GET";
				httpWebResponse = (HttpWebResponse)proxyRequestInstance.GetResponse();
			}
			catch (WebException ex)
			{
				int arg = -1;
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse httpWebResponse2 = (HttpWebResponse)ex.Response;
					arg = (int)httpWebResponse2.StatusCode;
				}
				ExTraceGlobals.CoreTracer.TraceDebug<string, WebExceptionStatus, int>(0L, "The remote logoff call failed (ignoring). exception={0}, status={1}, http status code={2}", ex.Message, ex.Status, arg);
			}
			finally
			{
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
					httpWebResponse = null;
				}
			}
		}

		private static RequestDispatcher.DispatchStepResult DispatchIfProxyLogon(OwaContext owaContext)
		{
			if (owaContext.RequestType != OwaRequestType.ProxyLogon)
			{
				return RequestDispatcher.DispatchStepResult.Continue;
			}
			OwaDiagnostics.TracePfd(29961, "Dispatching proxy logon request.", new object[0]);
			SerializedClientSecurityContext receivedSerializedClientSecurityContext = null;
			try
			{
				receivedSerializedClientSecurityContext = SerializedClientSecurityContext.Deserialize(owaContext.HttpContext.Request.InputStream);
			}
			catch (OwaSecurityContextSidLimitException exception)
			{
				return RequestDispatcher.HandleSecurityContextSidLimitException(owaContext, exception);
			}
			owaContext.ReceivedSerializedClientSecurityContext = receivedSerializedClientSecurityContext;
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static RequestDispatcher.DispatchStepResult DispatchIfLanguagePost(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.DispatchIfLanguagePost");
			if (owaContext.RequestType != OwaRequestType.LanguagePost)
			{
				return RequestDispatcher.DispatchStepResult.Continue;
			}
			OwaDiagnostics.TracePfd(25865, "The request is a post from the language selection page, processing this request...", new object[0]);
			if (owaContext.IsProxyRequest && !owaContext.IsFromCafe)
			{
				return RequestDispatcher.DispatchLanguagePostRequestFromProxy(owaContext);
			}
			return RequestDispatcher.DispatchLanguagePostRequest(owaContext);
		}

		private static RequestDispatcher.DispatchStepResult DispatchLanguagePostRequest(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.DispatchLanguagePostRequest");
			HttpRequest request = owaContext.HttpContext.Request;
			if (!Utilities.IsPostRequest(owaContext.HttpContext.Request))
			{
				return RequestDispatcher.DispatchStepResult.Continue;
			}
			CultureInfo cultureInfo;
			string text;
			bool flag;
			string text2;
			RequestDispatcher.GetLanguagePostFormParameters(owaContext, request, out cultureInfo, out text, out flag, out text2);
			if (owaContext.RequestExecution == RequestExecution.Local)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Dispatching language post request locally...");
				return RequestDispatcher.DispatchLanguagePostLocally(owaContext, owaContext.LogonIdentity, cultureInfo, text, flag, text2);
			}
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "Dispatching language post request remotelly (in the second CAS)...");
			owaContext.SetInternalHandlerParameter("culture", cultureInfo);
			owaContext.SetInternalHandlerParameter("tzid", text);
			owaContext.SetInternalHandlerParameter("opt", flag);
			if (!string.IsNullOrEmpty(text2))
			{
				owaContext.SetInternalHandlerParameter("destination", text2);
			}
			owaContext.SetInternalHandlerParameter("identity", owaContext.LogonIdentity);
			owaContext.DestinationUrl = OwaUrl.ProxyLanguagePost.ImplicitUrl;
			return RequestDispatcher.DispatchStepResult.RewritePath;
		}

		private static void GetLanguagePostFormParameters(OwaContext owaContext, HttpRequest request, out CultureInfo culture, out string timeZoneKeyName, out bool isOptimized, out string destination)
		{
			culture = null;
			timeZoneKeyName = string.Empty;
			isOptimized = false;
			destination = string.Empty;
			string text = request.Form["lcid"];
			int lcid = -1;
			if (string.IsNullOrEmpty(text))
			{
				throw new OwaInvalidRequestException("locale ID parameter is missing or empty");
			}
			if (!int.TryParse(text, out lcid) || !Culture.IsSupportedCulture(lcid))
			{
				throw new OwaInvalidRequestException("locale ID parameter is invalid");
			}
			culture = Culture.GetCultureInfoInstance(lcid);
			timeZoneKeyName = request.Form["tzid"];
			if (string.IsNullOrEmpty(timeZoneKeyName))
			{
				throw new OwaInvalidRequestException("timezone ID parameter is missing or empty");
			}
			if (!DateTimeUtilities.IsValidTimeZoneKeyName(timeZoneKeyName))
			{
				throw new OwaInvalidRequestException("timezone ID parameter is invalid");
			}
			if (request.Form["opt"] != null)
			{
				isOptimized = true;
			}
			destination = Utilities.GetFormParameter(owaContext.HttpContext.Request, "destination", false);
		}

		private static RequestDispatcher.DispatchStepResult DispatchLanguagePostRequestFromProxy(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.DispatchLanguagePostRequestFromProxy");
			HttpRequest request = owaContext.HttpContext.Request;
			CultureInfo culture = null;
			string timeZoneKeyName = null;
			bool isOptimized = false;
			string destination = null;
			SerializedClientSecurityContext serializedClientSecurityContext = null;
			try
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Parsing the body of the request...");
				ProxyLanguagePostRequest.ParseProxyLanguagePostBody(owaContext.HttpContext.Request.InputStream, out culture, out timeZoneKeyName, out isOptimized, out destination, out serializedClientSecurityContext);
			}
			catch (OwaSecurityContextSidLimitException exception)
			{
				return RequestDispatcher.HandleSecurityContextSidLimitException(owaContext, exception);
			}
			owaContext.ReceivedSerializedClientSecurityContext = serializedClientSecurityContext;
			RequestDispatcher.DispatchStepResult result;
			using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(serializedClientSecurityContext, AuthzFlags.AuthzSkipTokenGroups))
			{
				OwaClientSecurityContextIdentity owaClientSecurityContextIdentity = null;
				try
				{
					owaClientSecurityContextIdentity = OwaClientSecurityContextIdentity.CreateFromClientSecurityContext(clientSecurityContext, owaContext.ReceivedSerializedClientSecurityContext.LogonName, serializedClientSecurityContext.AuthenticationType);
					RequestDispatcher.DispatchStepResult dispatchStepResult = RequestDispatcher.DispatchLanguagePostLocally(owaContext, owaClientSecurityContextIdentity, culture, timeZoneKeyName, isOptimized, destination);
					result = dispatchStepResult;
				}
				finally
				{
					if (owaClientSecurityContextIdentity != null)
					{
						owaClientSecurityContextIdentity.Dispose();
						owaClientSecurityContextIdentity = null;
					}
				}
			}
			return result;
		}

		private static RequestDispatcher.DispatchStepResult DispatchLanguagePostLocally(OwaContext owaContext, OwaIdentity logonIdentity, CultureInfo culture, string timeZoneKeyName, bool isOptimized, string destination)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.DispatchLanguagePostLocally");
			if (owaContext.UserContext != null)
			{
				RequestDispatcher.DetectUnexpectedLanguagePostRequest(owaContext.UserContext);
			}
			owaContext.LanguagePostUserCulture = culture;
			owaContext.HttpContext.Response.Cookies.Set(new HttpCookie("mkt", culture.Name));
			owaContext.DestinationUrl = destination;
			Culture.InternalSetThreadCulture(culture);
			MailboxSession mailboxSession = null;
			try
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, bool>(0L, "Attempting to save the timeZoneKeyName (tzid={0}) and isOptimized={1} in the mailbox", timeZoneKeyName, isOptimized);
				mailboxSession = logonIdentity.CreateMailboxSession(owaContext.ExchangePrincipal, culture, owaContext.HttpContext.Request);
				if (owaContext.IsExplicitLogon && !mailboxSession.CanActAsOwner)
				{
					throw new OwaExplicitLogonException("User has no access rights to the mailbox", LocalizedStrings.GetNonEncoded(882888134));
				}
				try
				{
					ExTraceGlobals.CoreTracer.TraceDebug<int>(0L, "Trying to save the culture to the AD (lcid={0})", culture.LCID);
					PreferredCultures preferredCultures = new PreferredCultures(owaContext.ExchangePrincipal.PreferredCultures);
					preferredCultures.AddSupportedCulture(culture, new Predicate<CultureInfo>(Culture.IsSupportedCulture));
					Culture.SetPreferredCulture(owaContext.ExchangePrincipal, preferredCultures, mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent));
					owaContext.ExchangePrincipal = owaContext.ExchangePrincipal.WithPreferredCultures(preferredCultures);
				}
				catch (Exception ex)
				{
					if (!(ex is ADOperationException) && !(ex is InvalidOperationException))
					{
						throw;
					}
					owaContext.FailedToSaveUserCulture = true;
					if (ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						StringBuilder stringBuilder = new StringBuilder("Failed to save user's culture in the AD.");
						stringBuilder.Append("\n\nException: ");
						stringBuilder.Append(ex.GetType().ToString());
						stringBuilder.Append("\n");
						stringBuilder.Append(ex.Message);
						stringBuilder.Append(")");
						if (!string.IsNullOrEmpty(ex.StackTrace))
						{
							stringBuilder.Append("\n\nCallstack:\n");
							stringBuilder.Append(ex.StackTrace);
						}
						ExTraceGlobals.CoreTracer.TraceError(0L, stringBuilder.ToString());
					}
				}
				UserOptions userOptions = UserOptions.CreateTemporaryInstance(mailboxSession);
				bool flag = true;
				try
				{
					userOptions.LoadAll();
				}
				catch (QuotaExceededException ex2)
				{
					ExTraceGlobals.UserContextCallTracer.TraceDebug<string>(0L, "RequestDispatcher.DispatchLanguagePostLocally: userOptions.LoadAll failed. Exception: {0}", ex2.Message);
					flag = false;
				}
				userOptions.TimeZone = timeZoneKeyName;
				userOptions.IsOptimizedForAccessibility = isOptimized;
				userOptions.SpellingDictionaryLanguage = 1033;
				if (flag)
				{
					userOptions.CommitChanges();
					WorkingHours.StampOnMailboxIfMissing(mailboxSession, timeZoneKeyName);
				}
				RequestDispatcher.InitializeFavorites(mailboxSession);
			}
			finally
			{
				if (mailboxSession != null)
				{
					Utilities.DisconnectStoreSession(mailboxSession);
					mailboxSession.Dispose();
					mailboxSession = null;
				}
			}
			return RequestDispatcher.DispatchStepResult.Continue;
		}

		private static void DetectUnexpectedLanguagePostRequest(UserContext userContext)
		{
			if (userContext.UserCulture != null && userContext.TimeZone != null)
			{
				throw new OwaInvalidRequestException("The user has a valid Culture and TimeZone, reject this request");
			}
		}

		private static void InitializeFavorites(MailboxSession mailboxSession)
		{
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(null, mailboxSession, NavigationNodeGroupSection.First);
			if (navigationNodeCollection.Count > 0 && navigationNodeCollection[0].Children.Count > 0)
			{
				return;
			}
			using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Inbox, new PropertyDefinition[]
			{
				StoreObjectSchema.RecordKey
			}))
			{
				navigationNodeCollection.AppendFolderToFavorites(folder);
			}
			StoreObjectId storeObjectId = null;
			OutlookSearchFolder outlookSearchFolder = null;
			using (Folder folder2 = Folder.Bind(mailboxSession, DefaultFolderType.SearchFolders))
			{
				PropertyDefinition[] array = new PropertyDefinition[]
				{
					FolderSchema.Id,
					StoreObjectSchema.RecordKey,
					FolderSchema.IsOutlookSearchFolder,
					FolderSchema.OutlookSearchFolderClsId
				};
				object[][] array2;
				using (QueryResult queryResult = folder2.FolderQuery(FolderQueryFlags.None, new TextFilter(FolderSchema.DisplayName, LocalizedStrings.GetNonEncoded(1154673514), MatchOptions.FullString, MatchFlags.IgnoreCase), null, new PropertyDefinition[]
				{
					FolderSchema.Id
				}))
				{
					array2 = Utilities.FetchRowsFromQueryResult(queryResult, 10000);
				}
				if (array2.Length > 0)
				{
					storeObjectId = ((VersionedId)array2[0][0]).ObjectId;
				}
				if (storeObjectId != null)
				{
					Folder folder3 = Folder.Bind(mailboxSession, storeObjectId, array);
					try
					{
						if (Utilities.IsOutlookSearchFolder(folder3))
						{
							outlookSearchFolder = (folder3 as OutlookSearchFolder);
						}
					}
					finally
					{
						if (folder3 != null && outlookSearchFolder == null)
						{
							folder3.Dispose();
						}
					}
				}
				try
				{
					if (outlookSearchFolder == null)
					{
						outlookSearchFolder = OutlookSearchFolder.Create(mailboxSession, LocalizedStrings.GetNonEncoded(1154673514));
						outlookSearchFolder.Save();
						outlookSearchFolder.Load(array);
						List<QueryFilter> list = new List<QueryFilter>();
						StoreObjectId[] array3 = new StoreObjectId[]
						{
							mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems),
							mailboxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail),
							mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts),
							mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox),
							mailboxSession.GetDefaultFolderId(DefaultFolderType.Conflicts),
							mailboxSession.GetDefaultFolderId(DefaultFolderType.LocalFailures),
							mailboxSession.GetDefaultFolderId(DefaultFolderType.SyncIssues)
						};
						foreach (StoreObjectId storeObjectId2 in array3)
						{
							if (storeObjectId2 != null)
							{
								list.Add(new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.ParentItemId, storeObjectId2));
							}
						}
						OrFilter orFilter = new OrFilter(new QueryFilter[]
						{
							new BitMaskFilter(MessageItemSchema.Flags, 1UL, false),
							new BitMaskFilter(PropertyTagPropertyDefinition.CreateCustom("ITEM_TMPFLAG", 278331395U), 1UL, true)
						});
						AndFilter andFilter = new AndFilter(new QueryFilter[]
						{
							new AndFilter(list.ToArray()),
							orFilter
						});
						QueryFilter searchQuery = new AndFilter(new QueryFilter[]
						{
							Utilities.GetObjectClassTypeFilter(false, true, new string[]
							{
								"IPM.Appointment",
								"IPM.Contact",
								"IPM.DistList",
								"IPM.Activity",
								"IPM.StickyNote",
								"IPM.Task"
							}),
							andFilter
						});
						outlookSearchFolder.ApplyContinuousSearch(new SearchFolderCriteria(searchQuery, new StoreId[]
						{
							mailboxSession.GetDefaultFolderId(DefaultFolderType.Root)
						})
						{
							DeepTraversal = true
						});
						outlookSearchFolder.Save();
						outlookSearchFolder.Load(array);
						try
						{
							outlookSearchFolder.MakeVisibleToOutlook();
						}
						catch (QuotaExceededException ex)
						{
							ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "RequestDispatcher.InitializeFavorites: Failed. Exception: {0}", ex.Message);
						}
					}
					navigationNodeCollection.AppendFolderToFavorites(outlookSearchFolder);
				}
				catch (ObjectExistedException ex2)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "RequestDispatcher.InitializeFavorites: same folder exists. Exception: {0}", ex2.Message);
				}
				finally
				{
					if (outlookSearchFolder != null)
					{
						outlookSearchFolder.Dispose();
					}
				}
			}
			using (Folder folder4 = Folder.Bind(mailboxSession, DefaultFolderType.SentItems, new PropertyDefinition[]
			{
				StoreObjectSchema.RecordKey
			}))
			{
				navigationNodeCollection.AppendFolderToFavorites(folder4);
			}
			navigationNodeCollection.Save(mailboxSession);
		}

		private static RequestDispatcher.DispatchStepResult HandleSecurityContextSidLimitException(OwaContext owaContext, OwaSecurityContextSidLimitException exception)
		{
			string name = exception.Name;
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorTooManySidsInContext, name, new object[]
			{
				owaContext.ProxyCasUri,
				owaContext.LocalHostName,
				name,
				SerializedClientSecurityContext.MaximumSidsPerContext
			});
			owaContext.ErrorString = string.Format(LocalizedStrings.GetNonEncoded(-1114069142), SerializedClientSecurityContext.MaximumSidsPerContext);
			return RequestDispatcher.DispatchStepResult.RewritePathToError;
		}

		internal static bool DoesSubPageSupportSingleDocument(string path)
		{
			if (!RequestDispatcher.IsUserControl(path))
			{
				return false;
			}
			string[] array = new string[]
			{
				"taskview.ascx",
				"contactview.ascx",
				"directoryview.ascx",
				"recoverdeleteditems.ascx",
				"readsharingmessage.ascx"
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Equals(Path.GetFileName(path), array[i], StringComparison.InvariantCultureIgnoreCase))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsUserControl(string path)
		{
			return string.Equals(Path.GetExtension(path), ".ascx", StringComparison.InvariantCultureIgnoreCase);
		}

		public const string LanguagePostLocaleIdParameter = "lcid";

		public const string LanguagePostTimeZoneKeyNameParameter = "tzid";

		public const string LanguagePostAccessibilityParameter = "opt";

		public const string LanguagePostDestinationParameter = "destination";

		public const string UpdatedUserSettingCookie = "UpdatedUserSettings";

		public const string UserCultureCookie = "mkt";

		public const string SubPageContainerPath = "forms/premium/SubPageContainer.aspx";

		public const string SubPageEventHandlerPath = "forms/premium/SubPageEventHandler.aspx";

		internal const string ApplicationElementString = "ae";

		internal const string StateString = "s";

		internal const string ActionString = "a";

		internal const string TypeString = "t";

		private static readonly PropertyDefinition[] ServerSitePropertyDefinition = new PropertyDefinition[]
		{
			ServerSchema.ServerSite
		};

		private enum DispatchStepResult
		{
			RedirectToUrl,
			RewritePath,
			RewritePathToError,
			WriteErrorToWebPart,
			EndResponse,
			Stop,
			Continue
		}
	}
}
