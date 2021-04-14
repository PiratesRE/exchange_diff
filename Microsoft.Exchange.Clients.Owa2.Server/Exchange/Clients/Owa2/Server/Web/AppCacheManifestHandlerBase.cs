using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Clients.Security;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal abstract class AppCacheManifestHandlerBase : IHttpHandler, IPageContext
	{
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public virtual UserAgent UserAgent { get; private set; }

		public CultureInfo UserCultureInfo
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture;
			}
		}

		public HttpContext Context { get; private set; }

		public HttpResponse Response
		{
			get
			{
				return this.Context.Response;
			}
		}

		public string Theme
		{
			get
			{
				return this.GetThemeFolder();
			}
		}

		public bool SessionDataEnabled
		{
			get
			{
				return true;
			}
		}

		public bool IsAppCacheEnabledClient
		{
			get
			{
				return true;
			}
		}

		public abstract HostNameController HostNameController { get; }

		protected virtual string ManifestTemplate
		{
			get
			{
				switch (this.UserAgent.Layout)
				{
				case LayoutType.TouchNarrow:
					return AppCacheManifestHandlerBase.NarrowManifestTemplate.Template;
				case LayoutType.TouchWide:
					return AppCacheManifestHandlerBase.WideManifestTemplate.Template;
				case LayoutType.Mouse:
					return AppCacheManifestHandlerBase.MouseManifestTemplate.Template;
				default:
					throw new InvalidOperationException("Unexpected layout " + this.UserAgent.Layout);
				}
			}
		}

		public virtual SlabManifestType ManifestType
		{
			get
			{
				if (!DefaultPageBase.IsPalEnabled(this.Context, this.UserAgent.RawString))
				{
					return SlabManifestType.Standard;
				}
				return SlabManifestType.Pal;
			}
		}

		protected abstract bool IsDatacenterNonDedicated { get; }

		protected abstract string ResourceDirectory { get; }

		protected abstract bool UseRootDirForAppCacheVdir { get; }

		protected virtual bool? IsRequestForOfflineManifest
		{
			get
			{
				return OfflineClientRequestUtilities.IsRequestForOfflineAppcacheManifest(this.Context.Request);
			}
		}

		public static byte[] CalculateHashOnHashes(List<byte[]> hashes)
		{
			int num = 0;
			for (int i = 0; i < hashes.Count; i++)
			{
				num += hashes[i].Length;
			}
			byte[] array = new byte[num];
			num = 0;
			for (int j = 0; j < hashes.Count; j++)
			{
				hashes[j].CopyTo(array, num);
				num += hashes[j].Length;
			}
			byte[] result;
			using (SHA1Cng sha1Cng = new SHA1Cng())
			{
				sha1Cng.Initialize();
				result = sha1Cng.ComputeHash(array);
			}
			return result;
		}

		public static bool DoesBrowserSupportAppCache(UserAgent userAgent)
		{
			return userAgent.IsBrowserChrome() || userAgent.IsBrowserSafari() || (userAgent.IsBrowserIE() && (userAgent.BrowserVersion.Build >= 10 || userAgent.GetTridentVersion() >= 6.0)) || (userAgent.IsBrowserFirefox() && userAgent.BrowserVersion.Build >= 23);
		}

		public string FormatURIForCDN(string relativeUri, bool isBootResourceUri)
		{
			return relativeUri;
		}

		public string GetCdnEndpointForResources(bool bootResources)
		{
			return string.Empty;
		}

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			this.Context = context;
			this.UserAgent = OwaUserAgentUtilities.CreateUserAgentWithLayoutOverride(context);
			ExTraceGlobals.AppcacheManifestHandlerTracer.TraceDebug<string, LayoutType>((long)this.GetHashCode(), "User {0} requested for {1} manifest ", AppCacheManifestHandler.GetUserContextId(context), this.UserAgent.Layout);
			bool flag = this.IsManifestRequest();
			this.SetResponseHeaders(flag);
			this.AddAppCacheVersionCookie();
			UserAgent userAgent = OwaUserAgentUtilities.CreateUserAgentWithLayoutOverride(context, null, true);
			UserAgent userAgent2 = OwaUserAgentUtilities.CreateUserAgentWithLayoutOverride(context);
			if ((!this.IsManifestLinkerRequest() && !flag) || (this.ShouldUnInstallAppCache() && flag) || (userAgent2.Layout == LayoutType.TouchNarrow && userAgent.Layout == LayoutType.TouchWide))
			{
				this.RemoveManifest();
				return;
			}
			if (flag && this.IsRealmRewrittenFromPathToQuery(this.Context))
			{
				this.RemoveManifest();
				return;
			}
			try
			{
				if (flag)
				{
					bool? isRequestForOfflineManifest = this.IsRequestForOfflineManifest;
					if (isRequestForOfflineManifest == null)
					{
						this.Response.StatusCode = 440;
					}
					else
					{
						this.AddIsClientAppCacheEnabledCookie();
						if (isRequestForOfflineManifest.Value)
						{
							this.WriteManifest(true);
						}
						else
						{
							this.WriteManifest(false);
						}
					}
				}
				else
				{
					this.WriteManifestLinker();
				}
			}
			catch (IOException)
			{
				this.Response.StatusCode = 500;
			}
		}

		protected abstract bool IsRealmRewrittenFromPathToQuery(HttpContext context);

		protected abstract string GetThemeFolder();

		protected abstract bool ShouldSkipThemeFolder();

		protected abstract string GetUserUPN();

		protected abstract bool ShouldAddDefaultMasterPageEntiresWithFlightDisabled();

		protected virtual bool IsMowaClient
		{
			get
			{
				return OfflineClientRequestUtilities.IsRequestFromMOWAClient(this.Context.Request, this.UserAgent.RawString);
			}
		}

		protected abstract bool IsHostNameSwitchFlightEnabled { get; }

		public abstract string VersionString { get; set; }

		protected virtual string GetLocalizedCultureNameForThemesResource(CultureInfo userCultureInfo)
		{
			return LocalizedThemeStyleResource.GetCultureDirectory(userCultureInfo);
		}

		protected virtual string GetLocalizedCultureNameForImagesResource(CultureInfo userCultureInfo)
		{
			return ThemeStyleResource.GetSpriteDirectory(userCultureInfo);
		}

		protected virtual string AddedQueryParameters()
		{
			return string.Empty;
		}

		protected virtual bool IsManifestRequest()
		{
			return this.Context.Request.QueryString["owamanifest"] == "1";
		}

		protected virtual bool IsManifestLinkerRequest()
		{
			return this.Context.Request.QueryString["manifest"] == "0";
		}

		protected bool ShouldUnInstallAppCache()
		{
			if (this.IsHostNameSwitchFlightEnabled && !this.HostNameController.IsUserAgentExcludedFromHostNameSwitchFlight(this.Context.Request))
			{
				Uri requestUrlEvenIfProxied = this.Context.Request.GetRequestUrlEvenIfProxied();
				string text;
				if (this.HostNameController.IsDeprecatedHostName(requestUrlEvenIfProxied.Host, out text))
				{
					return true;
				}
			}
			HttpCookie httpCookie = this.Context.Request.Cookies.Get("UnInstallAppcache");
			if (httpCookie == null || httpCookie.Value == null)
			{
				return false;
			}
			string value = httpCookie.Value;
			return value.ToLower() == true.ToString().ToLower();
		}

		protected virtual SlabManifestCollection SlabManifestCollection
		{
			get
			{
				return SlabManifestCollectionFactory.GetInstance(this.VersionString);
			}
		}

		protected abstract List<CultureInfo> GetSupportedCultures();

		protected virtual IEnumerable<string> GetScripts(bool generateBootResourcesAppcache)
		{
			bool isMowaClient = this.IsMowaClient;
			SlabManifestCollection slabManifestCollection = this.SlabManifestCollection;
			SlabManifestType slabManifestType = isMowaClient ? SlabManifestType.Pal : SlabManifestType.Standard;
			string[] enabledFeatures = this.GetEnabledFeatures();
			IEnumerable<string> enumerable = from source in slabManifestCollection.GetCodeScriptResources(SlabManifestType.PreBoot, this.UserAgent.Layout, enabledFeatures, false).Union(slabManifestCollection.GetCodeScriptResources(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache))
			select string.Format("../prem/{0}/scripts/{1}", this.VersionString, source);
			IEnumerable<string> source3 = from source in slabManifestCollection.GetLocalizedStringsScriptResources(SlabManifestType.PreBoot, this.UserAgent.Layout, enabledFeatures, false).Union(slabManifestCollection.GetLocalizedStringsScriptResources(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache))
			select source;
			if (source3.Any<string>())
			{
				LocalizedStringsScriptResource localizedStringsScriptResource = new LocalizedStringsScriptResource(source3.First<string>(), ResourceTarget.Any, this.VersionString);
				string cultureName = localizedStringsScriptResource.GetLocalizedCultureName();
				IEnumerable<string> second = from source in source3
				select string.Format("../prem/{0}/scripts/{1}/{2}", this.VersionString, cultureName.ToLowerInvariant(), source);
				enumerable = enumerable.Union(second);
			}
			IEnumerable<string> source2 = from source in slabManifestCollection.GetLocalizedExtStringsScriptResources(SlabManifestType.PreBoot, this.UserAgent.Layout, enabledFeatures, false).Union(slabManifestCollection.GetLocalizedExtStringsScriptResources(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache))
			select source;
			if (source2.Any<string>())
			{
				LocalizedExtensibilityStringsScriptResource localizedExtensibilityStringsScriptResource = new LocalizedExtensibilityStringsScriptResource(source2.First<string>(), ResourceTarget.Any, this.VersionString);
				string cultureName = localizedExtensibilityStringsScriptResource.GetLocalizedCultureName();
				IEnumerable<string> second2 = from source in source2
				select string.Format("../prem/{0}/scripts/ext/{1}/{2}", this.VersionString, cultureName.ToLowerInvariant(), source);
				enumerable = enumerable.Union(second2);
			}
			return enumerable;
		}

		protected virtual IEnumerable<string> GetThemedResources(CultureInfo userCultureInfo, bool generateBootResourcesAppcache)
		{
			bool isMowaClient = this.IsMowaClient;
			SlabManifestCollection slabManifestCollection = this.SlabManifestCollection;
			SlabManifestType slabManifestType = isMowaClient ? SlabManifestType.Pal : SlabManifestType.Standard;
			string[] enabledFeatures = this.GetEnabledFeatures();
			string theme = this.GetThemeFolder();
			string locale = this.GetLocalizedCultureNameForThemesResource(userCultureInfo);
			IEnumerable<string> first = this.ShouldSkipThemeFolder() ? (from style in slabManifestCollection.GetThemedStyles(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache)
			select string.Format("../prem/{0}/resources/styles/{1}/{2}", this.VersionString, locale, style)) : slabManifestCollection.GetThemedStyles(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache).Select((string style) => string.Format("../prem/{0}/resources/themes/{1}/{2}/{3}", new object[]
			{
				this.VersionString,
				theme,
				locale,
				style
			}));
			IEnumerable<string> second = from image in slabManifestCollection.GetThemedImages(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache)
			select string.Format("../prem/{0}/resources/themes/{1}/images/{2}/{3}", new object[]
			{
				this.VersionString,
				theme,
				DefaultPageBase.IsRtl ? "rtl" : "0",
				image
			});
			IEnumerable<string> second2 = this.ShouldSkipThemeFolder() ? (from image in slabManifestCollection.GetThemedSpriteStyles(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache)
			select string.Format("../prem/{0}/resources/images/{1}/{2}", this.VersionString, DefaultPageBase.IsRtl ? "rtl" : "0", image)) : slabManifestCollection.GetThemedSpriteStyles(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache).Select((string image) => string.Format("../prem/{0}/resources/themes/{1}/images/{2}/{3}", new object[]
			{
				this.VersionString,
				theme,
				DefaultPageBase.IsRtl ? "rtl" : "0",
				image
			}));
			return first.Union(second).Union(second2);
		}

		protected virtual IEnumerable<string> GetNonThemedResources(CultureInfo userCultureInfo, bool generateBootResourcesAppcache)
		{
			bool isMowaClient = this.IsMowaClient;
			SlabManifestCollection slabManifestCollection = this.SlabManifestCollection;
			SlabManifestType slabManifestType = isMowaClient ? SlabManifestType.Pal : SlabManifestType.Standard;
			string[] enabledFeatures = this.GetEnabledFeatures();
			this.GetLocalizedCultureNameForThemesResource(userCultureInfo);
			IEnumerable<string> first = from image in slabManifestCollection.GetNonThemedImages(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache)
			select string.Format("../prem/{0}/resources/images/{1}/{2}", this.VersionString, DefaultPageBase.IsRtl ? "rtl" : "0", image);
			IEnumerable<string> second = from font in slabManifestCollection.GetFonts(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache)
			select string.Format("../prem/{0}/resources/styles/{1}", this.VersionString, font);
			IEnumerable<string> second2 = from style in slabManifestCollection.GetNonThemedStyles(slabManifestType, this.UserAgent.Layout, enabledFeatures, generateBootResourcesAppcache)
			select string.Format("../prem/{0}/resources/styles/{1}", this.VersionString, style);
			return first.Union(second).Union(second2);
		}

		protected abstract string[] GetEnabledFeatures();

		private static byte[] GetHash(string fileName, string fileVersion)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"] + fileName);
				Tuple<string, string> key = Tuple.Create<string, string>(fileName, fileVersion);
				Tuple<DateTime, byte[]> tuple;
				if (!AppCacheManifestHandlerBase.hashMap.TryGetValue(key, out tuple))
				{
					tuple = Tuple.Create<DateTime, byte[]>(fileInfo.LastWriteTimeUtc, AppCacheManifestHandlerBase.CalculateFileHash(fileInfo));
				}
				else
				{
					if (!(tuple.Item1 != fileInfo.LastWriteTimeUtc))
					{
						return tuple.Item2;
					}
					tuple = Tuple.Create<DateTime, byte[]>(fileInfo.LastWriteTimeUtc, AppCacheManifestHandlerBase.CalculateFileHash(fileInfo));
				}
				Dictionary<Tuple<string, string>, Tuple<DateTime, byte[]>> dictionary = new Dictionary<Tuple<string, string>, Tuple<DateTime, byte[]>>(AppCacheManifestHandlerBase.hashMap);
				dictionary[key] = tuple;
				AppCacheManifestHandlerBase.hashMap = dictionary;
				return tuple.Item2;
			}
			catch (FileNotFoundException)
			{
			}
			return null;
		}

		private static byte[] CalculateFileHash(FileInfo fileInfo)
		{
			if (fileInfo.Exists)
			{
				using (SHA1Cng sha1Cng = new SHA1Cng())
				{
					using (FileStream fileStream = fileInfo.OpenRead())
					{
						sha1Cng.Initialize();
						return sha1Cng.ComputeHash(fileStream);
					}
				}
			}
			return new byte[0];
		}

		private void SetResponseHeaders(bool isManifestRequest)
		{
			if (isManifestRequest)
			{
				this.Response.ContentType = "text/cache-manifest";
			}
			else
			{
				this.Response.ContentType = "text/html";
			}
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			this.Response.CacheControl = "No-Cache";
			this.Response.Cache.SetNoServerCaching();
		}

		private void AddIsClientAppCacheEnabledCookie()
		{
			HttpCookie httpCookie = new HttpCookie("IsClientAppCacheEnabled");
			httpCookie.Value = true.ToString();
			this.Response.Cookies.Add(httpCookie);
		}

		private void AddAppCacheVersionCookie()
		{
			HttpCookie httpCookie = new HttpCookie("AppcacheVer");
			httpCookie.Value = this.VersionString + ":" + this.UserCultureInfo.Name.ToLowerInvariant() + this.Theme.ToLowerInvariant();
			this.Response.Cookies.Add(httpCookie);
		}

		private void WriteManifest(bool isOffline)
		{
			StringBuilder stringBuilder = new StringBuilder(this.ManifestTemplate);
			if (string.IsNullOrWhiteSpace(stringBuilder.ToString()))
			{
				string text = string.Format("User {0} request for {1} manifest fetched null or empty manifest", AppCacheManifestHandler.GetUserContextId(this.Context), this.UserAgent.Layout);
				ExTraceGlobals.AppcacheManifestHandlerTracer.TraceError((long)this.GetHashCode(), text);
				throw new ArgumentNullException(text);
			}
			bool generateBootResourcesAppcache = !isOffline;
			stringBuilder = this.AddTemplatedParameters(stringBuilder, generateBootResourcesAppcache);
			string text2 = stringBuilder.ToString();
			this.Response.Write(text2);
			string[] array = text2.Split(new char[]
			{
				'\r',
				'\n',
				' ',
				'\t'
			}, StringSplitOptions.RemoveEmptyEntries);
			string resourceDirectory = this.ResourceDirectory;
			List<byte[]> list = new List<byte[]>();
			foreach (string text3 in array)
			{
				if (text3.StartsWith(resourceDirectory))
				{
					list.Add(AppCacheManifestHandlerBase.GetHash(text3.Substring(3), this.VersionString));
				}
			}
			IOrderedEnumerable<string> orderedEnumerable = from id in this.GetEnabledFeatures()
			orderby id
			select id;
			foreach (string s in orderedEnumerable)
			{
				list.Add(Encoding.ASCII.GetBytes(s));
			}
			this.Response.Write("# ComputedHash: ");
			this.Response.Write(Convert.ToBase64String(AppCacheManifestHandlerBase.CalculateHashOnHashes(list)));
			this.Response.Write("\r\n");
			this.Response.Write("# Offline: ");
			this.Response.Write(isOffline.ToString().ToLower());
			this.Response.Write("\r\n");
		}

		private StringBuilder AddTemplatedParameters(StringBuilder manifest, bool generateBootResourcesAppcache)
		{
			CultureInfo userCultureInfo = this.UserCultureInfo;
			manifest = this.AddVersion(manifest);
			manifest = this.AddDefaultMasterPageUrls(manifest);
			manifest = this.AddUserCulture(manifest, userCultureInfo);
			manifest = this.AddUserRealm(manifest);
			manifest = this.AddUserResources(manifest, userCultureInfo, generateBootResourcesAppcache);
			manifest = this.AddVirtualDirectoryRoot(manifest);
			return manifest;
		}

		private StringBuilder AddVersion(StringBuilder manifest)
		{
			manifest = manifest.Replace("$OWA2_VER", this.VersionString);
			return manifest;
		}

		private StringBuilder AddDefaultMasterPageUrls(StringBuilder manifest)
		{
			string[] array = AppCacheManifestHandlerBase.ManifestFile.MasterPageManifestEntries;
			if (this.IsDatacenterNonDedicated)
			{
				array = array.Concat(AppCacheManifestHandlerBase.ManifestFile.DatacenterManifestEntries).ToArray<string>();
				if (!this.IsMowaClient)
				{
					array = array.Concat(AppCacheManifestHandlerBase.ManifestFile.DatacenterManifestEntriesForNonMowaClients).ToArray<string>();
				}
			}
			if (this.ShouldAddDefaultMasterPageEntiresWithFlightDisabled())
			{
				array = AppCacheManifestHandlerBase.ManifestFile.MasterPageManifestEntiresFlightsDisabled;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in array)
			{
				stringBuilder.AppendLine(value);
			}
			manifest.Replace("$Default_MasterPageUrls", stringBuilder.ToString());
			return manifest;
		}

		private StringBuilder AddUserCulture(StringBuilder manifest, CultureInfo userCultureInfo)
		{
			manifest = manifest.Replace("$USER_CULTURE", userCultureInfo.Name.ToLowerInvariant());
			return manifest;
		}

		private StringBuilder AddResourceList(StringBuilder manifest, IEnumerable<string> resources)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in resources)
			{
				stringBuilder.AppendLine(value);
			}
			manifest.Replace("$USER_RESOURCES", stringBuilder.ToString());
			return manifest;
		}

		private StringBuilder AddUserSpecificResourceInjector(StringBuilder manifest)
		{
			manifest = manifest.Replace("\r\nCACHE:\r\n", "\r\nCACHE:\r\n../" + string.Format("userspecificresourceinjector.ashx?ver={0}", this.VersionString).ToLowerInvariant() + "\r\n");
			return manifest;
		}

		private StringBuilder AddUserRealm(StringBuilder manifest)
		{
			manifest = manifest.Replace("$USER_REALM", HttpUtility.UrlEncode(this.GetUserRealm(this.GetUserUPN())));
			return manifest;
		}

		private StringBuilder AddUserResources(StringBuilder manifest, CultureInfo cultureInfo, bool generateBootResourcesAppcache)
		{
			IEnumerable<string> scripts = this.GetScripts(generateBootResourcesAppcache);
			IEnumerable<string> nonThemedResources = this.GetNonThemedResources(cultureInfo, generateBootResourcesAppcache);
			IEnumerable<string> themedResources = this.GetThemedResources(cultureInfo, generateBootResourcesAppcache);
			return this.AddResourceList(manifest, scripts.Union(themedResources).Union(nonThemedResources));
		}

		private void WriteManifestLinker()
		{
			this.Response.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
			this.Response.Write("<html manifest=\"appCacheManifestHandler.ashx?owamanifest=1");
			this.Response.Write("\"><head></head><body></body></html>");
		}

		private void RemoveManifest()
		{
			this.Response.StatusCode = 404;
			this.Response.TrySkipIisCustomErrors = true;
		}

		private StringBuilder AddVirtualDirectoryRoot(StringBuilder manifest)
		{
			string newValue = string.Empty;
			if (this.UseRootDirForAppCacheVdir)
			{
				newValue = "/";
			}
			else
			{
				string text = this.Context.Request.ApplicationPath ?? string.Empty;
				string text2 = text.TrimEnd(new char[]
				{
					'/'
				});
				newValue = text2;
			}
			if (Globals.IsPreCheckinApp)
			{
				string name = "X-DFPOWA-Vdir";
				if (this.Context.Request.Cookies[name] != null)
				{
					string value = this.Context.Request.Cookies[name].Value;
					string str = HttpUtility.UrlEncode(this.GetUserRealm(this.GetUserUPN()));
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("/");
					stringBuilder.AppendLine("../");
					stringBuilder.AppendLine("../microsoft.com");
					stringBuilder.AppendLine("../microsoft.com/");
					stringBuilder.AppendLine("../?vdir=" + value);
					stringBuilder.AppendLine("../?wa=wsignin1.0&vdir=" + value);
					stringBuilder.AppendLine("../?realm=" + str + "&vdir=" + value);
					stringBuilder.AppendLine("../?wa=wsignin1.0&?realm=" + str + "&vdir=" + value);
					stringBuilder.AppendLine("../?flights=none&vdir=" + value);
					stringBuilder.AppendLine("../?vdir=" + value + "&flights=none");
					return manifest.Replace("$ROOT", stringBuilder.ToString());
				}
			}
			return manifest.Replace("$ROOT", newValue);
		}

		private string GetUserRealm(string smtpAddress)
		{
			string text = string.Empty;
			int num = smtpAddress.IndexOf('@');
			if (num >= 0 && num < smtpAddress.Length - 1)
			{
				text = smtpAddress.Substring(num + 1);
			}
			return text.ToLowerInvariant();
		}

		public const string AppcacheHandlerName = "appcachemanifesthandler.ashx";

		public const string AppcacheVer = "AppcacheVer";

		public const string UnInstallAppcache = "UnInstallAppcache";

		public const string ManifestLinkerParameterName = "manifest";

		private const string VersionStr = "$OWA2_VER";

		private const string UserCultureStr = "$USER_CULTURE";

		private const string DefaultMasterPageUrlStr = "$Default_MasterPageUrls";

		private const string ResourceStr = "$USER_RESOURCES";

		private const string RootStr = "$ROOT";

		private const string UserRealmStr = "$USER_REALM";

		private const string OwaManifestParameterName = "owamanifest";

		private static AppCacheManifestHandlerBase.ManifestFile MouseManifestTemplate = new AppCacheManifestHandlerBase.ManifestFile(LayoutType.Mouse);

		private static AppCacheManifestHandlerBase.ManifestFile NarrowManifestTemplate = new AppCacheManifestHandlerBase.ManifestFile(LayoutType.TouchNarrow);

		private static AppCacheManifestHandlerBase.ManifestFile WideManifestTemplate = new AppCacheManifestHandlerBase.ManifestFile(LayoutType.TouchWide);

		private static Dictionary<Tuple<string, string>, Tuple<DateTime, byte[]>> hashMap = new Dictionary<Tuple<string, string>, Tuple<DateTime, byte[]>>();

		private class ManifestFile
		{
			public ManifestFile(LayoutType layout)
			{
				this.layout = layout;
			}

			public string Template
			{
				get
				{
					if (this.template == null)
					{
						StringBuilder builder = new StringBuilder();
						Array.ForEach<string>(AppCacheManifestHandlerBase.ManifestFile.AnyManifestEntries, delegate(string s)
						{
							builder.AppendLine(s);
						});
						switch (this.layout)
						{
						case LayoutType.TouchNarrow:
							Array.ForEach<string>(AppCacheManifestHandlerBase.ManifestFile.NarrowManifestEntries, delegate(string s)
							{
								builder.AppendLine(s);
							});
							break;
						case LayoutType.TouchWide:
							Array.ForEach<string>(AppCacheManifestHandlerBase.ManifestFile.WideManifestEntries, delegate(string s)
							{
								builder.AppendLine(s);
							});
							break;
						case LayoutType.Mouse:
							Array.ForEach<string>(AppCacheManifestHandlerBase.ManifestFile.MouseManifestEntries, delegate(string s)
							{
								builder.AppendLine(s);
							});
							break;
						}
						this.template = string.Format(AppCacheManifestHandlerBase.ManifestFile.ManifestTemplateFormat, new object[]
						{
							"$OWA2_VER",
							"$Default_MasterPageUrls",
							builder.ToString(),
							"$USER_RESOURCES"
						});
					}
					return this.template;
				}
			}

			public static readonly string[] MasterPageManifestEntries = new string[]
			{
				"$ROOT",
				"../",
				string.Format("../{0}", "$USER_REALM"),
				string.Format("../{0}/", "$USER_REALM"),
				string.Format("../?realm={0}", "$USER_REALM")
			};

			public static readonly string[] DatacenterManifestEntries = new string[]
			{
				"../?wa=wsignin1.0",
				string.Format("../?realm={0}&wa=wsignin1.0", "$USER_REALM")
			};

			public static readonly string[] DatacenterManifestEntriesForNonMowaClients = new string[]
			{
				string.Format("../?wa=wsignin1.0&realm={0}", "$USER_REALM")
			};

			public static readonly string[] MasterPageManifestEntiresFlightsDisabled = new string[]
			{
				"../?flights=none"
			};

			private static readonly string ManifestTemplateFormat = "CACHE MANIFEST\r\n# v. {0}\r\n\r\nCACHE:\r\n\r\n{1}{2}{3}\r\nNETWORK:\r\n*\r\n\r\n";

			private static readonly string[] AnyManifestEntries = new string[]
			{
				"../1x1.gif",
				string.Format("../prem/$OWA2_VER/scripts/globalize/globalize.culture.{0}.js", "$USER_CULTURE")
			};

			private static readonly string[] MouseManifestEntries = new string[]
			{
				"../?layout=mouse",
				"../popout.aspx",
				"../projection.aspx"
			};

			private static readonly string[] NarrowManifestEntries = new string[]
			{
				"../?layout=tnarrow"
			};

			private static readonly string[] WideManifestEntries = new string[]
			{
				"../?layout=twide"
			};

			private string template;

			private readonly LayoutType layout;
		}
	}
}
