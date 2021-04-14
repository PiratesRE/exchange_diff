using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class ContextMenu
	{
		public string Id
		{
			get
			{
				return this.id;
			}
		}

		protected static void RenderMenuDivider(TextWriter output, string id)
		{
			ContextMenu.RenderMenuDivider(output, id, true);
		}

		protected static void RenderMenuDivider(TextWriter output, string id, bool setIsMenuDivider)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (id != null && id.Length == 0)
			{
				throw new InvalidOperationException("Argument id cannot be string empty");
			}
			output.Write("<div");
			if (id != null)
			{
				output.Write(" id=");
				output.Write(id);
			}
			output.Write(" class=\"cmDvdr\"");
			if (setIsMenuDivider)
			{
				output.Write(" _isMnuDvdr=1");
			}
			output.Write("><span>&nbsp;</span></div>");
		}

		protected abstract void RenderMenuItems(TextWriter output);

		public ContextMenu(string id, UserContext userContext) : this(id, userContext, true, false, null)
		{
		}

		protected ContextMenu(string id, UserContext userContext, bool hasImages) : this(id, userContext, hasImages, false, null)
		{
		}

		protected ContextMenu(string id, UserContext userContext, bool hasImages, string className) : this(id, userContext, hasImages, false, className)
		{
		}

		protected ContextMenu(string id, UserContext userContext, bool hasImages, bool isAnr) : this(id, userContext, hasImages, isAnr, null)
		{
		}

		protected ContextMenu(string id, UserContext userContext, bool hasImages, bool isAnr, string className)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id may not be null or empty string");
			}
			this.id = id;
			this.userContext = userContext;
			this.hasImages = hasImages;
			this.isAnr = isAnr;
			this.className = className;
		}

		protected virtual bool HasShadedColumn
		{
			get
			{
				return true;
			}
		}

		protected virtual ushort ImagePadding
		{
			get
			{
				return 12;
			}
		}

		private bool IsLazyLoadAsSubmenu
		{
			get
			{
				return !string.IsNullOrEmpty(this.clientScript);
			}
		}

		public void Render(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=");
			output.Write(this.id);
			output.Write(" class=\"ctxMnu");
			if (this.hasImages)
			{
				output.Write(" ctxMnuIco");
			}
			if (this.shouldScroll)
			{
				output.Write(" ctxMnuScrl");
			}
			if (!string.IsNullOrEmpty(this.className))
			{
				output.Write(" ");
				output.Write(this.className);
			}
			output.Write("\"");
			if (this.shouldScroll)
			{
				output.Write(" fScrl=1");
			}
			this.RenderExpandoData(output);
			output.Write(" style=display:none>");
			if (this.HasShadedColumn)
			{
				output.Write("<div id=\"cmpane\"></div>");
			}
			this.RenderMenuItems(output);
			if (!this.shouldScroll)
			{
				RenderingUtilities.RenderDropShadows(output, this.userContext);
			}
			output.Write("</div>");
		}

		protected virtual void RenderExpandoData(TextWriter output)
		{
		}

		protected virtual void RenderMenuItemExpandoData(TextWriter output)
		{
		}

		protected void RenderHeader(TextWriter output)
		{
			output.Write("<div class=\"sttc\" nowrap><span id=\"spnImg\" class=\"cmIco\">");
			RenderingUtilities.RenderInlineSpacer(output, this.userContext, 16, "imgI");
			output.Write("</span>");
			RenderingUtilities.RenderInlineSpacer(output, this.userContext, 12);
			output.Write("<span id=\"spnHdr\"></span></div>");
		}

		protected void RenderMenuItem(TextWriter output, Strings.IDs displayString, string command)
		{
			this.RenderMenuItem(output, displayString, ThemeFileId.None, null, command, false);
		}

		protected void RenderMenuItem(TextWriter output, Strings.IDs displayString, string command, string additionalStyles)
		{
			this.RenderMenuItem(output, displayString, null, command, additionalStyles);
		}

		protected void RenderMenuItem(TextWriter output, Strings.IDs displayString, string id, string command, string additionalStyles)
		{
			this.RenderMenuItem(output, LocalizedStrings.GetNonEncoded(displayString), null, id, command, false, null, additionalStyles, null, null, null, null, false);
		}

		protected void RenderMenuItem(TextWriter output, Strings.IDs displayString, string command, bool disabled)
		{
			this.RenderMenuItem(output, displayString, ThemeFileId.None, null, command, disabled);
		}

		protected void RenderMenuItem(TextWriter output, Strings.IDs displayString, ThemeFileId imageFileId, string id, string command)
		{
			this.RenderMenuItem(output, displayString, imageFileId, id, command, false);
		}

		protected void RenderMenuItem(TextWriter output, Strings.IDs displayString, ThemeFileId imageFileId, string id, string command, bool disabled)
		{
			this.RenderMenuItem(output, displayString, imageFileId, id, command, disabled, null, null, null);
		}

		protected void RenderMenuItem(TextWriter output, Strings.IDs displayString, ThemeFileId imageFileId, string id, string command, bool disabled, string onMouseOverScript, string onMouseOutScript)
		{
			this.RenderMenuItem(output, displayString, imageFileId, id, command, disabled, onMouseOverScript, onMouseOutScript, null);
		}

		protected void RenderMenuItem(TextWriter output, Strings.IDs displayString, ThemeFileId imageFileId, string id, string command, bool disabled, string onMouseOverScript, string onMouseOutScript, ContextMenu subContextMenu)
		{
			this.RenderMenuItem(output, LocalizedStrings.GetNonEncoded(displayString), imageFileId, id, command, disabled, onMouseOverScript, onMouseOutScript, subContextMenu, null);
		}

		protected void RenderMenuItem(TextWriter output, string text, ThemeFileId imageFileId, string id, string command, bool disabled, string onMouseOverScript, string onMouseOutScript)
		{
			this.RenderMenuItem(output, text, imageFileId, id, command, disabled, onMouseOverScript, onMouseOutScript, null, null);
		}

		protected void RenderMenuItem(TextWriter output, string text, ThemeFileId imageFileId, string id, string command, bool disabled, string onMouseOverScript, string onMouseOutScript, ContextMenu subContextMenu, ContextMenu.RenderMenuItemHtml renderMenuItemHtml)
		{
			this.RenderMenuItem(output, text, imageFileId, id, command, disabled, onMouseOverScript, onMouseOutScript, subContextMenu, renderMenuItemHtml, false);
		}

		protected void RenderMenuItem(TextWriter output, string text, ThemeFileId imageFileId, string id, string command, bool disabled, string onMouseOverScript, string onMouseOutScript, ContextMenu subContextMenu, ContextMenu.RenderMenuItemHtml renderMenuItemHtml, bool blockContent)
		{
			ContextMenu.RenderMenuItemIcon renderMenuItemIcon = null;
			if (this.hasImages && imageFileId != ThemeFileId.None)
			{
				renderMenuItemIcon = delegate(TextWriter writer)
				{
					this.userContext.RenderThemeImage(writer, imageFileId, null, new object[]
					{
						"id=imgI"
					});
				};
			}
			this.RenderMenuItem(output, text, renderMenuItemIcon, id, command, disabled, null, onMouseOverScript, onMouseOutScript, subContextMenu, renderMenuItemHtml, blockContent);
		}

		protected void RenderMenuItem(TextWriter output, string text, string imageUrl, string id, string command, bool disabled, string additionalAttributes, string onMouseOverScript, string onMouseOutScript, ContextMenu subContextMenu, ContextMenu.RenderMenuItemHtml renderMenuItemHtml)
		{
			ContextMenu.RenderMenuItemIcon renderMenuItemIcon = null;
			if (this.hasImages && imageUrl != null)
			{
				renderMenuItemIcon = delegate(TextWriter writer)
				{
					writer.Write("<img id=\"imgI\" class=\"cstmMnuImg\" src=\"");
					Utilities.SanitizeHtmlEncode(imageUrl, writer);
					writer.Write("\">");
				};
			}
			this.RenderMenuItem(output, text, renderMenuItemIcon, id, command, disabled, additionalAttributes, onMouseOverScript, onMouseOutScript, subContextMenu, renderMenuItemHtml, false);
		}

		protected void RenderMenuItem(TextWriter output, string text, ThemeFileId imageFileId, string id, string command, bool disabled, string additionalAttributes, string onMouseOverScript, string onMouseOutScript, ContextMenu subContextMenu, ContextMenu.RenderMenuItemHtml renderMenuItemHtml, bool blockContent)
		{
			ContextMenu.RenderMenuItemIcon renderMenuItemIcon = null;
			if (this.hasImages && imageFileId != ThemeFileId.None)
			{
				renderMenuItemIcon = delegate(TextWriter writer)
				{
					this.userContext.RenderThemeImage(writer, imageFileId, null, new object[]
					{
						"id=imgI"
					});
				};
			}
			this.RenderMenuItem(output, text, renderMenuItemIcon, id, command, disabled, additionalAttributes, onMouseOverScript, onMouseOutScript, subContextMenu, renderMenuItemHtml, blockContent);
		}

		protected void RenderMenuItem(TextWriter output, string text, ContextMenu.RenderMenuItemIcon renderMenuItemIcon, string id, string command, bool disabled, string additionalAttributes, string onMouseOverScript, string onMouseOutScript, ContextMenu subContextMenu, ContextMenu.RenderMenuItemHtml renderMenuItemHtml, bool blockContent)
		{
			this.RenderMenuItem(output, text, renderMenuItemIcon, id, command, disabled, additionalAttributes, null, onMouseOverScript, onMouseOutScript, subContextMenu, renderMenuItemHtml, blockContent);
		}

		protected void RenderMenuItem(TextWriter output, string text, ContextMenu.RenderMenuItemIcon renderMenuItemIcon, string id, string command, bool disabled, string additionalAttributes, string additionalStyles, string onMouseOverScript, string onMouseOutScript, ContextMenu subContextMenu, ContextMenu.RenderMenuItemHtml renderMenuItemHtml, bool blockContent)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (renderMenuItemHtml == null && text == null)
			{
				throw new InvalidOperationException("Must have either a non null string text or a non null renderMenuItemHtml");
			}
			if (!this.hasImages && renderMenuItemIcon != null)
			{
				throw new InvalidOperationException("This menu doesn't have images");
			}
			if (id != null && id.Length == 0)
			{
				throw new InvalidOperationException("Argument id cannot be string empty");
			}
			output.Write("<div _lnk=");
			output.Write(disabled ? "0" : "1");
			output.Write(" class=\"cmLnk");
			if (disabled)
			{
				output.Write(" cmDsbld");
			}
			if (subContextMenu != null)
			{
				output.Write(" subMnu");
			}
			if (!string.IsNullOrEmpty(additionalStyles))
			{
				output.Write(" ");
				output.Write(additionalStyles);
			}
			if (blockContent)
			{
				output.Write(" blockContent");
			}
			output.Write("\"");
			if (!string.IsNullOrEmpty(additionalAttributes))
			{
				output.Write(" ");
				output.Write(additionalAttributes);
			}
			if (id != null)
			{
				output.Write(" id=");
				output.Write(id);
			}
			if (command != null)
			{
				output.Write(" cmd=\"");
				output.Write(command);
				output.Write("\"");
			}
			if (!string.IsNullOrEmpty(onMouseOverScript))
			{
				output.Write(" onmouseover=\"");
				output.Write(onMouseOverScript);
				output.Write("\"");
			}
			if (!string.IsNullOrEmpty(onMouseOutScript))
			{
				output.Write(" onmouseout=\"");
				output.Write(onMouseOutScript);
				output.Write("\"");
			}
			if (subContextMenu != null)
			{
				if (subContextMenu.IsLazyLoadAsSubmenu)
				{
					output.Write(" sSmScript=\"");
					output.Write(subContextMenu.clientScript);
					output.Write("\"");
				}
				else
				{
					output.Write(" sSmId=");
					output.Write(subContextMenu.Id);
				}
			}
			this.RenderMenuItemExpandoData(output);
			output.Write(">");
			this.RenderMenuItemInnerSpan(output, text, renderMenuItemIcon, renderMenuItemHtml, blockContent);
			if (subContextMenu != null)
			{
				RenderingUtilities.RenderInlineSpacer(output, this.userContext, 1);
				ThemeFileId themeFileId = this.userContext.IsRtl ? ThemeFileId.LeftArrow : ThemeFileId.RightArrow;
				this.userContext.RenderThemeImage(output, themeFileId, null, new object[]
				{
					"id=imgRA"
				});
			}
			output.Write("</div>");
			if (subContextMenu != null && !subContextMenu.IsLazyLoadAsSubmenu)
			{
				subContextMenu.Render(output);
			}
		}

		protected void RenderMenuItemInnerSpan(TextWriter output, string text, ContextMenu.RenderMenuItemIcon renderMenuItemIcon, ContextMenu.RenderMenuItemHtml renderMenuItemHtml, bool blockContent)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (renderMenuItemHtml == null && text == null)
			{
				throw new InvalidOperationException("Must have either a non null string text or a non null renderMenuItemHtml");
			}
			if (!this.hasImages && renderMenuItemIcon != null)
			{
				throw new InvalidOperationException("This menu doesn't have images");
			}
			if (this.hasImages || this.isAnr)
			{
				output.Write("<span class=\"cmIco\"");
				if (this.id != null)
				{
					output.Write(" id=spnImg");
				}
				output.Write(">");
				if (!this.isAnr)
				{
					if (renderMenuItemIcon != null)
					{
						renderMenuItemIcon(output);
					}
					else
					{
						RenderingUtilities.RenderInlineSpacer(output, this.userContext, 16, "imgI");
					}
				}
				output.Write("</span>");
				if (!blockContent)
				{
					RenderingUtilities.RenderInlineSpacer(output, this.userContext, this.ImagePadding);
				}
			}
			if (!blockContent)
			{
				output.Write("<span id=spnT>");
			}
			if (text != null)
			{
				string text2 = Utilities.HtmlEncode(text);
				text2 = text2.Replace("\\n", "<br>");
				output.Write(SanitizedHtmlString.GetSanitizedStringWithoutEncoding(text2));
			}
			else
			{
				renderMenuItemHtml(output);
			}
			if (!blockContent)
			{
				output.Write("</span>");
			}
		}

		protected void RenderNoOpMenuItem(TextWriter output, string id, Strings.IDs labelResourceId, string spanId)
		{
			output.Write("<div");
			output.Write(" id=");
			output.Write(id);
			output.Write(">");
			this.RenderMenuItemInnerSpan(output, null, null, delegate(TextWriter writer)
			{
				writer.Write(LocalizedStrings.GetHtmlEncoded(labelResourceId));
				writer.Write("&nbsp;<span id=\"" + spanId + "\">...</span>");
			}, false);
			output.Write("</div>");
		}

		protected void RenderMenuHeader(TextWriter output, string id, Strings.IDs displayString, string additionalStyles)
		{
			output.Write("<div");
			if (!string.IsNullOrEmpty(id))
			{
				output.Write(" id=\"");
				output.Write(id);
				output.Write("\"");
			}
			output.Write(" class=\"mnuItmTxtHdr ");
			if (!string.IsNullOrEmpty(id))
			{
				output.Write(additionalStyles);
			}
			output.Write("\">");
			output.Write(LocalizedStrings.GetHtmlEncoded(displayString));
			output.Write("</div>");
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		protected UserContext userContext;

		protected bool hasImages = true;

		protected bool isAnr;

		protected bool shouldScroll;

		private string id = string.Empty;

		private string className = string.Empty;

		protected string clientScript;

		protected delegate void RenderMenuItemHtml(TextWriter output);

		protected delegate void RenderMenuItemIcon(TextWriter output);
	}
}
