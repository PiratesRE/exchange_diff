using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public abstract class OptionsBase
	{
		public OptionsBase(OwaContext owaContext, TextWriter writer)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.request = owaContext.HttpContext.Request;
			this.userContext = owaContext.UserContext;
			this.writer = writer;
			this.command = Utilities.GetFormParameter(this.request, "hidcmdpst", false);
		}

		public abstract void Render();

		public abstract void RenderScript();

		public bool ShowInfoBar
		{
			get
			{
				return !string.IsNullOrEmpty(this.commitStatus);
			}
		}

		public InfobarMessageType InfobarMessageType
		{
			get
			{
				return this.infobarMessageType;
			}
		}

		protected string Command
		{
			get
			{
				return this.command;
			}
		}

		protected void SetSavedSuccessfully(bool savedSuccessfully)
		{
			if (savedSuccessfully)
			{
				this.SetInfobarMessage(LocalizedStrings.GetNonEncoded(191284072), InfobarMessageType.Informational);
				return;
			}
			this.SetInfobarMessage(LocalizedStrings.GetNonEncoded(-1203841103), InfobarMessageType.Error);
		}

		protected void SetInfobarMessage(string message, InfobarMessageType type)
		{
			this.commitStatus = message;
			this.infobarMessageType = type;
		}

		protected void RenderJSVariable(string varName, string value)
		{
			if (string.IsNullOrEmpty(varName))
			{
				throw new ArgumentException("varName can not be null or empty string");
			}
			this.writer.Write("var ");
			this.writer.Write(varName);
			this.writer.Write(" = ");
			this.writer.Write(value);
			this.writer.Write(";");
		}

		protected void RenderJSVariable(string varName, bool value)
		{
			if (string.IsNullOrEmpty(varName))
			{
				throw new ArgumentException("varName can not be null or empty string");
			}
			this.writer.Write("var ");
			this.writer.Write(varName);
			this.writer.Write(" = ");
			this.writer.Write(value ? "true" : "false");
			this.writer.Write(";");
		}

		protected void RenderJSVariableWithQuotes(string varName, string value)
		{
			if (string.IsNullOrEmpty(varName))
			{
				throw new ArgumentException("varName can not be null or empty string");
			}
			this.writer.Write("var ");
			this.writer.Write(varName);
			this.writer.Write(" = \"");
			this.writer.Write(Utilities.JavascriptEncode(value));
			this.writer.Write("\";");
		}

		protected void RenderHeaderRow(ThemeFileId themeFileId, Strings.IDs headerStringId)
		{
			this.RenderHeaderRow(themeFileId, headerStringId, 1);
		}

		protected void RenderHeaderRow(ThemeFileId themeFileId, Strings.IDs headerStringId, int colspan)
		{
			this.writer.Write("<tr><td");
			if (colspan > 1)
			{
				this.writer.Write(" colspan=");
				this.writer.Write(colspan);
			}
			this.writer.Write(" class=\"hdr\"><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, themeFileId);
			this.writer.Write("\" alt=\"\"><h1 class=\"bld\">");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(headerStringId));
			this.writer.Write("</h1></td></tr>");
		}

		public void RenderInfobar()
		{
			if (this.ShowInfoBar)
			{
				InfobarMessage infobarMessage = InfobarMessage.CreateText(this.commitStatus, this.infobarMessageType);
				infobarMessage.IsActionResult = true;
				this.infobar.AddMessage(infobarMessage);
				this.infobar.Render(this.writer);
			}
		}

		private const string CommandParameter = "hidcmdpst";

		private string commitStatus;

		protected UserContext userContext;

		protected TextWriter writer;

		protected HttpRequest request;

		protected Infobar infobar = new Infobar();

		private InfobarMessageType infobarMessageType = InfobarMessageType.Informational;

		private string command;
	}
}
