using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal class PfdTracer
	{
		static PfdTracer()
		{
			PfdTracer.NotInterestedCookies.Add("ASP.NET_SessionId");
			PfdTracer.NotInterestedCookies.Add("cadata");
			PfdTracer.NotInterestedCookies.Add("cadataIV");
			PfdTracer.NotInterestedCookies.Add("cadataKey");
			PfdTracer.NotInterestedCookies.Add("cadataSig");
			PfdTracer.NotInterestedCookies.Add("cadataTTL");
			PfdTracer.NotInterestedCookies.Add("PBack");
			PfdTracer.NotInterestedHeaders.Add("Accept");
			PfdTracer.NotInterestedHeaders.Add("Accept-Encoding");
			PfdTracer.NotInterestedHeaders.Add("Accept-Language");
			PfdTracer.NotInterestedHeaders.Add("Connection");
			PfdTracer.NotInterestedHeaders.Add("Content-Length");
			PfdTracer.NotInterestedHeaders.Add("Content-Type");
			PfdTracer.NotInterestedHeaders.Add("Cookie");
			PfdTracer.NotInterestedHeaders.Add("Expect");
			PfdTracer.NotInterestedHeaders.Add("Host");
			PfdTracer.NotInterestedHeaders.Add("If-Modified-Since");
			PfdTracer.NotInterestedHeaders.Add("Proxy-Connection");
			PfdTracer.NotInterestedHeaders.Add("Range");
			PfdTracer.NotInterestedHeaders.Add("Referer");
			PfdTracer.NotInterestedHeaders.Add("Transfer-Encoding");
			PfdTracer.NotInterestedHeaders.Add("User-Agent");
			PfdTracer.NotInterestedHeaders.Add("Accept-Ranges");
			PfdTracer.NotInterestedHeaders.Add("Cache-Control");
			PfdTracer.NotInterestedHeaders.Add("ETag");
			PfdTracer.NotInterestedHeaders.Add("Last-Modified");
			PfdTracer.NotInterestedHeaders.Add("Server");
			PfdTracer.NotInterestedHeaders.Add("X-AspNet-Version");
			PfdTracer.NotInterestedHeaders.Add("X-Powered-By");
			PfdTracer.NotInterestedHeaders.Add("X-UA-Compatible");
			if (PfdTracer.PfdTraceToFile.Value)
			{
				PfdTracer.traceDirectory = Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\HttpProxy");
			}
		}

		public PfdTracer(int traceContext, int hashCode)
		{
			this.traceContext = traceContext;
			this.hashCode = hashCode;
			string text = HttpRuntime.AppDomainAppVirtualPath;
			if (string.IsNullOrEmpty(text))
			{
				text = "unknown";
			}
			else
			{
				text = text.Replace("\\", string.Empty).Replace("/", string.Empty);
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.vdir = text;
			}
		}

		private static bool IsTraceDisabled
		{
			get
			{
				return !PfdTracer.PfdTraceToDebugger.Value && !PfdTracer.PfdTraceToFile.Value && (!PfdTracer.traceToEtl || !ExTraceGlobals.BriefTracer.IsTraceEnabled(TraceType.PfdTrace));
			}
		}

		private string TraceFilePath
		{
			get
			{
				if (this.traceFilePath == null)
				{
					this.traceFilePath = Path.Combine(PfdTracer.traceDirectory, "trace-" + this.vdir + ".log");
				}
				return this.traceFilePath;
			}
		}

		public void TraceRequest(string stage, HttpRequest request)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			string s = string.Format("{0}: {1}: {2} {3}", new object[]
			{
				this.traceContext,
				stage,
				request.HttpMethod,
				request.Url.ToString()
			});
			this.Write(s);
		}

		public void TraceRequest(string stage, HttpWebRequest request)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			string s = string.Format("{0}: {1}: {2} {3}", new object[]
			{
				this.traceContext,
				stage,
				request.Method,
				request.RequestUri.ToString()
			});
			this.Write(s);
		}

		public void TraceResponse(string stage, HttpResponse response)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			int statusCode = response.StatusCode;
			string s;
			if (statusCode == 301 || statusCode == 302 || statusCode == 303 || statusCode == 305 || statusCode == 307)
			{
				string text = response.Headers["Location"];
				s = string.Format("{0}: {1}: redirected {2} to {3}", new object[]
				{
					this.traceContext,
					stage,
					response.StatusCode,
					text ?? "null"
				});
			}
			else
			{
				s = string.Format("{0}: {1}: responds {2} {3}", new object[]
				{
					this.traceContext,
					stage,
					response.StatusCode,
					response.StatusDescription
				});
			}
			this.Write(s);
		}

		public void TraceResponse(string stage, HttpWebResponse response)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			int statusCode = (int)response.StatusCode;
			string s;
			if (statusCode == 301 || statusCode == 302 || statusCode == 303 || statusCode == 305 || statusCode == 307)
			{
				s = string.Format("{0}: {1}: {2} redirected {3} to {4}", new object[]
				{
					this.traceContext,
					stage,
					response.Server,
					response.StatusCode,
					response.GetResponseHeader("Location")
				});
			}
			else
			{
				s = string.Format("{0}: {1}: {2} responds {3} {4}", new object[]
				{
					this.traceContext,
					stage,
					response.Server,
					response.StatusCode,
					response.StatusDescription
				});
			}
			this.Write(s);
		}

		public void TraceProxyTarget(AnchoredRoutingTarget anchor)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			string s = string.Format("{0}: {1}: {2}", this.traceContext, "AnchoredRoutingTarget", anchor.ToString());
			this.Write(s);
		}

		public void TraceProxyTarget(string key, string fqdn)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			string s = string.Format("{0}: {1}: select BE server {2} based on {3}", new object[]
			{
				this.traceContext,
				"Cookie",
				fqdn,
				key
			});
			this.Write(s);
		}

		public void TraceRedirect(string stage, string url)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			string s = string.Format("{0}: {1}: force redirect to {2}", this.traceContext, stage, url);
			this.Write(s);
		}

		public void TraceHeaders(string stage, WebHeaderCollection originalHeaders, WebHeaderCollection newHeaders)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			if (originalHeaders == null || newHeaders == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(20 * originalHeaders.Count);
			stringBuilder.Append(string.Format("{0}: {1}: ", this.traceContext, stage));
			PfdTracer.TraceDiffs(originalHeaders, newHeaders, PfdTracer.NotInterestedHeaders, stringBuilder);
			this.Write(stringBuilder.ToString());
		}

		public void TraceHeaders(string stage, NameValueCollection originalHeaders, NameValueCollection newHeaders)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			if (originalHeaders == null || newHeaders == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(20 * originalHeaders.Count);
			stringBuilder.Append(string.Format("{0}: {1}: ", this.traceContext, stage));
			PfdTracer.TraceDiffs(originalHeaders, newHeaders, PfdTracer.NotInterestedHeaders, stringBuilder);
			this.Write(stringBuilder.ToString());
		}

		public void TraceCookies(string stage, HttpCookieCollection originalCookies, CookieContainer newCookies)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			if (originalCookies == null || newCookies == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(20 * originalCookies.Count);
			stringBuilder.Append(string.Format("{0}: {1}: ", this.traceContext, stage));
			PfdTracer.TraceDiffs(originalCookies, PfdTracer.CopyCookies(newCookies), PfdTracer.NotInterestedCookies, stringBuilder);
			this.Write(stringBuilder.ToString());
		}

		public void TraceCookies(string stage, CookieCollection originalCookies, HttpCookieCollection newCookies)
		{
			if (PfdTracer.IsTraceDisabled)
			{
				return;
			}
			if (originalCookies == null || newCookies == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(20 * originalCookies.Count);
			stringBuilder.Append(string.Format("{0}: {1}: ", this.traceContext, stage));
			PfdTracer.TraceDiffs(PfdTracer.CopyCookies(originalCookies), newCookies, PfdTracer.NotInterestedCookies, stringBuilder);
			this.Write(stringBuilder.ToString());
		}

		private static NameValueCollection CopyCookies(CookieCollection cookies)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(cookies.Count, StringComparer.OrdinalIgnoreCase);
			foreach (object obj in cookies)
			{
				Cookie cookie = (Cookie)obj;
				nameValueCollection.Add(cookie.Name, cookie.Value);
			}
			return nameValueCollection;
		}

		private static NameValueCollection CopyCookies(CookieContainer cookies)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(cookies.Count, StringComparer.OrdinalIgnoreCase);
			BindingFlags invokeAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField;
			try
			{
				Hashtable hashtable = (Hashtable)cookies.GetType().InvokeMember("m_domainTable", invokeAttr, null, cookies, new object[0]);
				foreach (object obj in hashtable.Values)
				{
					SortedList sortedList = (SortedList)obj.GetType().InvokeMember("m_list", invokeAttr, null, obj, new object[0]);
					foreach (object obj2 in sortedList.Values)
					{
						CookieCollection cookieCollection = (CookieCollection)obj2;
						foreach (object obj3 in cookieCollection)
						{
							Cookie cookie = (Cookie)obj3;
							nameValueCollection.Add(cookie.Name, cookie.Value);
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return nameValueCollection;
		}

		private static string GetValue(object o, string key)
		{
			NameValueCollection nameValueCollection = o as NameValueCollection;
			if (nameValueCollection != null)
			{
				return nameValueCollection[key];
			}
			CookieCollection cookieCollection = o as CookieCollection;
			if (cookieCollection != null)
			{
				Cookie cookie = cookieCollection[key];
				if (cookie != null)
				{
					return cookie.Value;
				}
				return null;
			}
			else
			{
				HttpCookieCollection httpCookieCollection = o as HttpCookieCollection;
				if (httpCookieCollection == null)
				{
					return null;
				}
				HttpCookie httpCookie = httpCookieCollection[key];
				if (httpCookie != null)
				{
					return httpCookie.Value;
				}
				return null;
			}
		}

		private static void TraceDiffs(NameObjectCollectionBase original, NameObjectCollectionBase revised, HashSet<string> notInterestingNames, StringBuilder result)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (object obj in original)
			{
				string text = (string)obj;
				if (!notInterestingNames.Contains(text))
				{
					string value = PfdTracer.GetValue(revised, text);
					string value2 = PfdTracer.GetValue(original, text);
					if (value == null)
					{
						result.Append("-" + text + ",");
					}
					else
					{
						hashSet.Add(text);
						if (string.Compare(value2, value, StringComparison.OrdinalIgnoreCase) != 0)
						{
							result.Append("*" + text + ",");
						}
					}
				}
			}
			foreach (object obj2 in revised)
			{
				string text2 = (string)obj2;
				if (!notInterestingNames.Contains(text2) && !hashSet.Contains(text2))
				{
					result.Append("+" + text2 + ",");
				}
			}
		}

		private void Write(string s)
		{
			if (PfdTracer.PfdTraceToDebugger.Value)
			{
				bool isAttached = Debugger.IsAttached;
			}
			if (PfdTracer.traceToEtl)
			{
				ExTraceGlobals.BriefTracer.TracePfd((long)this.hashCode, s);
			}
			if (PfdTracer.PfdTraceToFile.Value)
			{
				using (StreamWriter streamWriter = new StreamWriter(this.TraceFilePath, true))
				{
					streamWriter.WriteLine(s);
				}
			}
		}

		public const string ClientRequest = "ClientRequest";

		public const string ProxyRequest = "ProxyRequest";

		public const string ProxyLogonRequest = "ProxyLogonRequest";

		public const string ClientResponse = "ClientResponse";

		public const string ProxyResponse = "ProxyResponse";

		public const string ProxyLogonResponse = "ProxyLogonResponse";

		public const string NeedLanguage = "EcpOwa442NeedLanguage";

		public const string FbaAuth = "FbaAuth";

		public static readonly BoolAppSettingsEntry PfdTraceToFile = new BoolAppSettingsEntry(HttpProxySettings.Prefix("PfdTraceToFile"), false, ExTraceGlobals.VerboseTracer);

		public static readonly BoolAppSettingsEntry PfdTraceToDebugger = new BoolAppSettingsEntry(HttpProxySettings.Prefix("PfdTraceToDebugger"), false, ExTraceGlobals.VerboseTracer);

		private static readonly HashSet<string> NotInterestedHeaders = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private static readonly HashSet<string> NotInterestedCookies = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private static bool traceToEtl = true;

		private static string traceDirectory = null;

		private readonly int traceContext;

		private readonly int hashCode;

		private readonly string vdir;

		private string traceFilePath;
	}
}
