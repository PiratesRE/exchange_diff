using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Security
{
	internal static class LogonLatencyLogger
	{
		static LogonLatencyLogger()
		{
			try
			{
				bool.TryParse(ConfigurationManager.AppSettings["LogLiveLogonLatency"], out LogonLatencyLogger.loggingEnabled);
				if (LogonLatencyLogger.loggingEnabled)
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Logon latency key was set to true in web.config, logging will take place");
				}
				else
				{
					ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceDebug(0L, "Logon latency key was set to false or not found in web.config, logging will not take place");
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "Unexpected exception in static constructor of LogonLatencyLogger. Exception message: {0}. Stack trace: {1}", ex.Message, ex.StackTrace);
			}
		}

		internal static void AddLatencyHeader(HttpContext httpContext, string header, long latency)
		{
			if (httpContext == null || httpContext.Response == null || !UserAgentUtilities.IsMonitoringRequest(httpContext.Request.UserAgent))
			{
				return;
			}
			try
			{
				httpContext.Response.AppendHeader(header, latency.ToString());
			}
			catch (HttpException arg)
			{
				ExTraceGlobals.PerformanceTracer.TraceDebug<HttpException>(0L, "Exception happened while trying to append latency headers. Exception will be ignored: {0}", arg);
			}
		}

		internal static void CreateCookie(HttpContext httpContext, string logonStep)
		{
			try
			{
				if (LogonLatencyLogger.loggingEnabled)
				{
					LogonLatencyLogger.SetCookie(httpContext, "logonLatency", string.Format("{0}={1}", logonStep, DateTime.UtcNow.Ticks.ToString()), Utilities.GetOutlookDotComDomain(httpContext.Request.GetRequestUrlEvenIfProxied().Host));
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "Unexpected exception while creating logon latency cookie. Exception message: {0}. Stack trace: {1}", ex.Message, ex.StackTrace);
			}
		}

		internal static void UpdateCookie(HttpContext httpContext, string action, DateTime checkPointDateTime)
		{
			try
			{
				if (LogonLatencyLogger.loggingEnabled)
				{
					DateTime utcNow = DateTime.UtcNow;
					long latency = Convert.ToInt64((utcNow - checkPointDateTime).TotalMilliseconds);
					if (action.Equals("ADL"))
					{
						LogonLatencyLogger.AddLatencyHeader(httpContext, "X-AuthDiagInfoLdapLatency", latency);
					}
					else
					{
						LogonLatencyLogger.AddLatencyHeader(httpContext, "X-AuthDiagInfoMservLookupLatency", latency);
					}
					string text = string.Empty;
					string cookieValue = string.Empty;
					if (httpContext.Request.Cookies["logonLatency"] != null && httpContext.Request.Cookies["logonLatency"].Value != null)
					{
						text = LogonLatencyLogger.GetLatestCookieValue(httpContext);
						if (action.Equals("ADL") || action.Equals("MSERVL"))
						{
							int num = 1;
							if (!string.IsNullOrEmpty(text))
							{
								while (text.Contains(action + num))
								{
									num++;
								}
							}
							action += num;
						}
						cookieValue = string.Format("{0}&{1}={2}", text, action, latency.ToString());
						LogonLatencyLogger.SetCookie(httpContext, "logonLatency", cookieValue, Utilities.GetOutlookDotComDomain(httpContext.Request.GetRequestUrlEvenIfProxied().Host));
					}
					else
					{
						string arg = "adlkupltncy";
						if (action.Equals("MSERVL"))
						{
							arg = "mservlkupltncy";
						}
						httpContext.Response.AppendToLog(string.Format("&{0}={1}", arg, latency.ToString()));
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "Unexpected exception while updating logon latency cookie. Exception message: {0}. Stack trace: {1}", ex.Message, ex.StackTrace);
			}
		}

		internal static void UpdateCookie(HttpContext httpContext, string logonStep)
		{
			try
			{
				if (LogonLatencyLogger.loggingEnabled)
				{
					string arg = string.Empty;
					string cookieValue = string.Empty;
					if (httpContext.Request.Cookies["logonLatency"] != null && httpContext.Request.Cookies["logonLatency"].Value != null)
					{
						arg = LogonLatencyLogger.GetLatestCookieValue(httpContext);
						cookieValue = string.Format("{0}&{1}={2}", arg, logonStep, DateTime.UtcNow.Ticks.ToString());
						LogonLatencyLogger.SetCookie(httpContext, "logonLatency", cookieValue, Utilities.GetOutlookDotComDomain(httpContext.Request.GetRequestUrlEvenIfProxied().Host));
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "Unexpected exception while updating logon latency cookie. Exception message: {0}. Stack trace: {1}", ex.Message, ex.StackTrace);
			}
		}

		internal static void WriteLatencyToIISLogAndDeleteCookie(HttpContext httpContext)
		{
			try
			{
				if (LogonLatencyLogger.loggingEnabled)
				{
					long num = -1L;
					long num2 = -1L;
					NameValueCollection nameValueCollection = new NameValueCollection();
					StringBuilder stringBuilder = new StringBuilder();
					if (httpContext.Request.Cookies["logonLatency"] != null && httpContext.Request.Cookies["logonLatency"].Value != null)
					{
						nameValueCollection = HttpUtility.ParseQueryString(LogonLatencyLogger.GetLatestCookieValue(httpContext));
						nameValueCollection["LGN04"] = DateTime.UtcNow.Ticks.ToString();
						LogonLatencyLogger.AssignLatency(nameValueCollection, "LGN02", "LGN01", ref num);
						LogonLatencyLogger.AssignLatency(nameValueCollection, "LGN04", "LGN02", ref num2);
						stringBuilder.AppendFormat("&livelgnltncy={0}&resubrpsltncy={1}", num.ToString(), num2.ToString());
						for (int i = 1; i <= 5; i++)
						{
							if (nameValueCollection["ADL" + i] != null)
							{
								stringBuilder.AppendFormat("&adlkupltncy{0}={1}", i, nameValueCollection["ADL" + i]);
							}
						}
						for (int j = 1; j <= 3; j++)
						{
							if (nameValueCollection["MSERVL" + j] != null)
							{
								stringBuilder.AppendFormat("&mservlkupltncy{0}={1}", j, nameValueCollection["MSERVL" + j]);
							}
						}
					}
					if (!stringBuilder.ToString().Contains("adlkupltncy1"))
					{
						stringBuilder.AppendFormat("&adlkupltncy1=-1", new object[0]);
					}
					httpContext.Response.AppendToLog(stringBuilder.ToString());
					httpContext.Items["logonLatency"] = stringBuilder.ToString();
					LogonLatencyLogger.DeleteCookie(httpContext);
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "Unexpected exception while writing logon latency to IIS. Exception message: {0}. Stack trace: {1}", ex.Message, ex.StackTrace);
			}
		}

		internal static void LogProfileReadLatency(HttpContext httpContext, TimeSpan latency)
		{
			if (!LogonLatencyLogger.loggingEnabled)
			{
				return;
			}
			try
			{
				httpContext.Response.AppendToLog(string.Format("&lvidprflrd={0}", latency.TotalMilliseconds));
			}
			catch (Exception arg)
			{
				ExTraceGlobals.PerformanceTracer.TraceDebug<Exception>(0L, "Exception happened while trying to Log Time In ProfileService. Exception will be ignored: {0}", arg);
			}
		}

		internal static void LogTimeInLiveIdModule(HttpContext httpContext)
		{
			if (!LogonLatencyLogger.loggingEnabled)
			{
				return;
			}
			try
			{
				DateTime? dateTime = httpContext.Items["RequestStartTime"] as DateTime?;
				if (dateTime != null && dateTime != null)
				{
					DateTime utcNow = DateTime.UtcNow;
					long latency = Convert.ToInt64((utcNow - dateTime.Value).TotalMilliseconds);
					long latency2 = -1L;
					DateTime? dateTime2 = httpContext.Items["AdRequestStartTime"] as DateTime?;
					DateTime? dateTime3 = httpContext.Items["AdRequestEndTime"] as DateTime?;
					if (dateTime2 != null && dateTime2 != null && dateTime3 != null && dateTime3 != null)
					{
						latency2 = Convert.ToInt64((dateTime3.Value - dateTime2.Value).TotalMilliseconds);
					}
					httpContext.Response.AppendToLog(string.Format("&lvidmdlltncy={0}&lvidadlookupltncy={1}", latency.ToString(), latency2.ToString()));
					LogonLatencyLogger.AddLatencyHeader(httpContext, "X-AuthDiagInfoLiveIdModuleLatency", latency);
					LogonLatencyLogger.AddLatencyHeader(httpContext, "X-AuthDiagInfoLiveIdModuleAdLookupLatency", latency2);
					httpContext.Items["RequestStartTime"] = null;
				}
			}
			catch (Exception arg)
			{
				ExTraceGlobals.PerformanceTracer.TraceDebug<Exception>(0L, "Exception happened while trying to Log Time In LiveIdModule. Exception will be ignored: {0}", arg);
			}
		}

		private static void AssignLatency(NameValueCollection values, string logonCurrentStepTicks, string logonPreviousStepTicks, ref long currentValue)
		{
			try
			{
				if (values[logonCurrentStepTicks] != null && values[logonPreviousStepTicks] != null)
				{
					TimeSpan timeSpan = new TimeSpan(Convert.ToInt64(values[logonCurrentStepTicks]) - Convert.ToInt64(values[logonPreviousStepTicks]));
					currentValue = Convert.ToInt64(timeSpan.TotalMilliseconds);
				}
				else
				{
					currentValue = 0L;
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<string, string>(0L, "Unexpected exception while computing logon latency. Exception message: {0}. Stack trace: {1}", ex.Message, ex.StackTrace);
			}
		}

		private static void SetCookie(HttpContext httpContext, string cookieName, string cookieValue, string cookieDomain)
		{
			HttpCookie httpCookie = httpContext.Response.Cookies.Get(cookieName);
			httpCookie.HttpOnly = true;
			httpCookie.Path = "/";
			httpCookie.Value = cookieValue;
			if (cookieDomain != null)
			{
				httpCookie.Domain = cookieDomain;
			}
			httpContext.Response.Cookies.Set(httpCookie);
		}

		private static string GetLatestCookieValue(HttpContext httpContext)
		{
			if (httpContext.Response.Cookies["logonLatency"] != null && httpContext.Response.Cookies["logonLatency"].Value != null)
			{
				return httpContext.Response.Cookies["logonLatency"].Value;
			}
			return httpContext.Request.Cookies["logonLatency"].Value;
		}

		private static void DeleteCookie(HttpContext httpContext)
		{
			if (httpContext.Request.Cookies["logonLatency"] != null)
			{
				httpContext.Request.Cookies["logonLatency"].Expires = DateTime.UtcNow.AddDays(-100.0);
				httpContext.Request.Cookies["logonLatency"].Domain = Utilities.GetOutlookDotComDomain(httpContext.Request.GetRequestUrlEvenIfProxied().Host);
				httpContext.Response.Cookies.Set(httpContext.Request.Cookies["logonLatency"]);
			}
		}

		internal const string LogonLatencyCookieName = "logonLatency";

		internal const string LogLiveLogonLatencyKey = "LogLiveLogonLatency";

		internal const string RequestStartTimeKey = "RequestStartTime";

		internal const string AdRequestStartTimeKey = "AdRequestStartTime";

		internal const string AdRequestEndTimeKey = "AdRequestEndTime";

		internal const string BeforeLogon = "LGN01";

		internal const string RPSTicketReceived = "LGN02";

		internal const string LiveAuthenticationCompleted = "LGN04";

		internal const string ADLookupLatency = "ADL";

		internal const int ADLookupLatencyCount = 5;

		internal const string MservLookupLatency = "MSERVL";

		internal const int MservLookupLatencyCount = 3;

		internal const string ADLookupLatencyHeader = "X-AuthDiagInfoLdapLatency";

		internal const string MservLookupLatencyHeader = "X-AuthDiagInfoMservLookupLatency";

		private static bool loggingEnabled;
	}
}
