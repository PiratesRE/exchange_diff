using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Configuration.DelegatedAuthentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class RequestFilterModule : IHttpModule
	{
		private static bool BlockNonCafeRequest
		{
			get
			{
				if (RequestFilterModule.blockNonCafeRequest == null)
				{
					lock (RequestFilterModule.blockNonCafeRequestLock)
					{
						if (RequestFilterModule.blockNonCafeRequest == null)
						{
							RequestFilterModule.blockNonCafeRequest = new bool?(!string.Equals(ConfigurationManager.AppSettings["AllowDirectBERequest"], "true", StringComparison.OrdinalIgnoreCase) && !string.Equals(ConfigurationManager.AppSettings["IsOSPEnvironment"], "true", StringComparison.OrdinalIgnoreCase) && !DatacenterRegistry.IsForefrontForOffice());
						}
					}
				}
				return RequestFilterModule.blockNonCafeRequest.Value;
			}
		}

		public void Init(HttpApplication application)
		{
			application.BeginRequest += this.OnBeginRequest;
			application.AuthenticateRequest += this.OnAuthenticationRequest;
		}

		public void Dispose()
		{
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			if (RequestFilterModule.BlockNonCafeRequest)
			{
				string a = HttpContext.Current.Request.Headers[WellKnownHeader.XIsFromCafe];
				if (!string.Equals(a, "1"))
				{
					httpContext.Response.Headers.Set("X-BlockDirectBERequest", "1");
					throw new BadRequestException();
				}
			}
			if (Utility.IsResourceRequest(request.Url.LocalPath))
			{
				return;
			}
			httpContext.Response.Headers.Set("X-Content-Type-Options", "nosniff");
			this.ProcessFeatureRedirection(httpContext, request);
			RequestTypeInfo requestTypeInfo = RequestFilterModule.DetermineRequestType(request);
			this.StampTokenToHeader(httpContext, request, requestTypeInfo);
			this.HandleRedirection(httpContext, request, requestTypeInfo);
		}

		private void HandleRedirection(HttpContext httpContext, HttpRequest request, RequestTypeInfo requestTypeInfo)
		{
			string text = null;
			if (requestTypeInfo.Need302Redirect)
			{
				text = request.RawUrl.Insert(request.FilePath.Length, "/");
			}
			else if (requestTypeInfo.NeedRedirectTargetTenant)
			{
				text = EcpUrl.ProcessUrl(request.RawUrl, true);
				text = EcpUrl.RemoveQueryParameter(text, requestTypeInfo.IsDelegatedAdminRequest ? "delegatedorg" : "organizationcontext", false);
			}
			else if (requestTypeInfo.UseImplicitPathRewrite && requestTypeInfo.IsSecurityTokenPresented)
			{
				text = request.Headers[RequestFilterModule.OriginalUrlKey];
			}
			if (text != null)
			{
				ExTraceGlobals.RedirectTracer.TraceInformation<string>(0, 0L, "[RequestFilterModule::HandleRedirection] Redirect to {0}).", text);
				httpContext.Response.Redirect(text, true);
			}
		}

		internal static RequestTypeInfo DetermineRequestType(HttpRequest httpRequest)
		{
			string filePath = httpRequest.FilePath;
			NameValueCollection queryString = httpRequest.QueryString;
			RequestTypeInfo result = default(RequestTypeInfo);
			string text = queryString["delegatedorg"];
			if (!string.IsNullOrEmpty(text))
			{
				result.NeedRedirectTargetTenant = true;
				result.IsDelegatedAdminRequest = true;
				result.TargetTenant = text;
			}
			string text2 = queryString["organizationcontext"];
			if (!string.IsNullOrEmpty(text2))
			{
				result.NeedRedirectTargetTenant = true;
				result.IsByoidAdmin = true;
				result.TargetTenant = text2;
			}
			if (result.IsDelegatedAdminRequest && result.IsByoidAdmin)
			{
				throw new BadRequestException(new Exception("Both delegatedorg and organizationcontext parameters are specified in request url."));
			}
			Match match = RequestFilterModule.regex.Match(filePath);
			if (match.Success)
			{
				Group group = match.Groups["isOrgContext"];
				Group group2 = match.Groups["targetTenant"];
				Group group3 = match.Groups["esoAddress"];
				Group group4 = match.Groups["closeSlash"];
				if (group2.Success)
				{
					if (result.NeedRedirectTargetTenant)
					{
						throw new BadRequestException(new Exception("Both '/@' style and parameter style are used in request url."));
					}
					if (group.Success)
					{
						result.IsByoidAdmin = true;
					}
					else
					{
						result.IsDelegatedAdminRequest = true;
						if (filePath.EndsWith("/", StringComparison.InvariantCulture))
						{
							result.UseImplicitPathRewrite = true;
						}
						result.IsSecurityTokenPresented = DelegatedAuthenticationModule.IsSecurityTokenPresented(httpRequest);
					}
					result.TargetTenant = group2.Value;
				}
				if (group3.Success)
				{
					result.IsEsoRequest = true;
					result.EsoMailboxSmtpAddress = group3.Value;
				}
				if (!group4.Success)
				{
					result.Need302Redirect = true;
				}
			}
			return result;
		}

		private void StampTokenToHeader(HttpContext httpContext, HttpRequest request, RequestTypeInfo requestTypeInfo)
		{
			if (requestTypeInfo.IsDelegatedAdminRequest)
			{
				request.Headers.Set(RequestFilterModule.TargetTenantKey, requestTypeInfo.TargetTenant);
				string text = request.RawUrl;
				if (requestTypeInfo.UseImplicitPathRewrite)
				{
					text = text.Insert(request.FilePath.Length, "default.aspx");
				}
				request.Headers.Set(RequestFilterModule.OriginalUrlKey, text);
				if (requestTypeInfo.IsSecurityTokenPresented)
				{
					httpContext.Items[RequestFilterModule.NoResolveIdKey] = "1";
				}
			}
			else if (requestTypeInfo.IsByoidAdmin)
			{
				request.Headers.Set(RequestFilterModule.OrganizationContextKey, requestTypeInfo.TargetTenant);
			}
			if (requestTypeInfo.IsEsoRequest)
			{
				request.Headers.Set("msExchEcpESOUser", requestTypeInfo.EsoMailboxSmtpAddress);
			}
		}

		private void OnAuthenticationRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			if (httpContext.Request.IsAuthenticated)
			{
				httpContext.Items["LogonUserIdentity"] = httpContext.User.Identity;
			}
			if (!string.IsNullOrEmpty(request.Headers["msExchPathRewritten"]))
			{
				return;
			}
			int num = 0;
			string text = request.Headers[RequestFilterModule.TargetTenantKey];
			if (!string.IsNullOrEmpty(text))
			{
				num += text.Length + 2;
			}
			else
			{
				string text2 = request.Headers[RequestFilterModule.OrganizationContextKey];
				if (!string.IsNullOrEmpty(text2))
				{
					num += text2.Length + 3;
				}
			}
			string text3 = request.Headers["msExchEcpESOUser"];
			if (!string.IsNullOrEmpty(text3))
			{
				num += text3.Length + 1;
			}
			if (num > 0)
			{
				string empty = RequestFilterModule.appDomainAppVirtualPath;
				if (empty.Length == 1 && empty[0] == '/')
				{
					empty = string.Empty;
				}
				string text4 = empty + request.RawUrl.Substring(empty.Length + num);
				if (!this.ShouldRedirectRequest(request))
				{
					httpContext.RewritePath(text4);
					request.Headers.Set("msExchPathRewritten", "1");
					ExTraceGlobals.EventLogTracer.TraceInformation<string, string>(0, 0L, "Path rewritten from '{0}' to '{1}'.", request.RawUrl, text4);
					return;
				}
				ExTraceGlobals.RedirectTracer.TraceInformation<string>(0, 0L, "[RequestFilterModule::OnAuthenticateRequest] Redirect to {0}).", text4);
				httpContext.Response.Redirect(text4, true);
			}
		}

		private bool ShouldRedirectRequest(HttpRequest request)
		{
			return request.FilePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
		}

		private void ProcessFeatureRedirection(HttpContext httpContext, HttpRequest request)
		{
			string text = request.QueryString["ftr"];
			if (!text.IsNullOrBlank())
			{
				try
				{
					EcpFeature ecpFeature = (EcpFeature)Enum.Parse(typeof(EcpFeature), text, true);
					string text2 = "ftr=" + text;
					int num = request.Url.Query.IndexOf(text2, StringComparison.InvariantCultureIgnoreCase);
					string text3 = request.Url.Query.Substring(0, num) + request.Url.Query.Substring(num + text2.Length);
					EcpFeatureDescriptor featureDescriptor = ecpFeature.GetFeatureDescriptor();
					string query = featureDescriptor.AbsoluteUrl.Query;
					text3 = ((text3.Length > 1 && !string.IsNullOrEmpty(query) && query.Contains("?")) ? ("&" + text3.Substring(1)) : ((text3 == "?") ? string.Empty : text3));
					string text4 = (featureDescriptor.UseAbsoluteUrl ? featureDescriptor.AbsoluteUrl.ToEscapedString() : (request.Url.LocalPath + query)) + text3;
					Uri uri = new Uri(text4);
					NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
					StringBuilder stringBuilder = new StringBuilder();
					foreach (object obj in nameValueCollection)
					{
						string text5 = (string)obj;
						if (!string.IsNullOrEmpty(text5))
						{
							stringBuilder.AppendFormat("{0}={1}&", text5, nameValueCollection[text5].Split(new char[]
							{
								','
							})[0]);
						}
					}
					text4 = text4.Replace(uri.Query, "?" + stringBuilder.ToString().TrimEnd(new char[]
					{
						'&'
					}));
					ExTraceGlobals.RedirectTracer.TraceInformation<string>(0, 0L, "[RequestFilterModule::ProcessFeatureRedirection] Redirect to {0}).", text4);
					httpContext.Response.Redirect(text4);
				}
				catch (ArgumentException innerException)
				{
					throw new BadQueryParameterException("ftr", innerException);
				}
			}
		}

		internal static void AddTargetTenantToHeader(string tenantName)
		{
			HttpContext.Current.Request.Headers.Add(RequestFilterModule.TargetTenantKey, tenantName);
		}

		public const string ExplicitLogonUserKey = "msExchEcpESOUser";

		public const char TargetTenantPrefix = '@';

		public const char OrganizationContextPrefix = '.';

		private const string PathRewrittenKey = "msExchPathRewritten";

		private const string DelegatedOrgParameter = "delegatedorg";

		private const string OrganizationContextParameter = "organizationcontext";

		private const string AllowDirectBERequestKey = "AllowDirectBERequest";

		private const string IsOSPEnvironmentKey = "IsOSPEnvironment";

		public static readonly string TargetTenantKey = "msExchTargetTenant";

		public static readonly string OrganizationContextKey = "msExchOrganizationContext";

		public static readonly string NoResolveIdKey = "msExchNoResolveId";

		public static readonly string OriginalUrlKey = "msExchOriginalUrl";

		private static readonly string appDomainAppVirtualPath = HttpRuntime.AppDomainAppVirtualPath ?? string.Empty;

		private static readonly int appDomainAppVirtualPathLength = RequestFilterModule.appDomainAppVirtualPath.Length;

		private static readonly Regex regex = new Regex("^" + RequestFilterModule.appDomainAppVirtualPath + "(/@                             # Target tenant or OrganizationContext start with '@' (TargetTenantPrefix)\r\n                 (?<isOrgContext>\\.)?           # OrganizationContext follow by '.' (OrganizationContextPrefix)\r\n                 (?<targetTenant>[^./][^/]+)    # follow by the domain name\r\n                )?                              # \r\n                (/                              #\r\n                 (?<esoAddress>[^@/]+@[^@/]+)   # ESO Email address contains one '@' in the middle\r\n                )?                              #\r\n                (?<closeSlash>/)?               # close slash. Without it, the request need 302 redirect\r\n                ", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);

		private static bool? blockNonCafeRequest;

		private static object blockNonCafeRequestLock = new object();
	}
}
