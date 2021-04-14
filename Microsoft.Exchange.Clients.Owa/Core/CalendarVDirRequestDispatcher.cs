using System;
using System.Globalization;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class CalendarVDirRequestDispatcher
	{
		public static void DispatchRequest(OwaContext owaContext)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "CalendarVDirRequestDispatcher.DispatchRequest");
			bool flag = CalendarVDirRequestDispatcher.ValidateAndRemapTheRequest(owaContext.HttpContext);
			if (flag)
			{
				Utilities.EndResponse(owaContext.HttpContext, HttpStatusCode.BadRequest);
				return;
			}
			flag = CalendarVDirRequestDispatcher.CheckRequestType(owaContext);
			if (flag)
			{
				return;
			}
			flag = CalendarVDirRequestDispatcher.BindExchangePrincipal(owaContext);
			if (flag)
			{
				return;
			}
			owaContext.RequestExecution = RequestExecution.Local;
			owaContext.AnonymousSessionContext = new AnonymousSessionContext(owaContext);
			if (owaContext.RequestExecution == RequestExecution.Local)
			{
				CalendarVDirRequestDispatcher.SetThreadCulture(owaContext);
				CalendarVDirRequestDispatcher.LookupExperience(owaContext);
				if (owaContext.RequestType == OwaRequestType.ServiceRequest)
				{
					return;
				}
				CalendarVDirRequestDispatcher.MapPublishedCalendarViewUrl(owaContext);
			}
		}

		private static void MapPublishedCalendarViewUrl(OwaContext owaContext)
		{
			if (owaContext.RequestType == OwaRequestType.PublishedCalendarView)
			{
				owaContext.HttpContext.RewritePath("anonymouscalendar.aspx");
			}
		}

		private static bool CheckRequestType(OwaContext owaContext)
		{
			bool result = false;
			owaContext.RequestType = Utilities.GetRequestType(owaContext.HttpContext.Request);
			if (owaContext.RequestType == OwaRequestType.Resource)
			{
				return true;
			}
			if (owaContext.RequestType != OwaRequestType.PublishedCalendarView && owaContext.RequestType != OwaRequestType.ProxyPing && owaContext.RequestType != OwaRequestType.ServiceRequest && owaContext.RequestType != OwaRequestType.ICalHttpHandler)
			{
				Utilities.EndResponse(owaContext.HttpContext, HttpStatusCode.BadRequest);
				result = true;
			}
			if (owaContext.RequestType == OwaRequestType.ProxyPing)
			{
				RequestDispatcherUtilities.RespondProxyPing(owaContext);
				Utilities.EndResponse(owaContext.HttpContext, owaContext.HttpStatusCode);
			}
			return result;
		}

		private static bool ValidateAndRemapTheRequest(HttpContext httpContext)
		{
			Uri url = httpContext.Request.Url;
			if (Utilities.IsOwaUrl(url, OwaUrl.ProxyPing, true))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<Uri>(0L, "RemapPublishedRequest: Ignore the ProxyPing - {0}", url);
				return false;
			}
			bool flag = url.PathAndQuery.Contains("/S-1-8-");
			int num = flag ? 7 : 6;
			if (url.Segments.Length < num)
			{
				ExTraceGlobals.CoreTracer.TraceError<Uri>(0L, "RemapPublishedRequest: Invalid Segments.Length - {0}", url);
				return true;
			}
			int num2 = url.Segments[0].Length + url.Segments[1].Length + url.Segments[2].Length;
			int num3 = num2;
			for (int i = 3; i < num - 1; i++)
			{
				num3 += url.Segments[i].Length;
			}
			string text = url.PathAndQuery.Remove(num2, num3 - num2);
			string a = url.Segments[url.Segments.Length - 1];
			string url2;
			if (string.Equals(a, "calendar.html", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "reachcalendar.html", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "calendar.ics", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "reachcalendar.ics", StringComparison.OrdinalIgnoreCase))
			{
				url2 = url.AbsoluteUri;
			}
			else
			{
				UriBuilder uriBuilder = new UriBuilder(url);
				if (flag)
				{
					uriBuilder.Path = string.Concat(new string[]
					{
						url.Segments[0],
						url.Segments[1],
						url.Segments[2],
						url.Segments[3],
						url.Segments[4],
						url.Segments[5],
						SharingDataType.ReachCalendar.PublishResourceName,
						".html"
					});
				}
				else
				{
					uriBuilder.Path = string.Concat(new string[]
					{
						url.Segments[0],
						url.Segments[1],
						url.Segments[2],
						url.Segments[3],
						url.Segments[4],
						SharingDataType.Calendar.PublishResourceName,
						".html"
					});
				}
				url2 = uriBuilder.Uri.ToString();
			}
			if (!CalendarVDirRequestDispatcher.ValidateAndCapturePublishingUrl(httpContext, url2))
			{
				return true;
			}
			ExTraceGlobals.CoreTracer.TraceDebug<Uri, string>(0L, "RemapPublishedRequest: Rewrite {0} to {1}", url, text);
			httpContext.RewritePath(text);
			return false;
		}

		private static bool ValidateAndCapturePublishingUrl(HttpContext context, string url)
		{
			bool result = false;
			try
			{
				context.Items["AnonymousUserContextPublishedUrl"] = PublishingUrl.Create(url);
				result = true;
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "CalendarVDirRequestDispatcher.CreateAndCapturePublishingUrl: Invalid PublishingUrl - {0}", url);
			}
			return result;
		}

		private static bool BindExchangePrincipal(OwaContext owaContext)
		{
			bool result = false;
			PublishingUrl publishingUrl = (PublishingUrl)owaContext.HttpContext.Items["AnonymousUserContextPublishedUrl"];
			try
			{
				using (PublishedCalendar publishedCalendar = (PublishedCalendar)PublishedFolder.Create(publishingUrl))
				{
					owaContext.ExchangePrincipal = publishedCalendar.CreateExchangePrincipal();
					HttpContext.Current.Items["AnonymousUserContextExchangePrincipalKey"] = owaContext.ExchangePrincipal;
					HttpContext.Current.Items["AnonymousUserContextTimeZoneKey"] = publishedCalendar.TimeZone;
					HttpContext.Current.Items["AnonymousUserContextSharingDetailsKey"] = publishedCalendar.DetailLevel;
					HttpContext.Current.Items["AnonymousUserContextPublishedCalendarNameKey"] = publishedCalendar.DisplayName;
					HttpContext.Current.Items["AnonymousUserContextPublishedCalendarIdKey"] = publishedCalendar.FolderId;
				}
			}
			catch (WrongServerException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<WrongServerException>(0L, "CalendarVDirRequestDispatcher.BindExchangePrincipal: Returns 500 due to wrong BE server. Exception = {0}.", ex);
				owaContext.HttpContext.Response.AppendHeader("X-BEServerException", CalendarVDirRequestDispatcher.BEServerExceptionHeaderValue);
				string value = ex.RightServerToString();
				if (!string.IsNullOrEmpty(value))
				{
					owaContext.HttpContext.Response.AppendHeader(WellKnownHeader.XDBMountedOnServer, value);
				}
				owaContext.HttpContext.Response.TrySkipIisCustomErrors = true;
				Utilities.EndResponse(owaContext.HttpContext, HttpStatusCode.InternalServerError);
				result = true;
			}
			catch (PublishedFolderAccessDeniedException arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<PublishedFolderAccessDeniedException>(0L, "CalendarVDirRequestDispatcher.BindExchangePrincipal: Returns 404 due to access is denied. Exception = {0}.", arg);
				Utilities.EndResponse(owaContext.HttpContext, HttpStatusCode.NotFound);
				result = true;
			}
			catch (FolderNotPublishedException arg2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<FolderNotPublishedException>(0L, "CalendarVDirRequestDispatcher.BindExchangePrincipal: Returns 404 due to Folder not published. Exception = {0}.", arg2);
				Utilities.EndResponse(owaContext.HttpContext, HttpStatusCode.NotFound);
				result = true;
			}
			catch (OverBudgetException ex2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<OverBudgetException>(0L, "CalendarVDirRequestDispatcher.BindExchangePrincipal: User is throttled. Exception = {0}.", ex2);
				Utilities.HandleException(owaContext, ex2);
				result = true;
			}
			return result;
		}

		private static void SetThreadCulture(OwaContext owaContext)
		{
			CultureInfo cultureInfo = Culture.GetBrowserDefaultCulture(owaContext);
			if (cultureInfo == null)
			{
				cultureInfo = Globals.ServerCulture;
			}
			owaContext.Culture = cultureInfo;
			Culture.SetThreadCulture(owaContext);
		}

		private static void LookupExperience(OwaContext owaContext)
		{
			BrowserType browserType;
			UserAgentParser.UserAgentVersion userAgentVersion;
			Experience[] array;
			RequestDispatcherUtilities.LookupExperiencesForRequest(owaContext, false, true, out browserType, out userAgentVersion, out array);
			if (array == null || array.Length == 0)
			{
				throw new OwaClientNotSupportedException("FormsRegistryManager.LookupExperiences couldn't find any experience for this client.");
			}
			owaContext.AnonymousSessionContext.Experiences = array;
		}

		private const string ExternalUserSidPrefix = "/S-1-8-";

		internal const string BEServerExceptionHeaderName = "X-BEServerException";

		internal static readonly string BEServerExceptionHeaderValue = typeof(IllegalCrossServerConnectionException).ToString();
	}
}
