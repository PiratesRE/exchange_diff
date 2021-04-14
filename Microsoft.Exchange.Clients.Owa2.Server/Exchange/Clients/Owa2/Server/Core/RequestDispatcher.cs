using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Clients.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class RequestDispatcher
	{
		public static HostNameController HostNameController
		{
			get
			{
				if (RequestDispatcher.hostNameController == null)
				{
					lock (RequestDispatcher.syncRoot)
					{
						if (RequestDispatcher.hostNameController == null)
						{
							RequestDispatcher.hostNameController = new HostNameController(ConfigurationManager.AppSettings);
						}
					}
				}
				return RequestDispatcher.hostNameController;
			}
		}

		public static void DispatchRequest(RequestContext requestContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DispatchRequest] entry.");
			HttpContext httpContext = requestContext.HttpContext;
			UrlUtilities.RewriteFederatedDomainInURL(httpContext);
			DispatchStepResult arg = RequestDispatcher.InternalDispatchRequest(requestContext);
			ExTraceGlobals.CoreCallTracer.TraceDebug<DispatchStepResult>(0L, "[RequestDispatcher::DispatchRequest] Last dispatch step result={0}.", arg);
			switch (arg)
			{
			case DispatchStepResult.RedirectToUrl:
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "[RequestDispatcher::DispatchRequest] Redirecting response to '{0}'.", requestContext.DestinationUrl);
				httpContext.Response.Redirect(requestContext.DestinationUrl, false);
				if (requestContext.RequestType != OwaRequestType.Logoff || !RequestDispatcherUtilities.IsDownLevelClient(requestContext.HttpContext, false))
				{
					httpContext.ApplicationInstance.CompleteRequest();
				}
				return;
			case DispatchStepResult.RewritePath:
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "[RequestDispatcher::DispatchRequest] Rewriting path to '{0}'.", requestContext.DestinationUrl);
				if (string.IsNullOrEmpty(requestContext.DestinationUrlQueryString))
				{
					httpContext.RewritePath(requestContext.DestinationUrl);
					return;
				}
				httpContext.RewritePath(requestContext.DestinationUrl, null, requestContext.DestinationUrlQueryString);
				break;
			case DispatchStepResult.RewritePathToError:
			case DispatchStepResult.Continue:
				break;
			case DispatchStepResult.EndResponse:
				ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "[RequestDispatcher::DispatchRequest] Ending response with statusCode={0}.", (int)requestContext.HttpStatusCode);
				HttpUtilities.EndResponse(requestContext.HttpContext, requestContext.HttpStatusCode);
				return;
			case DispatchStepResult.EndResponseWithPrivateCaching:
				ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "[RequestDispatcher::DispatchRequest] Ending response with statusCode={0} and PRIVATE cacheability.", (int)requestContext.HttpStatusCode);
				HttpUtilities.EndResponse(requestContext.HttpContext, requestContext.HttpStatusCode, HttpCacheability.Private);
				return;
			case DispatchStepResult.Stop:
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DispatchRequest] Stopped the dispatching of this request.");
				return;
			default:
				return;
			}
		}

		private static DispatchStepResult InternalDispatchRequest(RequestContext requestContext)
		{
			HttpRequest request = requestContext.HttpContext.Request;
			requestContext.RequestType = RequestDispatcherUtilities.GetRequestType(request);
			ExTraceGlobals.CoreCallTracer.TraceDebug<OwaRequestType>(0L, "[RequestDispatcher::InternalDispatchRequest] Processing requestType={0}.", requestContext.RequestType);
			OwaRequestType requestType = requestContext.RequestType;
			if (requestType <= OwaRequestType.LanguagePage)
			{
				switch (requestType)
				{
				case OwaRequestType.Invalid:
					requestContext.HttpStatusCode = HttpStatusCode.BadRequest;
					return DispatchStepResult.EndResponse;
				case OwaRequestType.Authorize:
					break;
				case OwaRequestType.Logoff:
				case OwaRequestType.Aspx:
					goto IL_C2;
				case OwaRequestType.EsoRequest:
					return DispatchStepResult.Stop;
				default:
					switch (requestType)
					{
					case OwaRequestType.ProxyPing:
						RequestDispatcherUtilities.RespondProxyPing(requestContext);
						return DispatchStepResult.EndResponse;
					case OwaRequestType.LanguagePage:
						break;
					default:
						goto IL_C2;
					}
					break;
				}
				requestContext.HttpContext.Response.AppendHeader("X-OWA-Version", Globals.ApplicationVersion);
				return DispatchStepResult.Stop;
			}
			if (requestType == OwaRequestType.Resource)
			{
				return DispatchStepResult.Stop;
			}
			switch (requestType)
			{
			case OwaRequestType.WopiRequest:
				return DispatchStepResult.Stop;
			case OwaRequestType.RemoteNotificationRequest:
				return DispatchStepResult.Stop;
			case OwaRequestType.GroupSubscriptionRequest:
				return DispatchStepResult.Stop;
			}
			IL_C2:
			if (!requestContext.HttpContext.Request.IsAuthenticated)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::InternalDispatchRequest] Request not authenticated. returning.");
				requestContext.HttpStatusCode = HttpStatusCode.Unauthorized;
				return DispatchStepResult.EndResponse;
			}
			DispatchStepResult dispatchStepResult = RequestDispatcher.DispatchIfLogoffRequest(requestContext);
			if (dispatchStepResult != DispatchStepResult.Continue)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<DispatchStepResult>(0L, "[RequestDispatcher::InternalDispatchRequest] DispatchIfLogoffRequest returned {0}. returning.", dispatchStepResult);
				return dispatchStepResult;
			}
			dispatchStepResult = RequestDispatcher.DispatchIfLastPendingGet(requestContext);
			if (dispatchStepResult != DispatchStepResult.Continue)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<DispatchStepResult>(0L, "[RequestDispatcher::InternalDispatchRequest] DispatchIfLastPendingGet returned {0}. returning.", dispatchStepResult);
				return dispatchStepResult;
			}
			dispatchStepResult = RequestDispatcher.DispatchIfGetUserPhotoRequest(requestContext);
			if (dispatchStepResult != DispatchStepResult.Continue)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<DispatchStepResult>(0L, "[RequestDispatcher::InternalDispatchRequest] DispatchIfGetUserPhotoRequest returned {0}. returning.", dispatchStepResult);
				return dispatchStepResult;
			}
			requestContext.UserContext = UserContextManager.GetMailboxContext(requestContext.HttpContext, null, true);
			if (!requestContext.UserContext.ExchangePrincipal.MailboxInfo.Configuration.IsOwaEnabled && !OfflineClientRequestUtilities.IsRequestFromMOWAClient(requestContext.HttpContext.Request, requestContext.HttpContext.Request.UserAgent))
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::InternalDispatchRequest] OWA Disabled: redirecting to error page.");
				throw new AccountDisabledException(new LocalizedString(Strings.GetLocalizedString(531497785)));
			}
			if (RequestDispatcher.ShouldBlockConnection(requestContext.HttpContext, requestContext.UserContext.LogonIdentity))
			{
				ExTraceGlobals.CoreCallTracer.TraceWarning<string>(0L, "[RequestDispatcher::InternalOnPostAuthorizeRequest] blocked by Client Access Rules. Request URL={0}.", requestContext.HttpContext.Request.Url.OriginalString);
				if (requestContext.UserContext.LogonIdentity.UserOrganizationId != null && !OrganizationId.ForestWideOrgId.Equals(requestContext.UserContext.LogonIdentity.UserOrganizationId))
				{
					requestContext.DestinationUrl = OwaUrl.LogoffPageBlockedByClientAccessRules.GetExplicitUrl(requestContext.HttpContext.Request);
				}
				else
				{
					requestContext.DestinationUrl = OwaUrl.LogoffBlockedByClientAccessRules.GetExplicitUrl(requestContext.HttpContext.Request);
				}
				return DispatchStepResult.RedirectToUrl;
			}
			dispatchStepResult = RequestDispatcher.ValidateExplicitLogonPermissions(requestContext);
			if (dispatchStepResult != DispatchStepResult.Continue)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<DispatchStepResult>(0L, "[RequestDispatcher::InternalDispatchRequest] ValidateExplicitLogonPermissions returned {0}. returning.", dispatchStepResult);
				return dispatchStepResult;
			}
			dispatchStepResult = RequestDispatcher.DispatchIfLanguagePost(requestContext);
			if (dispatchStepResult != DispatchStepResult.Continue)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<DispatchStepResult>(0L, "[RequestDispatcher::InternalDispatchRequest] DispatchIfLanguagePost returned {0}. returning.", dispatchStepResult);
				return dispatchStepResult;
			}
			if (requestContext.UserContext is UserContext)
			{
				dispatchStepResult = RequestDispatcher.ValidateAndSetThreadCulture(requestContext);
				if (dispatchStepResult != DispatchStepResult.Continue)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<DispatchStepResult>(0L, "[RequestDispatcher::InternalDispatchRequest] ValidateAndSetThreadCulture returned {0}. returning.", dispatchStepResult);
					return dispatchStepResult;
				}
			}
			Uri originalRequestUrlFromContext = UrlUtilities.GetOriginalRequestUrlFromContext(requestContext.HttpContext);
			dispatchStepResult = RequestDispatcher.SendAppCacheRedirect(requestContext, originalRequestUrlFromContext);
			if (dispatchStepResult != DispatchStepResult.Continue)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<DispatchStepResult>(0L, "[RequestDispatcher::InternalDispatchRequest] SendAppCacheRedirect returned {0}. returning.", dispatchStepResult);
				return dispatchStepResult;
			}
			requestContext.HttpContext.Response.AppendHeader("X-OWA-Version", Globals.ApplicationVersion);
			requestContext.HttpContext.Response.AppendHeader("X-OWA-OWSVersion", ExchangeVersion.Latest.Version.ToString());
			requestContext.HttpContext.Response.AppendHeader("X-OWA-MinimumSupportedOWSVersion", ExchangeVersionType.V2_6.ToString());
			RequestDispatcher.SetTimeoutForRequest(requestContext.HttpContext, requestContext.RequestType);
			RequestDispatcher.CheckAndAddHostNameChangedCookie(requestContext, request);
			return RequestDispatcher.DoFinalDispatch(requestContext);
		}

		private static void CheckAndAddHostNameChangedCookie(RequestContext requestContext, HttpRequest request)
		{
			UserContext userContext = UserContextManager.GetMailboxContext(requestContext.HttpContext, null, true) as UserContext;
			bool flag = userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ServerSettings.OwaHostNameSwitch.Enabled;
			if (flag && requestContext.RequestType == OwaRequestType.Form15 && !RequestDispatcher.HostNameController.IsUserAgentExcludedFromHostNameSwitchFlight(request))
			{
				Uri requestUrlEvenIfProxied = request.GetRequestUrlEvenIfProxied();
				string text = null;
				if (RequestDispatcher.HostNameController.IsDeprecatedHostName(requestUrlEvenIfProxied.Host, out text))
				{
					RequestDispatcher.HostNameController.AddHostSwitchFlightEnabledCookie(requestContext.HttpContext.Response);
				}
			}
		}

		private static bool ShouldBlockConnection(HttpContext httpContext, OwaIdentity logonIdentity)
		{
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaServer.OwaClientAccessRulesEnabled.Enabled)
			{
				return false;
			}
			RequestDetailsLogger logger = OwaApplication.GetRequestDetailsLogger;
			Action<ClientAccessRulesEvaluationContext> blockLoggerDelegate = delegate(ClientAccessRulesEvaluationContext context)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(logger, ClientAccessRulesConstants.ClientAccessRuleName, context.CurrentRule.Name);
			};
			Action<double> latencyLoggerDelegate = delegate(double latency)
			{
				if (latency > 50.0)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(logger, ClientAccessRulesConstants.ClientAccessRulesLatency, latency);
				}
			};
			OWAMiniRecipient owaminiRecipient = logonIdentity.GetOWAMiniRecipient();
			string usernameFromIdInformation = ClientAccessRulesUtils.GetUsernameFromIdInformation(owaminiRecipient.WindowsLiveID, owaminiRecipient.MasterAccountSid, owaminiRecipient.Sid, owaminiRecipient.ObjectId);
			return ClientAccessRulesUtils.ShouldBlockConnection(logonIdentity.UserOrganizationId, usernameFromIdInformation, ClientAccessProtocol.OutlookWebApp, ClientAccessRulesUtils.GetRemoteEndPointFromContext(httpContext), httpContext.Request.IsAuthenticatedByAdfs() ? ClientAccessAuthenticationMethod.AdfsAuthentication : ClientAccessAuthenticationMethod.BasicAuthentication, owaminiRecipient, blockLoggerDelegate, latencyLoggerDelegate);
		}

		private static void SetTimeoutForRequest(HttpContext httpContext, OwaRequestType requestType)
		{
			if (requestType == OwaRequestType.ServiceRequest && (httpContext.Request.QueryString["action"] == "CreateAttachment" || httpContext.Request.QueryString["action"] == "CreateAttachmentFromLocalFile" || httpContext.Request.Path.EndsWith("CreateAttachmentFromForm")))
			{
				httpContext.Server.ScriptTimeout = RequestDispatcher.AttachmentTimeout;
				ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "[RequestDispatcher::SetTimeoutForRequest] Request timeout is going to be {0}.", httpContext.Server.ScriptTimeout);
			}
		}

		private static DispatchStepResult DispatchIfLanguagePost(RequestContext requestContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DispatchIfLanguagePost] entry.");
			Canary15Cookie.CanaryValidationResult canaryValidationResult;
			if (requestContext.RequestType == OwaRequestType.LanguagePost && HttpUtilities.IsPostRequest(requestContext.HttpContext.Request) && Canary15Cookie.ValidateCanaryInHeaders(requestContext.HttpContext, OwaRequestHandler.GetOriginalIdentitySid(requestContext.HttpContext), Canary15Profile.Owa, out canaryValidationResult))
			{
				OwaDiagnostics.TracePfd(25865, "The request is a post from the language selection page, processing this request...", new object[0]);
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DispatchIfLanguagePost] Dispatching.");
				HttpRequest request = requestContext.HttpContext.Request;
				CultureInfo culture;
				string timeZoneKeyName;
				bool isOptimized;
				string destination;
				RequestDispatcherUtilities.GetLanguagePostFormParameters(requestContext, request, out culture, out timeZoneKeyName, out isOptimized, out destination);
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DispatchIfLanguagePost] Dispatching language post request locally...");
				return RequestDispatcher.HandleLanguagePost(requestContext, culture, timeZoneKeyName, isOptimized, destination);
			}
			return DispatchStepResult.Continue;
		}

		private static DispatchStepResult HandleLanguagePost(RequestContext requestContext, CultureInfo culture, string timeZoneKeyName, bool isOptimized, string destination)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::HandleLanguagePost] entry.");
			if (requestContext.UserContext == null)
			{
				throw new OwaInvalidOperationException("UserContext should be created by the time language post is handled");
			}
			requestContext.LanguagePostUserCulture = culture;
			requestContext.HttpContext.Response.Cookies.Set(new HttpCookie("mkt", culture.Name));
			if (!string.IsNullOrWhiteSpace(destination) && (destination.StartsWith("/ecp/", StringComparison.OrdinalIgnoreCase) || destination.StartsWith("/owa/", StringComparison.OrdinalIgnoreCase)))
			{
				requestContext.DestinationUrl = destination;
			}
			else
			{
				requestContext.DestinationUrl = string.Empty;
			}
			Culture.InternalSetThreadPreferredCulture(culture);
			MailboxSession mailboxSession = null;
			try
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string, bool>(0L, "[RequestDispatcher::HandleLanguagePost] Attempting to save the timeZoneKeyName (tzid={0}) and isOptimized={1} in the mailbox.", timeZoneKeyName, isOptimized);
				OwaIdentity logonIdentity = requestContext.UserContext.LogonIdentity;
				mailboxSession = logonIdentity.CreateMailboxSession(requestContext.UserContext.ExchangePrincipal, culture);
				if (requestContext.UserContext.IsExplicitLogon && !mailboxSession.CanActAsOwner)
				{
					throw new OwaExplicitLogonException("User has no access rights to the mailbox", "ErrorExplicitLogonAccessDenied");
				}
				try
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "[RequestDispatcher::HandleLanguagePost] Trying to save the culture to the AD (lcid={0})", culture.LCID);
					PreferredCultures preferredCultures = new PreferredCultures(requestContext.UserContext.ExchangePrincipal.PreferredCultures);
					preferredCultures.AddSupportedCulture(culture, new Predicate<CultureInfo>(ClientCultures.IsSupportedCulture));
					Culture.SetPreferredCulture(requestContext.UserContext.ExchangePrincipal, preferredCultures, mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent));
					requestContext.UserContext.ExchangePrincipal = requestContext.UserContext.ExchangePrincipal.WithPreferredCultures(preferredCultures);
				}
				catch (Exception ex)
				{
					if (!(ex is ADOperationException) && !(ex is InvalidOperationException))
					{
						throw;
					}
					requestContext.FailedToSaveUserCulture = true;
					if (ExTraceGlobals.CoreCallTracer.IsTraceEnabled(TraceType.ErrorTrace))
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
						ExTraceGlobals.CoreCallTracer.TraceError(0L, stringBuilder.ToString());
					}
				}
				UserOptionsType userOptionsType = new UserOptionsType();
				bool flag = true;
				try
				{
					userOptionsType.Load(mailboxSession, false, false);
				}
				catch (QuotaExceededException ex2)
				{
					ExTraceGlobals.UserContextCallTracer.TraceDebug<string>(0L, "[RequestDispatcher::HandleLanguagePost] UserOptions.LoadAll failed. Exception: {0}.", ex2.Message);
					flag = false;
				}
				userOptionsType.TimeZone = timeZoneKeyName;
				userOptionsType.IsOptimizedForAccessibility = isOptimized;
				userOptionsType.UserOptionsMigrationState = UserOptionsMigrationState.WorkingHoursTimeZoneFixUp;
				if (flag)
				{
					UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.TimeZone);
					UserConfigurationPropertyDefinition propertyDefinition2 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.IsOptimizedForAccessibility);
					UserConfigurationPropertyDefinition propertyDefinition3 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.UserOptionsMigrationState);
					userOptionsType.Commit(mailboxSession, new UserConfigurationPropertyDefinition[]
					{
						propertyDefinition,
						propertyDefinition2,
						propertyDefinition3
					});
				}
				RequestDispatcher.InitializeFavorites(mailboxSession);
			}
			finally
			{
				if (mailboxSession != null)
				{
					UserContextUtilities.DisconnectStoreSession(mailboxSession);
					mailboxSession.Dispose();
					mailboxSession = null;
				}
			}
			return DispatchStepResult.Continue;
		}

		private static void InitializeFavorites(MailboxSession mailboxSession)
		{
			try
			{
				FavoriteFolderCollection favoritesCollection = FavoriteFolderCollection.GetFavoritesCollection(mailboxSession, FolderTreeDataSection.First);
				favoritesCollection.InitializeDefaultFavorites();
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.UserContextCallTracer.TraceError<string>(0L, "[RequestDispatcher::InitializeFavorites] Initializing favorites failed. Exception: {0}.", ex.Message);
			}
		}

		private static DispatchStepResult DispatchIfLastPendingGet(RequestContext requestContext)
		{
			if (requestContext.RequestType != OwaRequestType.Oeh)
			{
				return DispatchStepResult.Continue;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DispatchIfLastPendingGet] entry.");
			requestContext.UserContext = UserContextManager.GetUserContext(requestContext.HttpContext, false);
			if (OwaEventHandlerBase.ShouldIgnoreRequest(requestContext, requestContext.UserContext))
			{
				requestContext.HttpStatusCode = HttpStatusCode.BadRequest;
				return DispatchStepResult.EndResponse;
			}
			return DispatchStepResult.Continue;
		}

		private static string GetLogoffURL(RequestContext requestContext)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.OwaServer.OwaClientAccessRulesEnabled.Enabled && "5".Equals(requestContext.HttpContext.Request.Params["reason"]))
			{
				return OwaUrl.LogonFBAOWABlockedByClientAccessRules.GetExplicitUrl(requestContext.HttpContext.Request);
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Eac.EACClientAccessRulesEnabled.Enabled && "6".Equals(requestContext.HttpContext.Request.Params["reason"]))
			{
				Uri requestUrlEvenIfProxied = requestContext.HttpContext.Request.GetRequestUrlEvenIfProxied();
				string str = string.Format("{0}://{1}{2}/ecp/", requestUrlEvenIfProxied.Scheme, requestUrlEvenIfProxied.Host, requestUrlEvenIfProxied.IsDefaultPort ? string.Empty : string.Format(":{0}", requestUrlEvenIfProxied.Port));
				return string.Format(OwaUrl.LogonFBAEACBlockedByClientAccessRules.GetExplicitUrl(requestContext.HttpContext.Request), HttpUtility.UrlEncode(str));
			}
			if (LogOnSettings.IsLegacyLogOff)
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.RedirectToLogoffPage.Enabled)
				{
					return OwaUrl.LogoffAspxPage.GetExplicitUrl(requestContext.HttpContext.Request);
				}
				return OwaUrl.LogonFBA.GetExplicitUrl(requestContext.HttpContext.Request);
			}
			else
			{
				bool enabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
				if (enabled)
				{
					Uri requestUrlEvenIfProxied2 = requestContext.HttpContext.Request.GetRequestUrlEvenIfProxied();
					Uri uri = new Uri(requestUrlEvenIfProxied2, OwaUrl.LogoffAspxPage.GetExplicitUrl(requestContext.HttpContext.Request));
					string explicitUrl = OwaUrl.SignOutPage.GetExplicitUrl(requestContext.HttpContext.Request);
					Uri uri2 = new Uri(requestUrlEvenIfProxied2, explicitUrl);
					return uri.ToString() + "&ru=" + uri2.ToString();
				}
				return OwaUrl.SignOutPage.GetExplicitUrl(requestContext.HttpContext.Request);
			}
		}

		private static DispatchStepResult DispatchIfLogoffRequest(RequestContext requestContext)
		{
			if (requestContext.RequestType != OwaRequestType.Logoff)
			{
				return DispatchStepResult.Continue;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DispatchIfLogoffRequest] entry.");
			OwaDiagnostics.TracePfd(21769, "Dispatching logoff request.", new object[0]);
			requestContext.UserContext = UserContextManager.GetUserContext(requestContext.HttpContext, false);
			if (requestContext.UserContext != null)
			{
				RequestDispatcher.DoLogoffCleanup(requestContext);
			}
			if (RequestDispatcherUtilities.IsChangePasswordLogoff(requestContext.HttpContext.Request))
			{
				requestContext.DestinationUrl = OwaUrl.LogoffChangePasswordPage.GetExplicitUrl(requestContext.HttpContext.Request);
			}
			else
			{
				requestContext.DestinationUrl = RequestDispatcher.GetLogoffURL(requestContext);
			}
			return DispatchStepResult.RedirectToUrl;
		}

		private static DispatchStepResult DispatchIfGetUserPhotoRequest(RequestContext context)
		{
			return new GetUserPhotoRequestDispatcher(ExTraceGlobals.CoreCallTracer).Dispatch(context);
		}

		private static void DoLogoffCleanup(RequestContext requestContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DoLogoff] entry.");
			HttpContext httpContext = requestContext.HttpContext;
			UserContext userContext = requestContext.UserContext as UserContext;
			UserContextKey key = userContext.Key;
			try
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<UserContext>(0L, "[RequestDispatcher::DoLogoffCleanup] Found user context in the cache, User context instance={0}.", userContext);
				userContext.State = UserContextState.MarkedForLogoff;
				userContext.LogBreadcrumb("MarkedForLogoff");
				userContext.DoLogoffCleanup();
				userContext.LogBreadcrumb("DoLogoffCleanup completed");
			}
			finally
			{
				UserContextCookie userContextCookie = UserContextCookie.GetUserContextCookie(requestContext.HttpContext);
				if (userContextCookie != null)
				{
					HttpUtilities.DeleteCookie(httpContext.Response, userContextCookie.CookieName);
				}
				HttpUtilities.DeleteCookie(httpContext.Response, Canary15Profile.Owa.Name);
				if (key.UserContextId == null)
				{
					string message = "User context id couldn't be retrieved. Logoff can't be performed";
					ExTraceGlobals.UserContextTracer.TraceDebug(0L, message);
					userContext.LogBreadcrumb(message);
					userContext.Dispose();
					userContext = null;
				}
				else
				{
					userContext.LogBreadcrumb("Logoff invoking Cache.Remove");
					object obj = HttpRuntime.Cache.Remove(key.ToString());
					userContext.LogBreadcrumb("Logoff invoked Cache.Remove");
					string message2 = (obj == null) ? "The userContext was already deleted from the cache" : "Context successfully deleted from the cache";
					ExTraceGlobals.UserContextTracer.TraceDebug(0L, message2);
					userContext.LogBreadcrumb(message2);
				}
			}
		}

		private static DispatchStepResult DoFinalDispatch(RequestContext requestContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DoFinalDispatch] entry.");
			if (requestContext.RequestType == OwaRequestType.ProxyLogon)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "[RequestDispatcher::DoFinalDispatch] Proxy logon request. Ending with status code {0}.", 241);
				requestContext.HttpStatusCode = (HttpStatusCode)241;
				return DispatchStepResult.EndResponse;
			}
			if (requestContext.RequestType == OwaRequestType.LanguagePost)
			{
				if (requestContext.FailedToSaveUserCulture)
				{
					requestContext.DestinationUrl = OwaUrl.InfoFailedToSaveCulture.GetExplicitUrl(requestContext.HttpContext.Request);
				}
				else if (string.IsNullOrEmpty(requestContext.DestinationUrl))
				{
					requestContext.DestinationUrl = OwaUrl.ApplicationRoot.GetExplicitUrl(requestContext.HttpContext.Request);
				}
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "[RequestDispatcher::DoFinalDispatch] language post request. Redirecting to '{0}' url.", requestContext.DestinationUrl ?? "<NULL>");
				return DispatchStepResult.RedirectToUrl;
			}
			if (requestContext.RequestType == OwaRequestType.KeepAlive)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DoFinalDispatch] processing keep alive request.");
				requestContext.HttpStatusCode = HttpStatusCode.OK;
				return DispatchStepResult.EndResponse;
			}
			if (requestContext.RequestType == OwaRequestType.Form15 && !RequestDispatcherUtilities.IsDownLevelClient(requestContext.HttpContext, false))
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "[RequestDispatcher::DoFinalDispatch] rewriting path to '{0}'.", OwaUrl.Default15Page.ImplicitUrl);
				requestContext.DestinationUrl = OwaUrl.Default15Page.ImplicitUrl;
				return DispatchStepResult.RewritePath;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::DoFinalDispatch] continue to the next handler.");
			return DispatchStepResult.Continue;
		}

		private static DispatchStepResult ValidateAndSetThreadCulture(RequestContext requestContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::ValidateAndSetThreadCulture] entry.");
			CultureInfo cultureInfo = null;
			EcpUserSettings ecpUserSettings = (EcpUserSettings)0U;
			UserContext userContext = requestContext.UserContext as UserContext;
			if (requestContext.UserContext != null && userContext == null)
			{
				throw new OwaInvalidOperationException(string.Format("Context was expected to be of type UserContext, but it is of type {0}", requestContext.UserContext.GetType()));
			}
			if (RequestDispatcherUtilities.TryReadUpdatedUserSettingsCookie(requestContext, out ecpUserSettings))
			{
				cultureInfo = RequestDispatcherUtilities.GetUserCultureFromEcpCookie(requestContext, ecpUserSettings);
				if (userContext != null)
				{
					userContext.RefreshUserSettings(cultureInfo, ecpUserSettings);
				}
				RequestDispatcherUtilities.DeleteUserSettingsCookie(requestContext);
			}
			if (cultureInfo == null)
			{
				bool flag = requestContext.RequestType == OwaRequestType.SuiteServiceProxyPage;
				if (requestContext.LanguagePostUserCulture != null)
				{
					cultureInfo = requestContext.LanguagePostUserCulture;
				}
				else if (userContext != null && !flag)
				{
					cultureInfo = Culture.GetPreferredCultureInfo(userContext.ExchangePrincipal);
					if (cultureInfo == null && userContext.IsExplicitLogon)
					{
						cultureInfo = ClientCultures.GetPreferredCultureInfo(userContext.LogonIdentity.GetOWAMiniRecipient().Languages);
						userContext.UserCulture = cultureInfo;
					}
					if (cultureInfo == null)
					{
						if (string.IsNullOrEmpty(requestContext.DestinationUrlQueryString))
						{
							requestContext.DestinationUrlQueryString = requestContext.HttpContext.Request.QueryString.ToString();
						}
						bool flag2 = string.IsNullOrEmpty(requestContext.HttpContext.Request.Headers["X-SuiteServiceProxyOrigin"]);
						CultureInfo culture;
						string timeZoneKeyName;
						if (OfflineClientRequestUtilities.IsRequestFromMOWAClient(requestContext.HttpContext.Request, requestContext.HttpContext.Request.UserAgent) && RequestDispatcherUtilities.TryReadMowaGlobalizationSettings(requestContext, out culture, out timeZoneKeyName))
						{
							return RequestDispatcher.HandleLanguagePost(requestContext, culture, timeZoneKeyName, false, requestContext.DestinationUrl);
						}
						if (flag2)
						{
							requestContext.DestinationUrl = RequestDispatcherUtilities.GetDestinationForRedirectToLanguagePage(requestContext);
							requestContext.HttpContext.Response.AppendHeader("X-OWA-Version", Globals.ApplicationVersion);
							requestContext.HttpContext.Response.AppendToLog("&redir=lang");
							return DispatchStepResult.RedirectToUrl;
						}
						requestContext.HttpContext.Response.Headers["X-OWA-Error"] = "OwaInvalidUserLanguageException";
						requestContext.HttpStatusCode = HttpStatusCode.NoContent;
						return DispatchStepResult.EndResponse;
					}
					else if (userContext.IsUserCultureExplicitlySet)
					{
						cultureInfo = userContext.UserCulture;
					}
				}
			}
			if (cultureInfo != null)
			{
				Culture.InternalSetThreadPreferredCulture(cultureInfo);
			}
			else
			{
				Culture.InternalSetThreadPreferredCulture();
			}
			return DispatchStepResult.Continue;
		}

		private static DispatchStepResult ValidateExplicitLogonPermissions(RequestContext requestContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::ValidateExplicitLogonPermissions] entry.");
			requestContext.UserContext.ValidateLogonPermissionIfNecessary();
			return DispatchStepResult.Continue;
		}

		private static DispatchStepResult SendAppCacheRedirect(RequestContext requestContext, Uri originalUri)
		{
			HttpContext httpContext = requestContext.HttpContext;
			HttpApplication applicationInstance = httpContext.ApplicationInstance;
			string explicitLogonUser = UserContextUtilities.GetExplicitLogonUser(httpContext);
			if (string.IsNullOrEmpty(explicitLogonUser))
			{
				RequestDispatcher.BindDefaultForceAppcacheCookieToSession(applicationInstance, httpContext);
				if (RequestDispatcher.RedirectBecauseClientForgotTrailingSlashOnRoot(applicationInstance, httpContext, originalUri))
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::SendAppCacheRedirect] OwaModule redirected because client forgot the trailing slash on the root and that's in the appcache.");
					return DispatchStepResult.Stop;
				}
				if (RequestDispatcher.RedirectBecauseClientRequestedOwaRootSlashRealm(applicationInstance, httpContext, originalUri))
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::SendAppCacheRedirect] OwaModule redirected because the client requested /realm and that's in the appcache.");
					return DispatchStepResult.Stop;
				}
				if (RequestDispatcher.RedirectBecauseClientRequestedWebsiteRoot(applicationInstance, httpContext, originalUri))
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::SendAppCacheRedirect] OwaModule redirected because the client requested the website root and that's in the appcache.");
					return DispatchStepResult.Stop;
				}
				if (RequestDispatcher.RedirectBecauseIE10RequiresReloadFromAppcache(applicationInstance, httpContext, originalUri))
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "[RequestDispatcher::SendAppCacheRedirect] OwaModule told IE10 to reload so it can use the appcache.");
					return DispatchStepResult.Stop;
				}
			}
			return DispatchStepResult.Continue;
		}

		private static bool RedirectBecauseClientForgotTrailingSlashOnRoot(HttpApplication httpApplication, HttpContext httpContext, Uri originalUri)
		{
			bool result = false;
			HttpRequest request = httpContext.Request;
			if (request.HttpMethod == "GET")
			{
				string absolutePath = originalUri.AbsolutePath;
				string rootWithSlash = RequestDispatcher.GetRootWithSlash();
				string value = rootWithSlash.TrimEnd(new char[]
				{
					'/'
				});
				if (absolutePath.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					if (string.IsNullOrEmpty(originalUri.Query) && OfflineClientRequestUtilities.IsRequestForAppCachedVersion(httpContext))
					{
						RequestDispatcher.SendJavascriptRedirectTo(httpApplication, httpContext.Response, rootWithSlash);
						result = true;
					}
					else
					{
						RequestDispatcher.Send301RedirectTo(httpContext.Response, rootWithSlash + originalUri.Query);
						result = true;
					}
				}
			}
			return result;
		}

		private static bool RedirectBecauseClientRequestedOwaRootSlashRealm(HttpApplication httpApplication, HttpContext httpContext, Uri originalUri)
		{
			bool result = false;
			HttpRequest request = httpContext.Request;
			if (request.HttpMethod == "GET")
			{
				string rootWithSlash = RequestDispatcher.GetRootWithSlash();
				string absolutePath = originalUri.AbsolutePath;
				string text = httpContext.Request.QueryString["realm"] ?? string.Empty;
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = Uri.EscapeDataString(text.ToLowerInvariant());
					if (absolutePath.Equals(rootWithSlash + text2, StringComparison.OrdinalIgnoreCase) || absolutePath.Equals(rootWithSlash + text2 + "/", StringComparison.OrdinalIgnoreCase))
					{
						string str = string.IsNullOrEmpty(originalUri.Query) ? "?" : (originalUri.Query + "&");
						string str2 = string.Format("{0}={1}", "realm", text2);
						RequestDispatcher.SendJavascriptRedirectTo(httpApplication, httpContext.Response, rootWithSlash + str + str2);
						result = true;
					}
				}
			}
			return result;
		}

		private static bool RedirectBecauseClientRequestedWebsiteRoot(HttpApplication httpApplication, HttpContext httpContext, Uri originalUri)
		{
			bool result = false;
			HttpRequest request = httpContext.Request;
			if (request.HttpMethod == "GET")
			{
				string absolutePath = originalUri.AbsolutePath;
				string rootWithSlash = RequestDispatcher.GetRootWithSlash();
				if (string.IsNullOrEmpty(absolutePath) || absolutePath.Equals("/", StringComparison.OrdinalIgnoreCase))
				{
					RequestDispatcher.SendJavascriptRedirectTo(httpApplication, httpContext.Response, rootWithSlash + originalUri.Query);
					result = true;
				}
			}
			return result;
		}

		private static bool RedirectBecauseIE10RequiresReloadFromAppcache(HttpApplication httpApplication, HttpContext httpContext, Uri originalUri)
		{
			bool result = false;
			HttpRequest request = httpContext.Request;
			if (request.HttpMethod == "GET")
			{
				string absolutePath = originalUri.AbsolutePath;
				string rootWithSlash = RequestDispatcher.GetRootWithSlash();
				if (absolutePath.Equals(rootWithSlash, StringComparison.OrdinalIgnoreCase) && RequestDispatcher.IsBrowserIE10(httpContext) && !OfflineClientRequestUtilities.IsRequestForAppCachedVersion(httpContext) && !DefaultPageBase.IsPalEnabled(httpContext))
				{
					result = RequestDispatcher.ForceIE10ReloadToUseAppcacheIfNotAlreadyDone(httpApplication, httpContext);
				}
			}
			return result;
		}

		private static bool ForceIE10ReloadToUseAppcacheIfNotAlreadyDone(HttpApplication httpApplication, HttpContext httpContext)
		{
			bool result = false;
			string text;
			string text2;
			RequestDispatcher.GetIE10ForceAppcacheCookies(httpContext, out text, out text2);
			if (text != "1" && (string.IsNullOrEmpty(text) || text2 != text))
			{
				string cookieValue = string.IsNullOrEmpty(text2) ? "1" : text2;
				RequestDispatcher.AddAlreadyForcedCookie(httpContext, cookieValue);
				RequestDispatcher.SendJavascriptReload(httpApplication, httpContext);
				result = true;
			}
			return result;
		}

		private static void BindDefaultForceAppcacheCookieToSession(HttpApplication httpApplication, HttpContext httpContext)
		{
			string a;
			string text;
			RequestDispatcher.GetIE10ForceAppcacheCookies(httpContext, out a, out text);
			if (a == "1" && !string.IsNullOrEmpty(text))
			{
				RequestDispatcher.AddAlreadyForcedCookie(httpContext, text);
			}
		}

		private static void GetIE10ForceAppcacheCookies(HttpContext httpContext, out string forced, out string canary)
		{
			forced = ((httpContext.Request.Cookies["IE10AlreadyForcedAppCache"] == null) ? string.Empty : httpContext.Request.Cookies["IE10AlreadyForcedAppCache"].Value);
			canary = ((httpContext.Request.Cookies["X-OWA-CANARY"] == null) ? string.Empty : httpContext.Request.Cookies["X-OWA-CANARY"].Value);
		}

		private static void AddAlreadyForcedCookie(HttpContext httpContext, string cookieValue)
		{
			HttpCookie httpCookie = new HttpCookie("IE10AlreadyForcedAppCache", cookieValue);
			httpCookie.Secure = true;
			httpCookie.HttpOnly = true;
			httpContext.Response.Cookies.Add(httpCookie);
		}

		private static void SendJavascriptRedirectTo(HttpApplication httpApplication, HttpResponse response, string redirectUri)
		{
			response.StatusCode = 200;
			response.ContentType = "text/html";
			redirectUri = Uri.EscapeUriString(Uri.UnescapeDataString(redirectUri));
			response.Write(string.Format("<!DOCTYPE html><html><head><script type=\"text/javascript\">window.location.replace(\"{0}\" + window.location.hash);</script></head><body></body></html>", redirectUri));
			httpApplication.CompleteRequest();
		}

		private static void SendJavascriptReload(HttpApplication httpApplication, HttpContext httpContext)
		{
			httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			httpContext.Response.Cache.SetNoStore();
			httpContext.Response.StatusCode = 200;
			httpContext.Response.ContentType = "text/html";
			httpContext.Response.Write("<!DOCTYPE html><html><head><script type='text/javascript'>window.location.reload(false);</script></head><body></body></html>");
			httpApplication.CompleteRequest();
		}

		private static void Send301RedirectTo(HttpResponse response, string redirectUri)
		{
			bool endResponse = true;
			response.RedirectPermanent(redirectUri, endResponse);
		}

		private static string GetRootWithSlash()
		{
			string text = HttpRuntime.AppDomainAppVirtualPath;
			if (Globals.IsPreCheckinApp)
			{
				text = "/owa/";
			}
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			return text;
		}

		private static bool IsBrowserIE10(HttpContext httpContext)
		{
			return httpContext.Request.Browser.Browser == "IE" && (httpContext.Request.Browser.MajorVersion == 10 || (httpContext.Request.UserAgent != null && httpContext.Request.UserAgent.IndexOf("Trident/6.0", StringComparison.OrdinalIgnoreCase) != -1));
		}

		public const string IE10AlreadyForcedAppCacheCookieName = "IE10AlreadyForcedAppCache";

		public const string IE10AlreadyForcedAppCacheCookieDefault = "1";

		private const string ClientSideRedirectBody = "<!DOCTYPE html><html><head><script type=\"text/javascript\">window.location.replace(\"{0}\" + window.location.hash);</script></head><body></body></html>";

		private const string OwaInvalidUserLanguageExceptionName = "OwaInvalidUserLanguageException";

		private const string ClientSideReloadBody = "<!DOCTYPE html><html><head><script type='text/javascript'>window.location.reload(false);</script></head><body></body></html>";

		private const string ECPRootUrlFormat = "{0}://{1}{2}/ecp/";

		private static readonly int AttachmentTimeout = 600;

		private static HostNameController hostNameController;

		private static readonly object syncRoot = new object();
	}
}
