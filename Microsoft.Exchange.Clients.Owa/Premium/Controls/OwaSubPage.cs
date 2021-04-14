using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public abstract class OwaSubPage : UserControl
	{
		public OwaSubPage()
		{
			this.owaContext = OwaContext.Current;
			this.IsStandalonePage = false;
			this.IsInOEHResponse = false;
		}

		public QueryStringParameters QueryStringParameters { get; set; }

		public TextWriter SanitizingResponse
		{
			get
			{
				return this.owaContext.SanitizingResponseWriter;
			}
		}

		protected static bool IsRtl
		{
			get
			{
				return Culture.IsRtl;
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

		protected override void OnPreRender(EventArgs e)
		{
			this.SanitizingResponse.Write("\n<!-- OwaSubPage = ");
			Utilities.HtmlEncode(base.GetType().ToString(), base.Response.Output);
			this.SanitizingResponse.Write(" -->\n");
		}

		protected void RenderInlineScripts()
		{
			Utilities.RenderInlineScripts(base.Response.Output, this.UserContext);
		}

		public void RenderExternalScriptFiles()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.RenderExternalScriptFiles(stringBuilder);
			base.Response.Output.Write(stringBuilder.ToString());
		}

		public void RenderExternalScriptFiles(StringBuilder builder)
		{
			builder.Append("<div id=\"divExternalScriptFiles\" class=\"h\">");
			builder.Append("<div>");
			Utilities.HtmlEncode(Utilities.GetScriptFullPath("uglobal.js"), builder);
			builder.Append("</div>");
			foreach (string fileName in this.ExternalScriptFilesIncludeChildSubPages)
			{
				builder.Append("<div>");
				Utilities.HtmlEncode(Utilities.GetScriptFullPath(fileName), builder);
				builder.Append("</div>");
			}
			builder.Append("</div>");
		}

		protected void RenderVariableDeclarationStart()
		{
			if (this.IsStandalonePage)
			{
				this.SanitizingResponse.Write("<script  type=\"text/javascript\">");
			}
			else
			{
				this.SanitizingResponse.Write("<script type=\"text/javascript\" id=\"divVariableDeclarations\">if(0){");
			}
			this.SanitizingResponse.Write("createGlobalVariables = function $createGlobalVariables(oPage){");
		}

		protected void RenderVariableDeclarationEnd()
		{
			if (this.IsStandalonePage)
			{
				this.SanitizingResponse.Write("};</script>");
				return;
			}
			this.SanitizingResponse.Write("};}</script>");
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

		internal bool IsStandalonePage { get; set; }

		internal bool IsInOEHResponse { get; set; }

		protected string GetParameter(string name, bool isParameterRequired)
		{
			if (this.QueryStringParameters == null)
			{
				return Utilities.GetQueryStringParameter(HttpContext.Current.Request, name, isParameterRequired);
			}
			string value = this.QueryStringParameters.GetValue(name);
			if (value == null && isParameterRequired)
			{
				throw new OwaInvalidRequestException(string.Format("Required URL parameter missing: {0}", name));
			}
			return value;
		}

		protected List<OwaSubPage> ChildSubPages
		{
			get
			{
				if (this.childSubPages == null)
				{
					this.childSubPages = new List<OwaSubPage>();
				}
				return this.childSubPages;
			}
		}

		public abstract IEnumerable<string> ExternalScriptFiles { get; }

		public IEnumerable<string> ExternalScriptFilesIncludeChildSubPages
		{
			get
			{
				foreach (string scriptFile in this.ExternalScriptFiles)
				{
					yield return scriptFile;
				}
				foreach (OwaSubPage owaSubPage in this.ChildSubPages)
				{
					foreach (string scriptFile2 in owaSubPage.ExternalScriptFilesIncludeChildSubPages)
					{
						yield return scriptFile2;
					}
				}
				yield break;
			}
		}

		public abstract SanitizedHtmlString Title { get; }

		public abstract string PageType { get; }

		public virtual bool SupportIM
		{
			get
			{
				return true;
			}
		}

		public virtual string BodyCssClass
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string HtmlAdditionalAttributes
		{
			get
			{
				return string.Empty;
			}
		}

		private OwaContext owaContext;

		private List<OwaSubPage> childSubPages;
	}
}
