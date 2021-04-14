using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Security
{
	public static class Utilities
	{
		public static string ApplicationVersion
		{
			get
			{
				return Utilities.applicationVersion;
			}
		}

		public static string ImagesPath
		{
			get
			{
				return Utilities.imagesPath;
			}
		}

		public static string HtmlEncode(string s)
		{
			return AntiXssEncoder.HtmlEncode(s, false);
		}

		public static void HtmlEncode(string s, TextWriter writer)
		{
			if (s == null || s.Length == 0)
			{
				return;
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(AntiXssEncoder.HtmlEncode(s, false));
		}

		internal static void HandleException(HttpContext httpContext, Exception exception, bool shouldSend440Response = false)
		{
			try
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<Exception>(0L, "Live ID Auth Module Error: {0}.", exception);
				Type type = exception.GetType();
				httpContext.Response.AppendToLog(string.Format("&LiveIdError={0}", type.Name));
				if (shouldSend440Response && Utilities.Need440Response(httpContext.Request))
				{
					Utilities.Render440TimeoutResponse(httpContext.Response, httpContext.Request.HttpMethod, httpContext);
				}
				else
				{
					ErrorInformation exceptionHandlingInformation = Utilities.GetExceptionHandlingInformation(httpContext, exception);
					StringBuilder stringBuilder = new StringBuilder("/owa/auth/errorfe.aspx");
					stringBuilder.AppendFormat("?{0}={1}", "httpCode", 500);
					stringBuilder.AppendFormat("&{0}={1}", "ts", DateTime.UtcNow.ToFileTimeUtc());
					if (!AuthCommon.IsFrontEnd)
					{
						stringBuilder.AppendFormat("&{0}={1}", "be", Environment.MachineName);
					}
					if (exceptionHandlingInformation.Exception != null)
					{
						stringBuilder.AppendFormat("&{0}={1}", "authError", exceptionHandlingInformation.Exception.GetType().Name);
					}
					if (exceptionHandlingInformation.MessageId != null)
					{
						stringBuilder.AppendFormat("&{0}={1}", "msg", exceptionHandlingInformation.MessageId.Value);
						exceptionHandlingInformation.AppendMessageParametersToUrl(stringBuilder);
					}
					if (!string.IsNullOrEmpty(exceptionHandlingInformation.CustomParameterName) && !string.IsNullOrEmpty(exceptionHandlingInformation.CustomParameterValue))
					{
						stringBuilder.AppendFormat("&{0}={1}", exceptionHandlingInformation.CustomParameterName, exceptionHandlingInformation.CustomParameterValue);
					}
					if (exceptionHandlingInformation.Mode != null)
					{
						stringBuilder.AppendFormat("&{0}={1}", "m", exceptionHandlingInformation.Mode.Value);
					}
					httpContext.Response.Headers.Add("X-Auth-Error", type.FullName);
					httpContext.Response.Redirect(stringBuilder.ToString());
				}
			}
			catch (HttpException arg)
			{
				ExTraceGlobals.LiveIdAuthenticationModuleTracer.TraceError<HttpException>(0L, "Could not handle auth error: {0}.", arg);
			}
		}

		internal static bool Need440Response(HttpRequest request)
		{
			return request.IsNotGetOrOehRequest();
		}

		internal static void Render440TimeoutResponse(HttpResponse response, string httpMethod, HttpContext httpContext)
		{
			string body = httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) ? "<HTML><BODY>440 Login Timeout</BODY></HTML>" : "<HTML><SCRIPT>if (parent.navbar != null) parent.location = self.location;else self.location = self.location;</SCRIPT><BODY>440 Login Timeout</BODY></HTML>";
			response.TrySkipIisCustomErrors = true;
			Utilities.RenderErrorPage(response, 440, "440 Login Timeout", body, httpContext);
		}

		internal static void RenderErrorPage(HttpResponse response, int statusCode, string status, string body, HttpContext httpContext)
		{
			response.ClearHeaders();
			response.ClearContent();
			response.ContentType = "text/html";
			response.StatusCode = statusCode;
			response.Status = status;
			if (response.StatusCode == 440 && httpContext != null)
			{
				Utilities.SetCookie(httpContext, "lastResponse", 440.ToString(), null);
			}
			response.Write(body);
			response.End();
		}

		internal static void SetCookie(HttpContext httpContext, string cookieName, string cookieValue, string cookieDomain)
		{
			HttpCookie httpCookie = new HttpCookie(cookieName);
			httpCookie.HttpOnly = true;
			httpCookie.Path = "/";
			httpCookie.Value = cookieValue;
			if (cookieDomain != null)
			{
				httpCookie.Domain = cookieDomain;
			}
			httpContext.Response.Cookies.Add(httpCookie);
		}

		internal static void ExecutePageAndCompleteRequest(HttpContext httpContext, string page)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			try
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					httpContext.Server.Execute(page, stringWriter);
				}
				httpContext.Response.Write(stringBuilder);
				httpContext.Response.StatusCode = 200;
				httpContext.Response.AppendHeader("Content-Length", httpContext.Response.Output.Encoding.GetByteCount(stringBuilder.ToString()).ToString());
				httpContext.Response.Flush();
				httpContext.ApplicationInstance.CompleteRequest();
			}
			finally
			{
				httpContext.Response.End();
			}
		}

		private static ErrorInformation GetExceptionHandlingInformation(HttpContext httpContext, Exception exception)
		{
			ErrorInformation errorInformation = new ErrorInformation
			{
				Exception = exception
			};
			if (exception is OrgIdMailboxRecentlyCreatedException)
			{
				OrgIdMailboxRecentlyCreatedException ex = exception as OrgIdMailboxRecentlyCreatedException;
				errorInformation.Message = ex.Message;
				errorInformation.MessageId = new Strings.IDs?(ex.ErrorMessageStringId);
				errorInformation.AddMessageParameter(ex.UserName);
				errorInformation.AddMessageParameter(ex.HoursBetweenAccountCreationAndNow.ToString());
				errorInformation.Mode = ex.ErrorMode;
			}
			else if (exception is OrgIdMailboxNotFoundException)
			{
				OrgIdMailboxNotFoundException ex2 = exception as OrgIdMailboxNotFoundException;
				errorInformation.Message = ex2.Message;
				errorInformation.MessageId = new Strings.IDs?(ex2.ErrorMessageStringId);
				errorInformation.AddMessageParameter(ex2.UserName);
				errorInformation.Mode = ex2.ErrorMode;
			}
			else if (exception is OrgIdLogonException)
			{
				OrgIdLogonException ex3 = exception as OrgIdLogonException;
				errorInformation.Message = ex3.Message;
				errorInformation.MessageId = new Strings.IDs?(ex3.ErrorMessageStringId);
				errorInformation.MessageParameter = ex3.UserName;
			}
			else if (exception is AppPasswordAccessException)
			{
				AppPasswordAccessException ex4 = exception as AppPasswordAccessException;
				errorInformation.Message = ex4.Message;
				errorInformation.MessageId = new Strings.IDs?(ex4.ErrorMessageStringId);
			}
			else if (exception is LiveClientException || exception is LiveConfigurationException || exception is LiveTransientException || exception is LiveOperationException)
			{
				errorInformation.Message = exception.Message;
				errorInformation.MessageId = new Strings.IDs?(1317300008);
				string text = httpContext.Request.QueryString["realm"];
				if (!string.IsNullOrEmpty(text))
				{
					errorInformation.AddMessageParameter(text);
				}
			}
			else if (exception is AccountTerminationException)
			{
				AccountTerminationException ex5 = exception as AccountTerminationException;
				errorInformation.Message = ex5.Message;
				errorInformation.MessageId = new Strings.IDs?(ex5.ErrorMessageStringId);
				errorInformation.MessageParameter = ex5.AccountState.ToString();
			}
			return errorInformation;
		}

		public static string GetImagesPath()
		{
			return Utilities.imagesPath;
		}

		public static string GetUserDomain(string user)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(user))
			{
				int num = user.IndexOf("@");
				if (num != -1)
				{
					result = user.Substring(num + 1).Trim();
				}
			}
			return result;
		}

		public static string GetOutlookDotComDomain(string hostName)
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

		public static string AppendUrlParameter(Uri uri, string name, string value)
		{
			StringBuilder stringBuilder = new StringBuilder(uri.ToString());
			if (string.IsNullOrEmpty(uri.Query))
			{
				stringBuilder.Append("?");
			}
			else
			{
				stringBuilder.Append("&");
			}
			stringBuilder.Append(name);
			stringBuilder.Append("=");
			stringBuilder.Append(HttpUtility.UrlEncode(value));
			return stringBuilder.ToString();
		}

		public static void JavascriptEncode(string s, TextWriter writer, bool escapeNonAscii)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			int i = 0;
			while (i < s.Length)
			{
				char c = s[i];
				if (c <= '"')
				{
					if (c != '\n')
					{
						if (c != '\r')
						{
							switch (c)
							{
							case '!':
							case '"':
								goto IL_78;
							default:
								goto IL_B3;
							}
						}
						else
						{
							writer.Write('\\');
							writer.Write('r');
						}
					}
					else
					{
						writer.Write('\\');
						writer.Write('n');
					}
				}
				else if (c <= '/')
				{
					if (c != '\'' && c != '/')
					{
						goto IL_B3;
					}
					goto IL_78;
				}
				else
				{
					switch (c)
					{
					case '<':
					case '>':
						goto IL_78;
					case '=':
						goto IL_B3;
					default:
						if (c == '\\')
						{
							goto IL_78;
						}
						goto IL_B3;
					}
				}
				IL_E7:
				i++;
				continue;
				IL_78:
				writer.Write('\\');
				writer.Write(s[i]);
				goto IL_E7;
				IL_B3:
				if (escapeNonAscii && s[i] > '\u007f')
				{
					writer.Write("\\u{0:x4}", (ushort)s[i]);
					goto IL_E7;
				}
				writer.Write(s[i]);
				goto IL_E7;
			}
		}

		public static void JavascriptEncode(string s, TextWriter writer)
		{
			Utilities.JavascriptEncode(s, writer, false);
		}

		public static CultureInfo GetSupportedBrowserLanguage(string language)
		{
			List<CultureInfo> list = new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));
			string[] array = new string[list.Count];
			int num = 0;
			foreach (CultureInfo cultureInfo in list)
			{
				array[num++] = cultureInfo.Name;
			}
			Array.Sort<string>(array, StringComparer.OrdinalIgnoreCase);
			if (Array.BinarySearch<string>(array, language, StringComparer.OrdinalIgnoreCase) >= 0)
			{
				return CultureInfo.GetCultureInfo(language);
			}
			return null;
		}

		public static string ValidateLanguageTag(string tag)
		{
			if (tag.Length < 1 || tag.Length > 44)
			{
				return null;
			}
			int num = 0;
			while (num < tag.Length && char.IsWhiteSpace(tag[num]))
			{
				num++;
			}
			if (num == tag.Length)
			{
				return null;
			}
			int num2 = num;
			for (int i = 0; i < 3; i++)
			{
				int num3 = 0;
				while (num3 < 8 && num2 < tag.Length && ((tag[num2] >= 'a' && tag[num2] <= 'z') || (tag[num2] >= 'A' && tag[num2] <= 'Z')))
				{
					num3++;
					num2++;
				}
				if (num2 == tag.Length || tag[num2] != '-')
				{
					break;
				}
				num2++;
			}
			if (num2 != tag.Length && tag[num2] != ';' && !char.IsWhiteSpace(tag[num2]))
			{
				return null;
			}
			return tag.Substring(num, num2 - num);
		}

		public static string GetAccessURLFromHostnameAndRealm(string hostName, string realm, bool isVanityDomain)
		{
			string text = Uri.UriSchemeHttps;
			if (isVanityDomain)
			{
				text = Uri.UriSchemeHttp;
			}
			if (string.IsNullOrEmpty(realm))
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}://{1}/owa/", new object[]
				{
					text,
					HttpUtility.UrlEncode(hostName)
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}://{1}/owa/?realm={2}", new object[]
			{
				text,
				HttpUtility.UrlEncode(hostName),
				HttpUtility.UrlEncode(realm)
			});
		}

		public const string VanityDomainQueryStringParameterName = "vd";

		private const string HttpMethodGet = "GET";

		private const int ResponseStatusCode440 = 440;

		private const string ResponseStatus440 = "440 Login Timeout";

		private const string ResponseBody440Get = "<HTML><BODY>440 Login Timeout</BODY></HTML>";

		private const string ResponseBody440Post = "<HTML><SCRIPT>if (parent.navbar != null) parent.location = self.location;else self.location = self.location;</SCRIPT><BODY>440 Login Timeout</BODY></HTML>";

		internal const string LastResponseStatusCodeCookieName = "lastResponse";

		private const string ResponseContentTypeTextHtml = "text/html";

		private static readonly string applicationVersion = typeof(Utilities).GetApplicationVersion();

		private static readonly string imagesPath = "/LIDAuth/" + Utilities.ApplicationVersion + "/images/";

		public static readonly string LiveIdUrlParameter = "liveId";

		public static readonly string EducationUrlParameter = "newurl";

		public static readonly string DestinationUrlParameter = "destination";

		public static readonly string UserNameParameter = "username";
	}
}
