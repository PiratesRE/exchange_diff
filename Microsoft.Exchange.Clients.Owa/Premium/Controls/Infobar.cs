using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class Infobar
	{
		public Infobar()
		{
			this.sessionContext = OwaContext.Current.SessionContext;
		}

		public Infobar(string divErrorId, string barClass) : this()
		{
			this.divErrorId = divErrorId;
			this.barClass = barClass;
		}

		public int MessageCount
		{
			get
			{
				return this.messages.Count;
			}
		}

		public void AddMessage(InfobarMessage infobarMessage)
		{
			this.messages.Add(infobarMessage);
		}

		public void AddMessage(Strings.IDs messageString, InfobarMessageType type)
		{
			this.AddMessage(SanitizedHtmlString.FromStringId(messageString), type);
		}

		public void AddMessage(SanitizedHtmlString messageHtml, InfobarMessageType type)
		{
			this.messages.Add(new InfobarMessage(messageHtml, type));
		}

		public void AddMessage(SanitizedHtmlString messageHtml, InfobarMessageType type, string tagId)
		{
			this.messages.Add(new InfobarMessage(messageHtml, type, tagId));
		}

		public void AddMessage(SanitizedHtmlString messageHtml, InfobarMessageType type, string tagId, bool hideMessage)
		{
			this.messages.Add(new InfobarMessage(messageHtml, type, tagId, hideMessage));
		}

		public void AddMessage(SanitizedHtmlString messageHtml, InfobarMessageType type, SanitizedHtmlString linkText, SanitizedHtmlString expandSection)
		{
			this.messages.Add(new InfobarMessage(messageHtml, type, null, linkText, expandSection));
		}

		public void AddMessage(SanitizedHtmlString messageHtml, InfobarMessageType type, string tagId, SanitizedHtmlString linkText, SanitizedHtmlString expandSection)
		{
			this.messages.Add(new InfobarMessage(messageHtml, type, tagId, linkText, expandSection));
		}

		public static void RenderMessage(TextWriter output, InfobarMessage infobarMessage, ISessionContext sessionContext)
		{
			if (infobarMessage == null)
			{
				throw new ArgumentNullException("infobarMessage");
			}
			Infobar.RenderMessage(output, infobarMessage.Type, infobarMessage.Message, infobarMessage.TagId, false, sessionContext);
		}

		public static void RenderMessage(TextWriter output, InfobarMessageType messageType, SanitizedHtmlString messageHtml, ISessionContext sessionContext)
		{
			Infobar.RenderMessage(output, messageType, messageHtml, null, false, sessionContext);
		}

		public static void RenderMessage(TextWriter output, InfobarMessageType messageType, SanitizedHtmlString messageHtml, string messageId, bool hideMessage, ISessionContext sessionContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (messageHtml == null)
			{
				throw new ArgumentNullException("messageHtml");
			}
			Infobar.RenderMessageIdAndClass(output, messageType, messageId, sessionContext);
			output.Write("\"");
			if (hideMessage)
			{
				output.Write(" style=\"display:none\" ");
				output.Write("isVisible");
				output.Write("=0");
			}
			else
			{
				output.Write(" ");
				output.Write("isVisible");
				output.Write("=1");
			}
			output.Write(">");
			output.Write(sessionContext.IsRtl ? "<div class=\"fltRight\">" : "<div class=\"fltLeft\">");
			sessionContext.RenderThemeImage(output, ThemeFileId.Dash, sessionContext.IsRtl ? "rtl dashImg" : "dashImg", new object[]
			{
				"id=imgDash"
			});
			output.Write("</div>");
			output.Write(messageHtml);
			output.Write("</div>");
		}

		public void Render(TextWriter output)
		{
			this.Render(output, true);
		}

		public void Render(TextWriter output, bool isEditable)
		{
			this.Render(output, isEditable, false);
		}

		public void Render(TextWriter output, bool isEditable, bool renderHidden)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=\"divInfobar\"");
			output.Write(" class=\"");
			output.Write(this.barClass);
			if (this.sessionContext.IsRtl)
			{
				output.Write(" rtl");
			}
			output.Write("\"");
			if (this.ShouldHideInfobar(renderHidden))
			{
				output.Write(" style=\"display:none\"");
			}
			output.Write(">");
			output.Write("<div id=\"divInfobarColor\"");
			output.Write(" class=\"");
			if (Infobar.HasHighSeverityMessages(this.messages))
			{
				output.Write("highSeverity");
			}
			else if (0 < this.messages.Count)
			{
				output.Write("lowSeverity");
			}
			output.Write("\"></div>");
			output.Write("<div id=\"divIB\">");
			RenderingUtilities.RenderErrorInfobar(this.sessionContext, output, this.divErrorId);
			Infobar.InfobarMessageComparer comparer = new Infobar.InfobarMessageComparer();
			this.messages.Sort(comparer);
			InfobarMessageType infobarMessageType = InfobarMessageType.Maximum;
			foreach (object obj in this.messages)
			{
				InfobarMessage infobarMessage = (InfobarMessage)obj;
				InfobarMessageType type = infobarMessage.Type;
				if (type == InfobarMessageType.Expanding || type == InfobarMessageType.ExpandingError)
				{
					Infobar.RenderExpandingMessage(output, infobarMessage.Type, infobarMessage.Message, infobarMessage.TagId, infobarMessage.LinkText, infobarMessage.ExpandSection, this.sessionContext, infobarMessageType == infobarMessage.Type);
				}
				else
				{
					Infobar.RenderMessage(output, infobarMessage.Type, infobarMessage.Message, infobarMessage.TagId, infobarMessage.HideMessage, this.sessionContext);
				}
				infobarMessageType = infobarMessage.Type;
			}
			output.Write("</div></div>");
		}

		public static void RenderExpandingMessage(TextWriter output, InfobarMessageType messageType, SanitizedHtmlString message, string messageId, SanitizedHtmlString linkText, SanitizedHtmlString expandSection, ISessionContext sessionContext)
		{
			Infobar.RenderExpandingMessage(output, messageType, message, messageId, linkText, expandSection, sessionContext, false);
		}

		public static void RenderExpandingMessage(TextWriter output, InfobarMessageType messageType, SanitizedHtmlString message, string messageId, SanitizedHtmlString linkText, SanitizedHtmlString expandSection, ISessionContext sessionContext, bool isVerticalSpaceRequired)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (linkText == null)
			{
				throw new ArgumentNullException("linkText");
			}
			if (expandSection == null)
			{
				throw new ArgumentNullException("expandSection");
			}
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			Infobar.RenderMessageIdAndClass(output, messageType, messageId, sessionContext);
			if (isVerticalSpaceRequired)
			{
				output.Write(" vsp");
			}
			output.Write("\">");
			string styleClass = sessionContext.IsRtl ? "rtl dashImg" : "dashImg";
			sessionContext.RenderThemeImage(output, ThemeFileId.Dash, styleClass, new object[]
			{
				"id=imgDash"
			});
			if (messageType == InfobarMessageType.ExpandingError)
			{
				output.Write("<span class=\"ibM\">");
			}
			output.Write(message);
			if (messageType == InfobarMessageType.ExpandingError)
			{
				output.Write("</span>");
			}
			output.Write("<span id=spnIbL ");
			Utilities.RenderScriptHandler(output, "onclick", "tglInfo(_this);");
			output.Write(">");
			output.Write(linkText);
			sessionContext.RenderThemeImage(output, ThemeFileId.Expand, null, new object[]
			{
				"id=imgExp"
			});
			output.Write("</span>");
			output.Write("<div id=divIbE ");
			Utilities.RenderScriptHandler(output, "onclick", "canEvt(event);");
			output.Write(" style=\"display:none\">");
			output.Write(expandSection);
			output.Write("</div></div>");
		}

		public void SetInfobarClass(string barClass)
		{
			this.barClass = barClass;
		}

		public void SetShouldHonorHideByDefault(bool shouldHonorHideByDefault)
		{
			this.shouldHonorHideByDefault = shouldHonorHideByDefault;
		}

		public string InfobarClass
		{
			get
			{
				return this.barClass;
			}
		}

		private static bool HasHighSeverityMessages(ArrayList messages)
		{
			foreach (object obj in messages)
			{
				InfobarMessage infobarMessage = (InfobarMessage)obj;
				if (InfobarMessageType.Informational4 < infobarMessage.Type)
				{
					return true;
				}
			}
			return false;
		}

		private bool ShouldHideInfobar(bool renderHidden)
		{
			if (renderHidden || (this.shouldHonorHideByDefault && this.sessionContext.HideMailTipsByDefault))
			{
				return true;
			}
			foreach (object obj in this.messages)
			{
				InfobarMessage infobarMessage = (InfobarMessage)obj;
				if (!infobarMessage.HideMessage)
				{
					return false;
				}
			}
			return true;
		}

		private static void RenderMessageIdAndClass(TextWriter output, InfobarMessageType messageType, string messageId, ISessionContext sessionContext)
		{
			output.Write("<div ");
			if (string.IsNullOrEmpty(messageId))
			{
				messageId = "divInfobarMessage";
			}
			output.Write(SanitizedHtmlString.Format(" id=\"{0}\"", new object[]
			{
				messageId
			}));
			output.Write(" iType=");
			output.Write((int)messageType);
			output.Write(" class=\"");
			output.Write("infobarMessageItem");
			if (sessionContext.IsRtl)
			{
				output.Write(" rtl");
			}
			switch (messageType)
			{
			case InfobarMessageType.ExpandingError:
			case InfobarMessageType.Error:
				output.Write(" error");
				return;
			case InfobarMessageType.JunkEmail:
				output.Write(" junk");
				return;
			case InfobarMessageType.Phishing:
				output.Write(" phishing");
				return;
			case InfobarMessageType.Warning:
				output.Write(" warning");
				return;
			default:
				return;
			}
		}

		private const string InfobarsVisibleAttribute = "isVisible";

		private const string InfobarMessageItemClass = "infobarMessageItem";

		private const string DefaultInfobarMessageDivId = "divInfobarMessage";

		private ArrayList messages = new ArrayList(2);

		private ISessionContext sessionContext;

		private string divErrorId = "divErr";

		private string barClass = "infobar";

		private bool shouldHonorHideByDefault;

		private class InfobarMessageComparer : IComparer
		{
			public int Compare(object objectX, object objectY)
			{
				if (objectX == null)
				{
					throw new ArgumentNullException("objectX");
				}
				if (objectY == null)
				{
					throw new ArgumentNullException("objectY");
				}
				InfobarMessage infobarMessage = objectX as InfobarMessage;
				InfobarMessage infobarMessage2 = objectY as InfobarMessage;
				if (infobarMessage.Type > infobarMessage2.Type)
				{
					return -1;
				}
				if (infobarMessage.Type < infobarMessage2.Type)
				{
					return 1;
				}
				return infobarMessage.Message.CompareTo(infobarMessage2.Message);
			}
		}
	}
}
