using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Clients;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(ExtenderControlBase))]
	[ClientScriptResource("RichTextEditor", "richtexteditor.js")]
	[ClientScriptResource("RichTextEditor", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ControlValueProperty("Value")]
	public class RichTextEditor : ScriptControlBase
	{
		public RichTextEditor() : base(HtmlTextWriterTag.Div)
		{
			this.formatBar = new Panel();
			this.errorBar = new Panel();
			this.iframeBody = new HtmlGenericControl();
		}

		public string[] ButtonsBIU
		{
			get
			{
				if (this.buttonsBIU == null)
				{
					this.buttonsBIU = new string[]
					{
						Strings.ButtonB,
						Strings.ButtonI,
						Strings.ButtonU
					};
				}
				return this.buttonsBIU;
			}
		}

		public string[] ButtonTooltips
		{
			get
			{
				if (this.buttonTooltips == null)
				{
					this.buttonTooltips = new string[]
					{
						Strings.FormatbarTooltipsBold,
						Strings.FormatbarTooltipsItalics,
						Strings.FormatbarTooltipsUnderline,
						Strings.FormatbarTooltipsStrikethrough,
						Strings.FormatbarTooltipsAlignLeft,
						Strings.FormatbarTooltipsCenter,
						Strings.FormatbarTooltipsAlignRight,
						Strings.FormatbarTooltipsBullets,
						Strings.FormatbarTooltipsNumbering,
						Strings.FormatbarTooltipsDecreaseIndent,
						Strings.FormatbarTooltipsIncreaseIndent,
						Strings.FormatbarTooltipsHighlight,
						Strings.FormatbarTooltipsFontColor,
						Strings.FormatbarTooltipsRemoveFormattingEraser,
						Strings.FormatbarTooltipsInsertHorizontalRule,
						Strings.FormatbarTooltipsUndo,
						Strings.FormatbarTooltipsRedo,
						Strings.FormatbarTooltipsInsertHyperlink,
						Strings.FormatbarTooltipsRemoveHyperlink,
						Strings.FormatbarTooltipsSuperscript,
						Strings.FormatbarTooltipsSubscript,
						Strings.FormatbarTooltipsLeftToRight,
						Strings.FormatbarTooltipsRightToLeft,
						Strings.FormatbarTooltipsCustomize
					};
				}
				return this.buttonTooltips;
			}
		}

		public string[] CustomizeTooltips
		{
			get
			{
				if (this.customizeTooltips == null)
				{
					this.customizeTooltips = new string[]
					{
						Strings.CustomizePaneTooltipsBoldItalicsUnderline,
						Strings.CustomizePaneTooltipsStrikeThrough,
						Strings.CustomizePaneTooltipsAlign,
						Strings.CustomizePaneTooltipsBulletsNumbering,
						Strings.CustomizePaneTooltipsIndent,
						Strings.CustomizePaneTooltipsHighlight,
						Strings.CustomizePaneTooltipsFontColor,
						Strings.CustomizePaneTooltipsRemoveFormattingEraser,
						Strings.CustomizePaneTooltipsInsertHorizontalRule,
						Strings.CustomizePaneTooltipsUndoRedo,
						Strings.CustomizePaneTooltipsInsertRemoveHyperlink,
						Strings.CustomizePaneTooltipsSuperscriptSubscript,
						Strings.CustomizePaneTooltipsLtrRtl,
						Strings.CustomizePaneTooltipsCustomize
					};
				}
				return this.customizeTooltips;
			}
		}

		public string NoneButtonString
		{
			get
			{
				if (this.noneButtonString == null)
				{
					this.noneButtonString = Strings.None;
				}
				return this.noneButtonString;
			}
		}

		public bool IsMessageFontEditor { get; set; }

		[DefaultValue(0)]
		public int MaxLength { get; set; }

		public override void RenderEndTag(HtmlTextWriter writer)
		{
			base.RenderEndTag(writer);
			if (Util.IsIE())
			{
				writer.Write(string.Format("<div id=\"{0}_endDiv\" class=\"OffScreen\" tabindex=\"0\"></div>", this.ClientID));
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			Panel panel = new Panel();
			Panel panel2 = new Panel();
			this.CssClass = "richTextEditor";
			this.formatBar.ID = "divFmtBr";
			this.formatBar.CssClass = "formatBarInUMC";
			if (Util.IsIE())
			{
				base.Attributes.Add("tabindex", "0");
			}
			this.errorBar.ID = "divErrBr";
			this.iframeBody.TagName = HtmlTextWriterTag.Iframe.ToString();
			this.iframeBody.ID = "ifBdy";
			this.iframeBody.Attributes["class"] = "richTextFrame";
			this.iframeBody.Attributes["frameborder"] = "0";
			if (Util.IsIE())
			{
				this.iframeBody.Attributes["src"] = ThemeResource.BlankHtmlPath;
			}
			panel.Controls.Add(this.formatBar);
			panel2.Controls.Add(this.errorBar);
			panel2.Controls.Add(this.iframeBody);
			this.Controls.Add(panel);
			this.Controls.Add(panel2);
			if (this.IsMessageFontEditor)
			{
				this.iframeBody.Attributes["class"] = "richTextFrame messageFontPreview";
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			CommonMaster commonMaster = (CommonMaster)this.Page.Master;
			if (commonMaster != null)
			{
				commonMaster.AddCssFiles("nbsprite1.mouse.css");
				commonMaster.AddCssFiles("EditorStyles.mouse.css");
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("FormatBarID", this.formatBar.ClientID);
			descriptor.AddElementProperty("IFrameBody", this.iframeBody.ClientID);
			descriptor.AddElementProperty("ErrorBar", this.errorBar.ClientID);
			if (Util.IsIE())
			{
				descriptor.AddElementProperty("EndDiv", this.ClientID + "_endDiv");
			}
			descriptor.AddScriptProperty("ButtonsBIU", this.ButtonsBIU.ToJsonString(null));
			descriptor.AddScriptProperty("ButtonTooltips", this.ButtonTooltips.ToJsonString(null));
			descriptor.AddScriptProperty("CustomizeTooltips", this.CustomizeTooltips.ToJsonString(null));
			descriptor.AddScriptProperty("NoneButtonString", this.NoneButtonString.ToJsonString(null));
			descriptor.AddProperty("IsMessageFontEditor", this.IsMessageFontEditor);
			if (this.MaxLength > 0)
			{
				descriptor.AddProperty("MaxLength", this.MaxLength - RichTextEditor.ReservedSpace);
			}
		}

		private const string EndDivFmt = "<div id=\"{0}_endDiv\" class=\"OffScreen\" tabindex=\"0\"></div>";

		private static readonly int ReservedSpace = TextConverterHelper.SanitizeHtml("<br/>").Length - 5;

		private string[] buttonsBIU;

		private string[] buttonTooltips;

		private string[] customizeTooltips;

		private string noneButtonString;

		private Panel formatBar;

		private Panel errorBar;

		private HtmlGenericControl iframeBody;
	}
}
