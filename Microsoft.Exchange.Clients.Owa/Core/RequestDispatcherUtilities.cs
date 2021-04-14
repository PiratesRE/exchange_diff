using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class RequestDispatcherUtilities
	{
		internal static ApplicationElement GetApplicationElement(HttpRequest httpRequest)
		{
			ApplicationElement result = ApplicationElement.StartPage;
			string queryStringParameter = Utilities.GetQueryStringParameter(httpRequest, "ae", false);
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				object obj = FormsRegistry.ApplicationElementParser.Parse(httpRequest.QueryString["ae"]);
				if (obj == null)
				{
					throw new OwaInvalidRequestException("Invalid application element");
				}
				result = (ApplicationElement)obj;
			}
			return result;
		}

		internal static FormValue DoFormsRegistryLookup(ISessionContext sessionContext, ApplicationElement applicationElement, string type, string action, string state)
		{
			return RequestDispatcherUtilities.DoFormsRegistryLookup(sessionContext.Experiences, sessionContext.SegmentationFlags, applicationElement, type, action, state);
		}

		internal static FormValue DoFormsRegistryLookup(Experience[] experiences, ulong segmentationFlags, ApplicationElement applicationElement, string type, string action, string state)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "RequestDispatcher.DoFormsRegistryLookup");
			if (type == null)
			{
				type = string.Empty;
			}
			if (state == null)
			{
				state = string.Empty;
			}
			if (action == null)
			{
				action = string.Empty;
			}
			FormKey formKey = new FormKey(applicationElement, type, action, state);
			FormValue formValue = FormsRegistryManager.LookupForm(formKey, experiences);
			if (formValue == null)
			{
				return null;
			}
			if ((formValue.SegmentationFlags & segmentationFlags) != formValue.SegmentationFlags)
			{
				formKey.Action = action;
				formKey.State = state;
				formKey.Class = "Disabled";
				formValue = FormsRegistryManager.LookupForm(formKey, experiences);
			}
			if (formValue == null)
			{
				return null;
			}
			return formValue;
		}

		internal static void LookupExperiencesForRequest(OwaContext owaContext, bool isOptimizedForAccessibility, bool isRichClientFeatureEnabled, out BrowserType browserType, out UserAgentParser.UserAgentVersion browserVersion, out Experience[] experiences)
		{
			string application = string.Empty;
			string empty = string.Empty;
			browserType = BrowserType.Other;
			browserVersion = default(UserAgentParser.UserAgentVersion);
			if (isOptimizedForAccessibility || RequestDispatcherUtilities.ShouldDoBasicRegistryLookup(owaContext))
			{
				UserAgentParser.Parse(string.Empty, out application, out browserVersion, out empty);
			}
			else
			{
				UserAgentParser.Parse(owaContext.HttpContext.Request.UserAgent, out application, out browserVersion, out empty);
			}
			browserType = Utilities.GetBrowserType(owaContext.HttpContext.Request.UserAgent);
			if (browserType == BrowserType.Other)
			{
				application = "Safari";
				browserVersion = new UserAgentParser.UserAgentVersion(3, 0, 0);
				empty = string.Empty;
				browserType = BrowserType.Safari;
			}
			experiences = FormsRegistryManager.LookupExperiences(application, browserVersion, empty, ClientControl.None, isRichClientFeatureEnabled);
		}

		internal static bool ShouldDoBasicRegistryLookup(OwaContext owaContext)
		{
			return (RequestDispatcherUtilities.IsDownLevelClient(owaContext.HttpContext, false) && owaContext.RequestType != OwaRequestType.Attachment && owaContext.RequestType != OwaRequestType.WebReadyRequest && owaContext.RequestType != OwaRequestType.Aspx) || owaContext.RequestType == OwaRequestType.WebPart;
		}

		internal static void SetProxyRequestUrl(OwaContext owaContext)
		{
			UriBuilder uriBuilder = new UriBuilder(owaContext.HttpContext.Request.Url);
			Uri uri = uriBuilder.Uri;
			uriBuilder = new UriBuilder(owaContext.SecondCasUri.Uri);
			uriBuilder.Path = uri.AbsolutePath;
			string text = uri.Query;
			if (text.StartsWith("?", StringComparison.Ordinal))
			{
				text = text.Substring(1, text.Length - 1);
			}
			uriBuilder.Query = text;
			Uri uri2 = uriBuilder.Uri;
			OwaDiagnostics.TracePfd(23177, "The request will be proxied to \"{0}\"", new object[]
			{
				uri2
			});
			owaContext.SetInternalHandlerParameter("pru", uri2);
		}

		internal static void RespondProxyPing(OwaContext owaContext)
		{
			owaContext.HttpContext.Response.AppendHeader("X-OWA-Version", Globals.ApplicationVersion);
			owaContext.HttpStatusCode = (HttpStatusCode)242;
		}

		internal const string DisabledItemType = "Disabled";
	}
}
