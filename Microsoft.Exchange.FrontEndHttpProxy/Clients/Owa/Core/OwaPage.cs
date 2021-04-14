using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Web.UI;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.HttpProxy;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class OwaPage : Page
	{
		public OwaPage()
		{
		}

		public OwaPage(bool setNoCacheNoStore)
		{
			this.setNoCacheNoStore = setNoCacheNoStore;
		}

		public UserAgent UserAgent
		{
			get
			{
				if (this.userAgent == null)
				{
					UserAgent userAgent = new UserAgent(base.Request.UserAgent, false, base.Request.Cookies);
					if (base.Request.QueryString != null)
					{
						string text = base.Request.QueryString["layout"];
						if (text != null)
						{
							userAgent.SetLayoutFromString(text);
						}
						else
						{
							string text2 = base.Request.QueryString["url"];
							if (text2 != null)
							{
								int num = text2.IndexOf('?');
								if (num >= 0 && num < text2.Length - 1)
								{
									string query = text2.Substring(num + 1);
									NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(query);
									text = nameValueCollection["layout"];
									if (text != null)
									{
										userAgent.SetLayoutFromString(text);
									}
								}
							}
						}
					}
					this.userAgent = userAgent;
				}
				return this.userAgent;
			}
		}

		public string Identity
		{
			get
			{
				return base.GetType().BaseType.Name;
			}
		}

		protected static bool IsRtl
		{
			get
			{
				return Microsoft.Exchange.Clients.Owa.Core.Culture.IsRtl;
			}
		}

		protected static bool SMimeEnabledPerServer
		{
			get
			{
				return false;
			}
		}

		protected bool IsDownLevelClient
		{
			get
			{
				if (this.isDownLevelClient == -1)
				{
					this.isDownLevelClient = (base.Request.IsDownLevelClient() ? 1 : 0);
				}
				return this.isDownLevelClient == 1;
			}
		}

		protected virtual bool UseStrictMode
		{
			get
			{
				return true;
			}
		}

		protected virtual bool HasFrameset
		{
			get
			{
				return false;
			}
		}

		protected virtual bool IsTextHtml
		{
			get
			{
				return true;
			}
		}

		public static bool IsPalEnabled(HttpContext context)
		{
			if (context.Request != null && context.Request.Cookies != null && context.Request.Cookies["PALEnabled"] != null)
			{
				return context.Request.Cookies["PALEnabled"].Value != "-1";
			}
			return context.Request.QueryString["palenabled"] == "1" || (context.Request.UserAgent != null && context.Request.UserAgent.Contains("MSAppHost"));
		}

		public string GetDefaultCultureCssFontFileName()
		{
			CultureInfo userCulture = Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture();
			return Microsoft.Exchange.Clients.Owa.Core.Culture.GetCssFontFileNameFromCulture(userCulture);
		}

		protected override void InitializeCulture()
		{
			CultureInfo cultureInfo = Microsoft.Exchange.Clients.Owa.Core.Culture.GetBrowserDefaultCulture(base.Request);
			if (cultureInfo == null && OwaVdirConfiguration.Instance.LogonAndErrorLanguage > 0)
			{
				try
				{
					cultureInfo = CultureInfo.GetCultureInfo(OwaVdirConfiguration.Instance.LogonAndErrorLanguage);
				}
				catch (CultureNotFoundException)
				{
					cultureInfo = null;
				}
			}
			if (cultureInfo != null)
			{
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
				Thread.CurrentThread.CurrentCulture = cultureInfo;
			}
			base.InitializeCulture();
		}

		protected void RenderIdentity()
		{
			base.Response.Output.Write("<input type=hidden name=\"");
			base.Response.Output.Write("hidpid");
			base.Response.Output.Write("\" value=\"");
			EncodingUtilities.HtmlEncode(this.Identity, base.Response.Output);
			base.Response.Output.Write("\">");
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (this.HasFrameset)
			{
				base.Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\">");
				base.Response.Write("\n");
			}
			else if (this.UseStrictMode && this.IsTextHtml)
			{
				base.Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">");
				base.Response.Write("\n");
			}
			else if (this.IsTextHtml)
			{
				base.Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
				base.Response.Write("\n");
			}
			base.Response.Write("<!-- ");
			EncodingUtilities.HtmlEncode(OwaPage.CopyrightMessage, base.Response.Output);
			base.Response.Write(" -->");
			base.Response.Write("\n<!-- OwaPage = ");
			EncodingUtilities.HtmlEncode(base.GetType().ToString(), base.Response.Output);
			base.Response.Write(" -->\n");
		}

		protected override void OnInit(EventArgs e)
		{
			if (this.setNoCacheNoStore)
			{
				AspNetHelper.MakePageNoCacheNoStore(base.Response);
			}
			this.EnableViewState = false;
			base.OnInit(e);
		}

		protected string GetNoScriptHtml()
		{
			string htmlEncoded = LocalizedStrings.GetHtmlEncoded(719849305);
			return string.Format(htmlEncoded, "<a href=\"http://www.microsoft.com/windows/ie/downloads/default.mspx\">", "</a>");
		}

		protected string InlineJavascript(string fileName)
		{
			return this.InlineResource(fileName, "scripts\\premium\\", (string fullFilePath) => "<script>" + File.ReadAllText(fullFilePath) + "</script>", OwaPage.inlineScripts);
		}

		protected string InlineImage(ThemeFileId themeFileId)
		{
			string fileName = ThemeFileList.GetNameFromId(themeFileId);
			return this.InlineResource(fileName, "themes\\resources", (string fullFilePath) => "data:" + MimeMapping.GetMimeMapping(fileName) + ";base64," + Convert.ToBase64String(File.ReadAllBytes(fullFilePath)), OwaPage.inlineImages);
		}

		protected string InlineCss(ThemeFileId themeFileId)
		{
			string nameFromId = ThemeFileList.GetNameFromId(themeFileId);
			return this.InlineCss(nameFromId);
		}

		protected string InlineCss(string fileName)
		{
			return this.InlineResource(fileName, "themes\\resources", (string fullFilePath) => "<style>" + File.ReadAllText(fullFilePath) + "</style>", OwaPage.inlineStyles);
		}

		private string InlineResource(string fileName, string partialFileLocation, OwaPage.ResoruceCreator createResource, Dictionary<string, Tuple<string, DateTime>> resourceDictionary)
		{
			string text = HttpRuntime.AppDomainAppPath.ToLower();
			if (text.EndsWith("ecp\\"))
			{
				text = text.Replace("ecp\\", "owa\\");
			}
			string text2 = Path.Combine(text, "auth\\" + ProxyApplication.ApplicationVersion, partialFileLocation, fileName);
			DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text2);
			Tuple<string, DateTime> tuple;
			lock (resourceDictionary)
			{
				if (!resourceDictionary.TryGetValue(text2, out tuple) || tuple.Item2 < lastWriteTimeUtc)
				{
					tuple = Tuple.Create<string, DateTime>(createResource(text2), lastWriteTimeUtc);
					resourceDictionary[text2] = tuple;
				}
			}
			return tuple.Item1;
		}

		protected const string SilverlightXapName = "OwaSl";

		protected const string SilverlightRootNamespace = "Microsoft.Exchange.Clients.Owa.Silverlight";

		protected const string SilverlightPluginErrorHandler = "SL_OnPluginError";

		protected const string PALEnabledCookieName = "PALEnabled";

		protected const string LoadFailedCookieName = "loadFailed";

		private const string PageIdentityHiddenName = "hidpid";

		private const string LayoutParamName = "layout";

		private const string ScriptsPath = "scripts\\premium\\";

		private const string ResourcesPath = "themes\\resources";

		private const string OwaVDir = "owa\\";

		private const string EcpVDir = "ecp\\";

		public static readonly string CopyrightMessage = "Copyright (c) 2011 Microsoft Corporation.  All rights reserved.";

		public static readonly string SupportedBrowserHelpUrl = "http://office.com/redir/HA102824601.aspx";

		private static Dictionary<string, Tuple<string, DateTime>> inlineScripts = new Dictionary<string, Tuple<string, DateTime>>();

		private static Dictionary<string, Tuple<string, DateTime>> inlineImages = new Dictionary<string, Tuple<string, DateTime>>();

		private static Dictionary<string, Tuple<string, DateTime>> inlineStyles = new Dictionary<string, Tuple<string, DateTime>>();

		private bool setNoCacheNoStore = true;

		private int isDownLevelClient = -1;

		private UserAgent userAgent;

		private delegate string ResoruceCreator(string fullFileName);
	}
}
