using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class EcpUrl
	{
		public static string EcpVDir
		{
			get
			{
				return EcpUrl.ProcessUrl(EcpUrl.EcpVDirForStaticResource, true, EcpUrl.EcpVDirForStaticResource, false, false);
			}
		}

		public static string GetEcpVDirForCanary()
		{
			string text = EcpUrl.ProcessUrl(EcpUrl.EcpVDirForStaticResource, true, EcpUrl.EcpVDirForStaticResource, false, true);
			if (text.EndsWith("/"))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public static string OwaVDir
		{
			get
			{
				return EcpUrl.ProcessUrl("/owa/", true, "/owa/", false, false);
			}
		}

		public static string OwaCreateUnifiedGroup
		{
			get
			{
				return EcpUrl.ProcessUrl("/owa/?path=/mail/action/createmoderngroup", true, "/owa/", false, false);
			}
		}

		internal static bool IsEcpStandalone { get; private set; } = DatacenterRegistry.IsForefrontForOffice() && EcpUrl.GetLocalServer().IsFfoWebServiceRole;

		public static string GetOwaNavigateBackUrl()
		{
			string text = EcpUrl.OwaVDir;
			string owaNavigationParameter = HttpContext.Current.GetOwaNavigationParameter();
			if (!string.IsNullOrWhiteSpace(owaNavigationParameter))
			{
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(owaNavigationParameter);
				for (int i = 0; i < nameValueCollection.Count; i++)
				{
					text = EcpUrl.AppendFragmentParameter(text, nameValueCollection.Keys[i] ?? string.Empty, nameValueCollection[i]);
				}
			}
			return text;
		}

		internal static string AppendLanguageOverriddenParameter(string url)
		{
			return EcpUrl.AppendQueryParameter(url, "mkt", CultureInfo.CurrentUICulture.Name);
		}

		internal static string AppendFragmentParameter(string url, string name, string value)
		{
			return EcpUrl.AppendParameter(url, '#', name, value);
		}

		internal static string AppendQueryParameter(string url, string name, string value)
		{
			return EcpUrl.AppendParameter(url, '?', name, value);
		}

		internal static string AppendParameter(string url, char identifier, string name, string value)
		{
			StringBuilder stringBuilder = new StringBuilder(url, url.Length + name.Length + value.Length + 2);
			if (url.IndexOf(identifier) >= 0)
			{
				stringBuilder.Append("&");
			}
			else
			{
				stringBuilder.Append(identifier.ToString());
			}
			stringBuilder.Append(HttpUtility.UrlEncode(name));
			if (!string.IsNullOrEmpty(name))
			{
				stringBuilder.Append('=');
			}
			stringBuilder.Append(HttpUtility.UrlEncode(value));
			return stringBuilder.ToString();
		}

		internal static string RemoveQueryParameter(string url, string name, bool throwIfNotFound)
		{
			bool flag = false;
			int num = url.IndexOf('?');
			if (num > 0)
			{
				while (!flag && num < url.Length)
				{
					int num2 = url.IndexOf(name, num, StringComparison.InvariantCultureIgnoreCase);
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
			if (throwIfNotFound && !flag)
			{
				throw new InvalidOperationException(string.Format("The url '{0}' doesn't contain parameter '{1}'.", url, name));
			}
			return url;
		}

		internal static string GetLeftPart(string url, UriPartial part)
		{
			Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
			if (uri.IsAbsoluteUri)
			{
				return uri.GetLeftPart(part);
			}
			switch (part)
			{
			case UriPartial.Scheme:
			case UriPartial.Authority:
				throw new InvalidOperationException();
			case UriPartial.Path:
			{
				int num = url.IndexOf('?');
				if (num < 0)
				{
					return url;
				}
				return url.Substring(0, num);
			}
			default:
				return url;
			}
		}

		public static string ProcessUrl(string url)
		{
			return EcpUrl.ProcessUrl(url, true);
		}

		public static string ProcessUrl(string url, bool isFullPath)
		{
			return EcpUrl.ProcessUrl(url, isFullPath, EcpUrl.EcpVDirForStaticResource, false, false);
		}

		internal static string ProcessUrl(string url, bool isFullPath, string vdir, bool skipTargetTenant, bool skipEso = false)
		{
			HttpContext context = HttpContext.Current;
			StringBuilder stringBuilder = null;
			bool flag = false;
			if (!skipTargetTenant)
			{
				flag |= (EcpUrl.AppendDelegatedTenantIfAny(context, url, ref stringBuilder, isFullPath, vdir) || EcpUrl.AppendOrganizationContextIfAny(context, url, ref stringBuilder, isFullPath, vdir));
			}
			if (!skipEso)
			{
				flag |= EcpUrl.AppendEsoUserIfAny(context, url, ref stringBuilder, isFullPath, vdir);
			}
			if (flag)
			{
				int length = EcpUrl.EcpVDirForStaticResource.Length;
				stringBuilder.Append(url, length, url.Length - length);
				url = stringBuilder.ToString();
			}
			return url;
		}

		private static bool AppendDelegatedTenantIfAny(HttpContext context, string url, ref StringBuilder sb, bool isFullPath, string vdir)
		{
			bool result = false;
			string targetTenant = context.GetTargetTenant();
			if (!string.IsNullOrEmpty(targetTenant) && !context.HasOrganizationContext())
			{
				EcpUrl.EnsureStringBuilderInitialized(ref sb, isFullPath, vdir);
				sb.Append('@');
				sb.Append(targetTenant);
				sb.Append('/');
				result = true;
			}
			return result;
		}

		private static bool AppendOrganizationContextIfAny(HttpContext context, string url, ref StringBuilder sb, bool isFullPath, string vdir)
		{
			bool result = false;
			string targetTenant = context.GetTargetTenant();
			if (!string.IsNullOrEmpty(targetTenant) && context.HasOrganizationContext())
			{
				EcpUrl.EnsureStringBuilderInitialized(ref sb, isFullPath, vdir);
				sb.Append('@');
				sb.Append('.');
				sb.Append(targetTenant);
				sb.Append('/');
				result = true;
			}
			return result;
		}

		private static bool AppendEsoUserIfAny(HttpContext context, string url, ref StringBuilder sb, bool isFullPath, string vdir)
		{
			bool result = false;
			string explicitUser = context.GetExplicitUser();
			if (!string.IsNullOrEmpty(explicitUser))
			{
				EcpUrl.EnsureStringBuilderInitialized(ref sb, isFullPath, vdir);
				sb.Append(explicitUser);
				sb.Append('/');
				result = true;
			}
			return result;
		}

		private static void EnsureStringBuilderInitialized(ref StringBuilder sb, bool isFullPath, string vdir)
		{
			if (sb == null)
			{
				sb = new StringBuilder();
				if (isFullPath)
				{
					sb.Append(vdir);
				}
			}
		}

		[Conditional("DEBUG")]
		private static void ThrowIfAlreadyContainsPrefix(string url, object prefix, params object[] morePrefixes)
		{
			if (url.Contains("/" + prefix + morePrefixes.StringArrayJoin(string.Empty)))
			{
				throw new InvalidOperationException("The url already contains the path prefix. Probably this method has been called twice. Please fix it.");
			}
		}

		[Conditional("DEBUG")]
		private static void ThrowIfFullURLNotStartWithECPVdir(string url, bool isFullPath, string vdir)
		{
			if (isFullPath && !url.StartsWith(vdir, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("The url isn't start with " + vdir + " as expected: " + url, "url");
			}
		}

		public static string ToEscapedString(this Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (!uri.IsAbsoluteUri)
			{
				return uri.ToString();
			}
			return uri.AbsoluteUri;
		}

		public static string ResolveClientUrl(string url)
		{
			return EcpUrl.ResolveClientUrl(new Uri(url, UriKind.RelativeOrAbsolute)).ToString();
		}

		public static Uri ResolveClientUrl(Uri url)
		{
			UriBuilder uriBuilder = null;
			if (url != null && url.IsAbsoluteUri)
			{
				uriBuilder = new UriBuilder(url);
				if (uriBuilder.Scheme == "http" && EcpUrl.SslOffloaded)
				{
					uriBuilder.Scheme = "https";
				}
			}
			if (HttpContext.Current.IsExplicitSignOn() || HttpContext.Current.HasTargetTenant())
			{
				uriBuilder = (uriBuilder ?? new UriBuilder(url));
				uriBuilder.Path = EcpUrl.ProcessUrl(uriBuilder.Path, true);
			}
			if (uriBuilder != null)
			{
				url = uriBuilder.Uri;
			}
			return url;
		}

		private static bool IsSslOffloaded()
		{
			bool result = false;
			if (Util.IsDataCenter)
			{
				bool.TryParse(ConfigurationManager.AppSettings["LiveIdAuthModuleSslOffloadedKey"], out result);
			}
			else
			{
				result = Registry.SslOffloaded;
			}
			return result;
		}

		private static Server GetLocalServer()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 634, "GetLocalServer", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\Utilities\\EcpUrl.cs");
			topologyConfigurationSession.UseConfigNC = true;
			topologyConfigurationSession.UseGlobalCatalog = true;
			Server server = topologyConfigurationSession.FindLocalServer();
			if (server == null)
			{
				ExTraceGlobals.ProxyTracer.TraceInformation(0, 0L, "Could not find local server in directory.");
				throw new CmdletAccessDeniedException(Strings.FailedToGetLocalServerInfo);
			}
			return server;
		}

		public static bool HasScheme(string virtualPath)
		{
			int num = virtualPath.IndexOf(':');
			if (num == -1)
			{
				return false;
			}
			int num2 = virtualPath.IndexOf('/');
			return num2 == -1 || num < num2;
		}

		public static bool IsRooted(string basepath)
		{
			return string.IsNullOrEmpty(basepath) || basepath[0] == '/' || basepath[0] == '\\';
		}

		public static bool IsRelativeUrl(string virtualPath)
		{
			return !EcpUrl.HasScheme(virtualPath) && !EcpUrl.IsRooted(virtualPath);
		}

		public static bool IsAppRelativePath(string path)
		{
			if (path == null)
			{
				return false;
			}
			int length = path.Length;
			return length != 0 && path[0] == '~' && (length == 1 || path[1] == '\\' || path[1] == '/');
		}

		public static string GetRelativePathToAppRoot(string url)
		{
			int num;
			int num2;
			if (!EcpUrl.GetRelativePathToAppRootRange(url, out num, out num2))
			{
				return null;
			}
			return url.Substring(num, num2 - num);
		}

		public static string ReplaceRelativePath(string url, string newRelativePath, bool throwIfFail)
		{
			int length;
			int num;
			if (EcpUrl.GetRelativePathToAppRootRange(url, out length, out num))
			{
				return url.Substring(0, length) + newRelativePath + url.Substring(num, url.Length - num);
			}
			if (throwIfFail)
			{
				throw new ArgumentException("Can not determine relative path", "url");
			}
			return null;
		}

		private static bool GetRelativePathToAppRootRange(string url, out int start, out int end)
		{
			start = -1;
			end = -1;
			if (EcpUrl.HasScheme(url))
			{
				return false;
			}
			if (EcpUrl.IsAppRelativePath(url))
			{
				start = 2;
			}
			else if (EcpUrl.IsRelativeUrl(url))
			{
				start = 0;
			}
			else if (EcpUrl.IsRooted(url) && url.StartsWith(EcpUrl.EcpVDirForStaticResource))
			{
				start = EcpUrl.EcpVDirForStaticResource.Length;
			}
			if (start == -1)
			{
				return false;
			}
			end = url.IndexOf('?');
			if (end == -1)
			{
				end = url.Length;
			}
			return start < end;
		}

		public const string CopyrightLink = "http://go.microsoft.com/fwlink/p/?LinkId=256676";

		public const string OwaVDirForStaticResource = "/owa/";

		internal const string LogoffAndReturnPath = "logoff.aspx?src=exch&ru=";

		public const string DDIServicePath = "DDI/DDIService.svc?schema=";

		public static readonly string EcpVDirForStaticResource = HttpRuntime.AppDomainAppVirtualPath + '/';

		public static readonly bool SslOffloaded = EcpUrl.IsSslOffloaded();

		public class QueryParameter
		{
			public const string ID = "id";

			public const string EWS_ID = "ewsid";

			public const string TplNames = "tplNames";

			public const string Recipient = "recip";

			public const string MessageId = "MsgId";

			public const string Mailbox = "Mbx";

			public const string Cause = "cause";

			public const string IsOwa = "isowa";

			public const string SubscriptionType = "st";

			public const string SubscriptionGuid = "su";

			public const string SharedSecretParam = "ss";

			public const string Command = "cmd";

			public const string Feature = "ftr";

			public const string RedirectionURL = "backUr";

			public const string SelectionMode = "mode";

			public const string MessageTypes = "types";

			public const string Cross = "cross";

			public const string XPremiseServer = "xprs";

			public const string XPremiseVersion = "xprv";

			public const string XPremiseFeatures = "xprf";

			public const string ShowHelp = "showhelp";

			public const string LanguageOverriddenParam = "mkt";

			public const string LanguageOverriddenCandidateParam = "mkt2";

			public const string AllowTypingParam = "allowtyping";

			public const string ViewParam = "vw";

			public const string ReferrerParam = "rfr";

			public const string TopNavParam = "topnav";

			public const string StartPageParam = "p";

			public const string OnPremiseStartPageParam = "op";

			public const string CloudStartPageParam = "cp";

			public const string OnPremiseVisibleParam = "ov";

			public const string QueryStringParam = "q";

			public const string BreadCrumbParam = "bc";

			public const string FolderIdParam = "fldID";

			public const string DialPlanID = "dialPlanId";

			public const string New = "new";

			public const string DeviceType = "dt";

			public const string HelpId = "helpid";

			public const string IsNarrowPage = "isNarrow";

			public const string AssetID = "AssetID";

			public const string Etoken = "ClientToken=";

			public const string QueryMarket = "LC";

			public const string Scope = "Scope";

			public const string DeploymentId = "DeployId";

			public const string CallingApplication = "app";

			public const string DataTransferMode = "dtm";

			public const string Provider = "Provider";

			public const string SchemaName = "sn";

			public const string RequestId = "reqId";

			public const string RetentionTagTypeGroup = "typeGroup";

			public const string RequestVersion = "ExchClientVer";

			public const string RequestTargetServer = "TargetServer";

			public const string SimplifiedChangePhotoExperience = "chgPhoto";
		}
	}
}
