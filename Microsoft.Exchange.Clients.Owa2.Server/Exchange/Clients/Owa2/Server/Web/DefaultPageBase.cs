using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Core;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public abstract class DefaultPageBase : Page, IPageContext
	{
		public virtual ResourceBase[] UserDataEmbeddedLinks
		{
			get
			{
				if (this.userDataEmbededLinks == null)
				{
					this.userDataEmbededLinks = UserResourcesFinder.GetUserDataEmbeddedLinks(this.GetBootSlab(), this.VersionString);
				}
				return this.userDataEmbededLinks;
			}
		}

		public static bool IsRtl
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft;
			}
		}

		public new static string UICulture
		{
			get
			{
				return Thread.CurrentThread.CurrentCulture.Name;
			}
		}

		public string UserLanguage
		{
			get
			{
				foreach (ResourceBase resourceBase in this.UserDataEmbeddedLinks)
				{
					LocalizedStringsScriptResource localizedStringsScriptResource = resourceBase as LocalizedStringsScriptResource;
					if (localizedStringsScriptResource != null)
					{
						return localizedStringsScriptResource.GetLocalizedCultureName();
					}
				}
				return null;
			}
		}

		public static string PayloadClassName
		{
			get
			{
				return "PageDataPayload";
			}
		}

		public static string ThemeStyleCultureDirectory
		{
			get
			{
				return LocalizedThemeStyleResource.GetCultureDirectory(Thread.CurrentThread.CurrentUICulture);
			}
		}

		public virtual UserAgent UserAgent { get; private set; }

		public abstract string SlabsManifest { get; }

		public abstract string TenantId { get; }

		public abstract string MdbGuid { get; }

		public bool IsMowaAlt2Session
		{
			get
			{
				return OfflineClientRequestUtilities.IsRequestForAppCachedVersion(this.Context);
			}
		}

		public bool IsClientInOfflineMode
		{
			get
			{
				bool? flag = OfflineClientRequestUtilities.IsRequestForOfflineAppcacheManifest(base.Request);
				return OfflineClientRequestUtilities.IsRequestFromOfflineClient(base.Request) || (flag != null && flag.Value);
			}
		}

		public virtual string ServerSettings
		{
			get
			{
				string text;
				if (Globals.IsPreCheckinApp)
				{
					text = "/owa";
				}
				else
				{
					text = HttpRuntime.AppDomainAppVirtualPath;
				}
				return string.Concat(new string[]
				{
					"'version': '",
					this.VersionString,
					"','startTime': st,'bootedFromAppcache': appCachedPage,'cdnEndpoint': '",
					this.GetCdnEndpointForResources(false),
					"','mapControlUrl': '",
					Default.MapControlUrl,
					"','appDomainAppVirtualPath': '",
					text,
					"','layout': layout,'uiCulture': userCultureVar,'uiLang': userLanguageVar,'userCultureRtl': userCultureRtl,'uiTheme': clientTheme,'osfStringPath': '",
					this.GetLocalizedOsfStringResourcePath(false),
					"','scriptsFolder': '",
					this.GetScriptsFolder(false),
					"','resourcesFolder': '",
					this.GetResourcesFolder(false),
					"','featuresSupported': ",
					this.GetFeaturesSupportedJsonArray(FlightedFeatureScope.Client | FlightedFeatureScope.ClientServer),
					",'themedImagesFolderFormat': '",
					this.GetThemedImagesFolderFormat(false),
					"','stylesFolderFormat': '",
					this.GetStylesFolderFormat(false),
					"','stylesLocale': stylesLocale"
				});
			}
		}

		public virtual bool SessionDataEnabled
		{
			get
			{
				return true;
			}
		}

		public new string Theme
		{
			get
			{
				if (string.IsNullOrEmpty(this.theme))
				{
					this.theme = this.GetThemeFolder();
				}
				return this.theme;
			}
		}

		public virtual string IsChangeLayoutFeatureEnabled
		{
			get
			{
				return "true";
			}
		}

		public virtual bool CompositeSessionData { get; protected set; }

		public virtual bool RecoveryBoot
		{
			get
			{
				return DefaultPageBase.IsRecoveryBoot(this.Context);
			}
		}

		public bool IsAppCacheEnabledClient { get; protected set; }

		public string AppCacheClientQueryParamWithValue
		{
			get
			{
				return string.Format("{0}={1}", "appcacheclient", this.IsAppCacheEnabledClient ? "1" : "0");
			}
		}

		public bool IsOfflineAppCacheEnabledClient { get; protected set; }

		public string EncodingErrorMessage
		{
			get
			{
				return Strings.ErrorWrongEncoding;
			}
		}

		public string LogOffOwaUrl
		{
			get
			{
				return OwaUrl.LogoffOwa.GetExplicitUrl(this.Context.Request);
			}
		}

		public virtual SlabManifestType ManifestType
		{
			get
			{
				if (this.slabManifestType == null)
				{
					this.slabManifestType = (DefaultPageBase.IsPalEnabled(this.Context, this.UserAgent.RawString) ? SlabManifestType.Pal : SlabManifestType.Standard);
				}
				return this.slabManifestType;
			}
			protected set
			{
				this.slabManifestType = value;
			}
		}

		public string UserSpecificResourcesHash
		{
			get
			{
				return UserResourcesFinder.GetResourcesHash(this.UserDataEmbeddedLinks, this, true, this.VersionString);
			}
		}

		protected string BootScriptsFolder
		{
			get
			{
				return this.GetScriptsFolder(true);
			}
		}

		protected string BootResourcesFolder
		{
			get
			{
				return this.GetResourcesFolder(true);
			}
		}

		protected string BootStylesFolder
		{
			get
			{
				return this.GetStylesFolder(true);
			}
		}

		protected string BootImagesFolder
		{
			get
			{
				return this.GetImagesFolder(true);
			}
		}

		protected string BootThemedImagesFolderFormat
		{
			get
			{
				return this.GetThemedImagesFolderFormat(true);
			}
		}

		protected string BootThemedStylesFolder
		{
			get
			{
				return string.Format(this.BootThemedImagesFolderFormat, this.Theme);
			}
		}

		protected string BootThemedImagesFolder
		{
			get
			{
				return string.Format(this.FormatURIForCDN(ResourcePathBuilderUtilities.GetThemedLocaleImageResourcesRelativeFolderPath(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString), DefaultPageBase.IsRtl), true), this.Theme);
			}
		}

		protected string BootStylesFolderFormat
		{
			get
			{
				return this.GetStylesFolderFormat(true);
			}
		}

		protected string CdnVersionCheckUrl
		{
			get
			{
				return this.GetScriptsFolder(false) + "/cdnversioncheck.js";
			}
		}

		protected string LogoImageFileName
		{
			get
			{
				switch (this.UserAgent.Layout)
				{
				case LayoutType.TouchNarrow:
					return "owa_logo.narrow.png";
				case LayoutType.TouchWide:
					return "owa_logo.wide.png";
				case LayoutType.Mouse:
					return "owa_logo.mouse.png";
				default:
					return null;
				}
			}
		}

		protected string BootTraceUrl
		{
			get
			{
				string text = ConfigurationManager.AppSettings["BootTraceUrl"];
				return text ?? string.Empty;
			}
		}

		protected bool ShouldLoadSegoeFonts
		{
			get
			{
				return DefaultPageBase.GetFontLocale() == "0" && !this.UserAgent.IsOsWindowsNtVersionOrLater(6, 1);
			}
		}

		protected bool ShouldLoadSegoeSemilight
		{
			get
			{
				return DefaultPageBase.GetFontLocale() == "0" && !this.UserAgent.IsOsWindows8OrLater();
			}
		}

		internal LocalizedExtensibilityStringsScriptResource LocalizedOsfStringResource
		{
			get
			{
				if (this.localizedOsfStringResource == null)
				{
					this.localizedOsfStringResource = new LocalizedExtensibilityStringsScriptResource("osfruntime_strings.js", ResourceTarget.Any, this.VersionString);
				}
				return this.localizedOsfStringResource;
			}
		}

		public abstract string FormatURIForCDN(string relativeUri, bool isBootResourceUri);

		public abstract string GetCdnEndpointForResources(bool bootResources);

		public abstract string VersionString { get; set; }

		public static bool IsRecoveryBoot(HttpContext context)
		{
			bool result = false;
			if (context != null)
			{
				result = context.Request.QueryString.AllKeys.Contains("bO");
			}
			return result;
		}

		public static bool IsPalEnabled(HttpContext context)
		{
			return DefaultPageBase.IsPalEnabled(context, context.Request.UserAgent);
		}

		public static bool IsPalEnabled(HttpContext context, string userAgent)
		{
			return OfflineClientRequestUtilities.IsRequestFromMOWAClient(context.Request, userAgent);
		}

		protected void WriteUserSpecificScripts()
		{
			UserSpecificResourceInjectorBase.WriteScriptBlock(new Action<string>(base.Response.Write), this, UserContextManager.GetUserContext(HttpContext.Current), this.UserDataEmbeddedLinks, this.VersionString);
		}

		protected override void OnPreInit(EventArgs e)
		{
			this.UserAgent = OwaUserAgentUtilities.CreateUserAgentWithLayoutOverride(this.Context);
			this.IsAppCacheEnabledClient = this.GetIsClientAppCacheEnabled(this.Context);
			this.IsOfflineAppCacheEnabledClient = (this.IsAppCacheEnabledClient && this.IsClientInOfflineMode);
			this.CompositeSessionData = this.CalculateCompositeSessionDataEnabled();
			base.Response.AddHeader("pragma", "no-cache");
			base.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			base.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
			if (UrlUtilities.IsAuthRedirectRequest(this.Context.Request) || AppCacheManifestHandlerBase.DoesBrowserSupportAppCache(this.UserAgent))
			{
				string url = null;
				if (this.ShouldRedirectWithoutUnnecessaryParams(out url))
				{
					base.Response.Redirect(url);
				}
			}
		}

		protected virtual string GetThemeFolder()
		{
			return "base";
		}

		protected abstract bool ShouldSkipThemeFolder();

		protected virtual bool ShouldRedirectWithoutUnnecessaryParams(out string destinationUrl)
		{
			destinationUrl = null;
			string uriString = this.Context.Request.Url.OriginalString;
			string text = this.Context.Request.Headers["msExchProxyUri"];
			if (!string.IsNullOrEmpty(text))
			{
				uriString = text;
			}
			Uri uri = new Uri(uriString);
			NameValueCollection coll = HttpUtility.ParseQueryString(uri.Query);
			string str = null;
			if (DefaultPageBase.QueryHasUnnecessaryParameters(coll, out str))
			{
				string text2 = uri.AbsolutePath;
				if (text2.ToLowerInvariant().EndsWith("default.aspx"))
				{
					text2 = text2.Substring(0, text2.Length - "default.aspx".Length);
				}
				text2 += str;
				destinationUrl = text2;
				return true;
			}
			return false;
		}

		protected abstract string GetFeaturesSupportedJsonArray(FlightedFeatureScope scope);

		private static bool QueryHasUnnecessaryParameters(NameValueCollection coll, out string query)
		{
			query = null;
			bool result = false;
			if (coll.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = null;
				string text2 = null;
				for (int i = 0; i < coll.Keys.Count; i++)
				{
					string text3 = coll.Keys[i];
					string value = coll.Keys[i].ToLowerInvariant();
					if (Array.IndexOf<string>(DefaultPageBase.ParametersToRemove, value) >= 0)
					{
						result = true;
					}
					else if ("path".Equals(value))
					{
						text2 = coll[text3];
						result = true;
					}
					else if ("modurl".Equals(value))
					{
						text = coll[text3];
						result = true;
					}
					else
					{
						if (stringBuilder.Length == 0)
						{
							stringBuilder.Append('?');
						}
						else
						{
							stringBuilder.Append('&');
						}
						stringBuilder.Append(Uri.EscapeDataString(text3));
						if (!string.IsNullOrEmpty(coll[text3]))
						{
							stringBuilder.Append('=');
							stringBuilder.Append(Uri.EscapeDataString(coll[text3]));
						}
					}
				}
				if (!string.IsNullOrWhiteSpace(text2))
				{
					stringBuilder.Append(string.Format("#{0}={1}", "path", Uri.EscapeDataString(text2)));
					result = true;
				}
				else if (!string.IsNullOrWhiteSpace(text))
				{
					stringBuilder.Append(string.Format("#{0}={1}", "modurl", Uri.EscapeDataString(text)));
					result = true;
				}
				if (stringBuilder.Length > 0)
				{
					query = stringBuilder.ToString();
				}
			}
			return result;
		}

		protected virtual bool GetIsClientAppCacheEnabled(HttpContext context)
		{
			bool flag = false;
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(context.Request.Url.Query);
			foreach (string text in nameValueCollection.AllKeys)
			{
				string text2;
				if (text == null || !DefaultPageBase.ParamsInAppCache.TryGetValue(text.ToLowerInvariant(), out text2) || (text2 != null && text2 != context.Request.Params[text]))
				{
					flag = true;
					break;
				}
			}
			return (!flag && OfflineClientRequestUtilities.IsRequestForAppCachedVersion(context)) || DefaultPageBase.IsPalEnabled(context, this.UserAgent.RawString);
		}

		protected string InlineJavascript(string fileName)
		{
			string text = Path.Combine(FolderConfiguration.Instance.RootPath, ResourcePathBuilderUtilities.GetScriptResourcesRelativeFolderPath(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString)), fileName);
			DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
			Tuple<string, DateTime> tuple;
			lock (DefaultPageBase.inlineScripts)
			{
				if (!DefaultPageBase.inlineScripts.TryGetValue(text, out tuple) || tuple.Item2 < lastWriteTimeUtc)
				{
					tuple = Tuple.Create<string, DateTime>(File.ReadAllText(text), lastWriteTimeUtc);
					DefaultPageBase.inlineScripts[text] = tuple;
				}
			}
			return tuple.Item1;
		}

		protected string InlineImage(string fileName)
		{
			string text = Path.Combine(Path.Combine(FolderConfiguration.Instance.RootPath, ResourcePathBuilderUtilities.GetBootImageResourcesRelativeFolderPath(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString), DefaultPageBase.IsRtl)), fileName);
			DateTime lastWriteTimeUtc;
			try
			{
				lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
			}
			catch
			{
				return this.BootImagesFolder + "/" + fileName;
			}
			Tuple<string, DateTime> tuple;
			lock (DefaultPageBase.inlineImages)
			{
				if (!DefaultPageBase.inlineImages.TryGetValue(text, out tuple) || tuple.Item2 < lastWriteTimeUtc)
				{
					tuple = Tuple.Create<string, DateTime>("data:image/" + Path.GetExtension(fileName).Substring(1) + ";base64," + Convert.ToBase64String(File.ReadAllBytes(text)), lastWriteTimeUtc);
					DefaultPageBase.inlineImages[text] = tuple;
				}
			}
			return tuple.Item1;
		}

		protected string InlineFontCss()
		{
			string fontLocale = DefaultPageBase.GetFontLocale();
			return this.InlineStyles(string.Format("fabric.font.{0}.css", fontLocale));
		}

		protected string InlineFabricCss()
		{
			return this.InlineStyles(string.Format("fabric.color.theme.{0}.css", this.Theme));
		}

		protected string InlineStyles(string filename)
		{
			string text = Path.Combine(FolderConfiguration.Instance.RootPath, ResourcePathBuilderUtilities.GetStyleResourcesRelativeFolderPath(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString)), filename);
			Tuple<string, DateTime> tuple;
			lock (DefaultPageBase.inlineStyles)
			{
				DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
				if (!DefaultPageBase.inlineStyles.TryGetValue(text, out tuple) || tuple.Item2 < lastWriteTimeUtc)
				{
					tuple = Tuple.Create<string, DateTime>("<style>" + File.ReadAllText(text) + "</style>", lastWriteTimeUtc);
					DefaultPageBase.inlineStyles[text] = tuple;
				}
			}
			return tuple.Item1;
		}

		protected virtual IEnumerable<string> GetBootScripts()
		{
			Slab slab = this.GetBootSlab();
			return from s in slab.PackagedSources
			select s.Name;
		}

		protected Slab GetBootSlab()
		{
			if (this.bootSlab == null)
			{
				this.bootSlab = UserResourcesFinder.CreateUserBootSlab(this.ManifestType, this.UserAgent.Layout, this.VersionString);
			}
			return this.bootSlab;
		}

		private static string GetFontLocale()
		{
			string cultureDirectory = LocalizedThemeStyleResource.GetCultureDirectory(Thread.CurrentThread.CurrentUICulture);
			if (!(cultureDirectory == "rtl"))
			{
				return cultureDirectory;
			}
			return "0";
		}

		private string GetLayoutSuffix()
		{
			string result;
			switch (this.UserAgent.Layout)
			{
			case LayoutType.TouchNarrow:
				result = "narrow";
				break;
			case LayoutType.TouchWide:
				result = "wide";
				break;
			case LayoutType.Mouse:
				result = "mouse";
				break;
			default:
				result = "mouse";
				break;
			}
			return result;
		}

		protected string GetScriptsFolder(bool bootScriptsFolder)
		{
			return this.FormatURIForCDN(ResourcePathBuilderUtilities.GetScriptResourcesRelativeFolderPath(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString)), bootScriptsFolder);
		}

		protected string GetResourcesFolder(bool bootResourcesFolder)
		{
			return this.FormatURIForCDN(ResourcePathBuilderUtilities.GetBootResourcesRelativeFolderPath(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString)), bootResourcesFolder);
		}

		protected string GetStylesFolder(bool bootStylesFolder)
		{
			return this.FormatURIForCDN(ResourcePathBuilderUtilities.GetStyleResourcesRelativeFolderPath(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString)), bootStylesFolder);
		}

		protected string GetImagesFolder(bool bootImagesfolder)
		{
			return this.FormatURIForCDN(ResourcePathBuilderUtilities.GetBootImageResourcesRelativeFolderPath(ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString), DefaultPageBase.IsRtl), bootImagesfolder);
		}

		protected string GetThemedImagesFolderFormat(bool bootThemedImagesFolder)
		{
			return this.FormatURIForCDN(ResourcePathBuilderUtilities.GetBootThemeImageResourcesRelativeFolderPath(this.VersionString, ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString), DefaultPageBase.IsRtl, this.ShouldSkipThemeFolder()), bootThemedImagesFolder);
		}

		protected string GetStylesFolderFormat(bool bootStylesFolderFormat)
		{
			return this.FormatURIForCDN(ResourcePathBuilderUtilities.GetBootStyleResourcesRelativeFolderPath(this.VersionString, ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.VersionString), "#LCL", this.ShouldSkipThemeFolder()), bootStylesFolderFormat);
		}

		protected string FormatResourceString(string link)
		{
			return link.ToLowerInvariant();
		}

		protected virtual string GetLocalizedOsfStringResourcePath(bool isBootResourcePath)
		{
			return this.LocalizedOsfStringResource.GetResourcePath(this, false);
		}

		protected virtual bool CalculateCompositeSessionDataEnabled()
		{
			bool result = false;
			if (this.IsAppCacheEnabledClient && !this.IsClientInOfflineMode && !DefaultPageBase.IsRecoveryBoot(this.Context))
			{
				UserContext userContext = UserContextManager.GetUserContext(this.Context);
				result = (userContext != null && userContext.FeaturesManager != null && userContext.FeaturesManager.ServerSettings.OwaCompositeSessionData.Enabled);
			}
			return result;
		}

		public const string CompositeResourceRequestQueryParam = "crr";

		public const string CompositeResourceRequestRetryQueryParam = "crrretry";

		public const string RetryCompositeResourceRequest = "RetryCrrRequest";

		public const string AppCacheClientQueryParam = "appcacheclient";

		public const string AppCacheEnabledClientValue = "1";

		public const string AppCacheDisabledClientValue = "0";

		public const string ExternalDropScriptResources = "extDropScriptResources";

		public const string ChangeLayoutFeatureEnabledVar = "changeLayoutEnabled";

		public const string LayoutVar = "layout";

		public const string StylesLocaleVar = "stylesLocale";

		public const string AppCachedPageVar = "appCachedPage";

		public const string ClientServerFeaturesVar = "featuresVar";

		public const string UserSpecificResourcesHashVar = "userSpecificResourcesHashVar";

		public const string UserLanguageVar = "userLanguageVar";

		public const string UserCultureVar = "userCultureVar";

		public const string UserCultureRtlVar = "userCultureRtl";

		public const string ScriptResourcesVar = "scriptResources";

		public const string UserScriptResourcesVar = "userScriptResources";

		public const string StyleSheetResourcesVar = "styleResources";

		public const string ServerVersionVar = "sver";

		public const string LastClientVersionVar = "lcver";

		public const string ThemeVar = "clientTheme";

		public const string StylesFolderCulturePlaceHolder = "#LCL";

		public const string LocalizedExtStringResourceName = "osfruntime_strings.js";

		public const string UserSpecificResourceInjectorJs = "userspecificresourceinjector.ashx?ver={0}";

		public const string LanguageReplacementMarker = "##LANG##";

		public const string CultureReplacementMarker = "##CULTURE##";

		public const string AuthParamName = "authRedirect";

		public const string ClientExistingVersionParam = "cver";

		public const string VersionParam = "ver";

		public const string FeaturesParam = "cf";

		public const string VersionChangeParam = "vC";

		public const string AppCacheParam = "appcache";

		public const string BootOnlineParam = "bO";

		public const string StartLoadTimeVarName = "st";

		public const string LayoutParameterName = "layout";

		public const string DefaultPageVersion = "3";

		private const string PathParameter = "path";

		private const string ModurlParameter = "modurl";

		internal static readonly Dictionary<string, string> ParamsInAppCache = new Dictionary<string, string>
		{
			{
				"appcache",
				"true"
			},
			{
				"realm",
				null
			},
			{
				"layout",
				null
			},
			{
				"wa",
				"wsignin1.0"
			},
			{
				"palenabled",
				"1"
			}
		};

		private static readonly string[] ParametersToRemove = new string[]
		{
			"authredirect",
			"ll-cc",
			"exsvurl",
			"source",
			"cbcxt",
			"lc",
			"exch",
			"delegatedorg",
			"ae",
			"slusng",
			"id",
			"src",
			"to",
			"type",
			"vd"
		};

		private static Dictionary<string, Tuple<string, DateTime>> inlineScripts = new Dictionary<string, Tuple<string, DateTime>>();

		private static Dictionary<string, Tuple<string, DateTime>> inlineImages = new Dictionary<string, Tuple<string, DateTime>>();

		private static Dictionary<string, Tuple<string, DateTime>> inlineStyles = new Dictionary<string, Tuple<string, DateTime>>();

		private ResourceBase[] userDataEmbededLinks;

		private string theme;

		private Slab bootSlab;

		private SlabManifestType slabManifestType;

		private LocalizedExtensibilityStringsScriptResource localizedOsfStringResource;
	}
}
