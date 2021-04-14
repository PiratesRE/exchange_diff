using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class OwaPage : Page
	{
		public OwaPage()
		{
			this.owaContext = OwaContext.Current;
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
				return OwaConfigurationManager.Configuration.IsSMimeEnabledOnCurrentServerr;
			}
		}

		public OwaPage(bool setNoCacheNoStore)
		{
			this.setNoCacheNoStore = setNoCacheNoStore;
			this.owaContext = OwaContext.Current;
		}

		public TextWriter SanitizingResponse
		{
			get
			{
				return this.owaContext.SanitizingResponseWriter;
			}
		}

		public virtual string OwaVersion
		{
			get
			{
				return Globals.ApplicationVersion;
			}
		}

		protected OwaContext OwaContext
		{
			get
			{
				return this.owaContext;
			}
		}

		protected UserContext UserContext
		{
			get
			{
				return this.owaContext.UserContext;
			}
		}

		protected ISessionContext SessionContext
		{
			get
			{
				return this.owaContext.SessionContext;
			}
		}

		protected bool IsDownLevelClient
		{
			get
			{
				if (this.isDownLevelClient == -1)
				{
					this.isDownLevelClient = (Utilities.IsDownLevelClient(base.Request) ? 1 : 0);
				}
				return this.isDownLevelClient == 1;
			}
		}

		public string Identity
		{
			get
			{
				return base.GetType().BaseType.Name;
			}
		}

		protected string ExchangePrincipalDisplayName
		{
			get
			{
				return Utilities.GetMailboxOwnerDisplayName(this.owaContext.UserContext.MailboxSession);
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

		protected void RenderIdentity()
		{
			base.Response.Output.Write("<input type=hidden name=\"");
			base.Response.Output.Write("hidpid");
			base.Response.Output.Write("\" value=\"");
			Utilities.HtmlEncode(this.Identity, base.Response.Output);
			base.Response.Output.Write("\">");
		}

		protected bool IsPostFromMyself()
		{
			return this.IsPostFromPage(this.Identity);
		}

		protected bool IsPostFromPage<T>() where T : OwaPage
		{
			return this.IsPostFromPage(typeof(T).Name);
		}

		private bool IsPostFromPage(string pageIdentity)
		{
			if (Utilities.IsPostRequest(base.Request))
			{
				string formParameter = Utilities.GetFormParameter(base.Request, "hidpid", false);
				if (formParameter != null && string.CompareOrdinal(formParameter, pageIdentity) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public override void ProcessRequest(HttpContext context)
		{
			try
			{
				base.ProcessRequest(context);
			}
			catch (ThreadAbortException)
			{
				OwaContext.Get(context).UnlockMinResourcesOnCriticalError();
			}
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
			Utilities.HtmlEncode(Globals.CopyrightMessage, base.Response.Output);
			base.Response.Write(" -->");
			base.Response.Write("\n<!-- OwaPage = ");
			Utilities.HtmlEncode(base.GetType().ToString(), base.Response.Output);
			base.Response.Write(" -->\n");
		}

		protected override void OnInit(EventArgs e)
		{
			Microsoft.Exchange.Clients.Owa.Core.Culture.SetThreadCulture(this.owaContext);
			if (this.setNoCacheNoStore)
			{
				Utilities.MakePageNoCacheNoStore(base.Response);
			}
			this.EnableViewState = false;
			base.OnInit(e);
			if (!this.owaContext.IsAsyncRequest)
			{
				this.owaContext.IsAsyncRequest = base.AsyncMode;
			}
		}

		protected override void OnError(EventArgs e)
		{
			Exception lastError = base.Server.GetLastError();
			base.Server.ClearError();
			Utilities.HandleException(this.OwaContext, lastError, true);
		}

		protected void RenderScriptHandler(string eventName, string handlerCode)
		{
			this.RenderScriptHandler(eventName, handlerCode, false);
		}

		protected void RenderScriptHandler(string eventName, string handlerCode, bool returnFalse)
		{
			Utilities.RenderScriptHandler(base.Response.Output, eventName, handlerCode, returnFalse);
		}

		public void RenderOnClick(string handlerCode)
		{
			this.RenderOnClick(handlerCode, false);
		}

		public void RenderOnClick(string handlerCode, bool returnFalse)
		{
			Utilities.RenderScriptHandler(this.SanitizingResponse, "onclick", handlerCode, returnFalse);
		}

		protected void RenderInlineScripts()
		{
			Utilities.RenderScriptTagStart(base.Response.Output);
			Utilities.RenderInlineScripts(base.Response.Output, this.SessionContext);
			Utilities.RenderScriptTagEnd(base.Response.Output);
		}

		protected void RenderExternalScripts(ScriptFlags scriptFlags, IEnumerable<string> fileNames)
		{
			Utilities.RenderExternalScripts(base.Response.Output, scriptFlags, fileNames);
		}

		protected void RenderExternalScripts(ScriptFlags scriptFlags, params string[] fileNames)
		{
			Utilities.RenderExternalScripts(base.Response.Output, scriptFlags, (IEnumerable<string>)fileNames);
		}

		protected void RenderScripts(params string[] fileNames)
		{
			Utilities.RenderScripts(this.SanitizingResponse, this.SessionContext, ScriptFlags.IncludeUglobal, fileNames);
		}

		protected bool IsSilverlightEnabled
		{
			get
			{
				return this.UserContext.IsFeatureEnabled(Feature.Silverlight);
			}
		}

		protected void RenderSilverlight(string rootVisualType, string id, int width, int height, string alternateHtml)
		{
			SanitizedHtmlString value = SanitizedHtmlString.Format("<div style=\"width:{1}px;height:{2}px;\"><object \r\n                id=\"{0}\" \r\n                data=\"data:application/x-silverlight-2,\"\r\n                type=\"application/x-silverlight-2\"\r\n                width=\"{1}\" \r\n                height=\"{2}\"\r\n                ><param name=\"source\" value=\"/OWA/{3}/ClientBin/{4}.xap\"/>\r\n                <param name=\"initParams\" value=\"RootVisualType={5}.{6}\"/>\r\n                <param name=\"onError\" value=\"{7}\" />\r\n                <param name=\"minRuntimeVersion\" value=\"{8}\" />\r\n                <param name=\"autoUpgrade\" value=\"true\" />{9}</object></div>", new object[]
			{
				id,
				width,
				height,
				Globals.ApplicationVersion,
				"OwaSl",
				"Microsoft.Exchange.Clients.Owa.Silverlight",
				rootVisualType,
				"SL_OnPluginError",
				"2.0.31005.0",
				alternateHtml
			});
			base.Response.Output.Write(value);
		}

		protected void RenderSegmentationBitsForJavascript()
		{
			uint[] segmentationBitsForJavascript = this.UserContext.SegmentationBitsForJavascript;
			this.SanitizingResponse.Write("[");
			this.SanitizingResponse.Write(segmentationBitsForJavascript[0]);
			this.SanitizingResponse.Write(", ");
			this.SanitizingResponse.Write(segmentationBitsForJavascript[1]);
			this.SanitizingResponse.Write("]");
		}

		protected void RenderEndOfFileDiv()
		{
			this.SanitizingResponse.Write("<div id=divEOF style=display:none></div>");
		}

		protected string GetSaveAttachmentToDiskMessage(Strings.IDs attachmentMessageId)
		{
			Strings.IDs localizedId;
			if (this.SessionContext.BrowserType == BrowserType.IE)
			{
				localizedId = 1297545050;
			}
			else if (this.SessionContext.BrowserType == BrowserType.Safari)
			{
				localizedId = 175065296;
			}
			else
			{
				localizedId = -1815684119;
			}
			return string.Format(LocalizedStrings.GetNonEncoded(attachmentMessageId), LocalizedStrings.GetNonEncoded(localizedId));
		}

		protected const string SilverlightXapName = "OwaSl";

		protected const string SilverlightRootNamespace = "Microsoft.Exchange.Clients.Owa.Silverlight";

		protected const string SilverlightPluginErrorHandler = "SL_OnPluginError";

		private const string PageIdentityHiddenName = "hidpid";

		private bool setNoCacheNoStore = true;

		private OwaContext owaContext;

		private int isDownLevelClient = -1;
	}
}
