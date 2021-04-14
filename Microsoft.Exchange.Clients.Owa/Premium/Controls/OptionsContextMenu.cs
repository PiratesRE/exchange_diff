using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class OptionsContextMenu : ContextMenu
	{
		public OptionsContextMenu(UserContext userContext) : base("divOptionsContextMenu", userContext, false)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div class=\"alertPopupShading\"></div>");
			output.Write("<div id=\"divOptsMnuTopBorderLeft\" class=\"alertDialogTopBorder\"></div>");
			output.Write("<div id=\"divOptsMnuTopBorderRight\" class=\"alertDialogTopBorder\"></div>");
			base.RenderMenuHeader(output, null, 1511584348, null);
			base.RenderMenuItem(output, -1226104492, "navToOptOOF", "mnuItmTxtItmIndented");
			if (this.userContext.IsFeatureEnabled(Feature.ChangePassword))
			{
				base.RenderMenuItem(output, -1294384513, "navToOptPwd", "mnuItmTxtItmIndented");
			}
			if (this.userContext.IsFeatureEnabled(Feature.Rules))
			{
				base.RenderMenuItem(output, 1115834861, "navToOptRules", "mnuItmTxtItmIndented");
			}
			base.RenderMenuItem(output, -657439717, "navToOptions", "mnuItmTxtItmIndented mnuItmTxtItmSpaced");
			if (this.userContext.IsFeatureEnabled(Feature.Themes))
			{
				this.RenderThemeSelector(output);
			}
		}

		private void RenderThemeSelector(TextWriter output)
		{
			base.RenderMenuHeader(output, "ThmTtl", 582309493, null);
			output.Write("<div class=\"dynamicSelector\">");
			output.Write("<span id=\"leftarrow\" class=\"thmSelArrw\">");
			this.userContext.RenderThemeImage(output, ThemeFileId.PreviousArrow);
			output.Write("</span>");
			output.Write("<span class=\"inlineContainer\">");
			output.Write("<div id=\"Themes\" class=\"container\">");
			this.RenderThemeThumbnails(output);
			output.Write("</div>");
			this.userContext.RenderThemeImage(output, ThemeFileId.Progress, "themePrg", new object[]
			{
				"id=\"ThemeProgress\"",
				"style=\"display:none\""
			});
			output.Write("</span>");
			output.Write("<span id=\"rightarrow\" class=\"thmSelArrw\">");
			this.userContext.RenderThemeImage(output, ThemeFileId.NextArrow);
			output.Write("</span>");
			output.Write("</div>");
		}

		private void RenderThemeThumbnails(TextWriter output)
		{
			BrandingUtilities.IsBranded();
			IDictionary dictionary = new Dictionary<string, string>();
			string storageId = base.UserContext.Theme.StorageId;
			for (int i = 0; i < ThemeManager.Themes.Length; i++)
			{
				Theme theme = ThemeManager.Themes[i];
				dictionary.Add(theme.StorageId, theme.DisplayName);
			}
			int value = 40 * (dictionary.Count / 2 + ((dictionary.Count % 2 > 0) ? 1 : 0));
			output.Write("<div id=\"divThemes\" class=\"scroller\" style=\"width:");
			output.Write(value);
			output.Write("px;\">");
			output.Write("<span class=\"ThmPreviews\">");
			int num = 0;
			foreach (object obj in dictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (num == 2)
				{
					output.Write("</span><span class=\"ThmPreviews\">");
					num = 0;
				}
				output.Write("<span oV=\"" + dictionaryEntry.Key.ToString() + "\" ");
				output.Write("oP=\"" + ThemeManager.Themes[(int)((UIntPtr)ThemeManager.GetIdFromStorageId(dictionaryEntry.Key.ToString()))].Url + "\" ");
				if (storageId == dictionaryEntry.Key.ToString())
				{
					output.Write("class=\"selThm\" ");
				}
				output.Write("id=\"ThmPreview\">");
				output.Write("<img tabindex=\"0\" src=\"");
				ThemeManager.RenderThemePreviewUrl(output, (string)dictionaryEntry.Key);
				output.Write("\" alt=\"");
				if (!string.IsNullOrEmpty(dictionaryEntry.Value.ToString()))
				{
					Utilities.HtmlEncode(dictionaryEntry.Value.ToString(), output);
				}
				output.Write("\" title=\"");
				if (!string.IsNullOrEmpty(dictionaryEntry.Value.ToString()))
				{
					Utilities.HtmlEncode(dictionaryEntry.Value.ToString(), output);
				}
				output.Write("\">");
				output.Write("</span>");
				num++;
			}
			output.Write("</span></div>");
		}

		private const int ThemePreviewWidth = 40;

		private const int PreviewsPerColumn = 2;
	}
}
