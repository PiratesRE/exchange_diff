using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("FilterTextBox", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	[ToolboxData("<{0}:FilterTextBox runat=server></{0}:FilterTextBox>")]
	public class FilterTextBox : TextBox, IScriptControl
	{
		[Bindable(true)]
		[DefaultValue(null)]
		public CommandSprite.SpriteId? SearchButtonImageId { get; set; }

		public FilterTextBox()
		{
			this.AutoPostBack = false;
			this.CssClass = "filterTextBox";
			this.MaxLength = 256;
			this.SearchButtonImageId = new CommandSprite.SpriteId?(CommandSprite.SpriteId.SearchDefault);
		}

		public string SearchButtonToolTip
		{
			get
			{
				return this.searchButtonToolTip;
			}
			set
			{
				this.searchButtonToolTip = value;
			}
		}

		public string ClearButtonToolTip
		{
			get
			{
				return this.clearButtonToolTip;
			}
			set
			{
				this.clearButtonToolTip = value;
			}
		}

		public string WatermarkText
		{
			get
			{
				return this.watermarkText;
			}
			set
			{
				this.watermarkText = value;
			}
		}

		[ClientPropertyName("EnableAutoSuggestion")]
		public bool EnableAutoSuggestion
		{
			get
			{
				return this.enableAutoSuggestion;
			}
			set
			{
				this.enableAutoSuggestion = value;
			}
		}

		public string SuggestionServicePath
		{
			get
			{
				return this.suggestionServicePath;
			}
			set
			{
				this.suggestionServicePath = value;
			}
		}

		public string SuggestionServiceWorkFlow
		{
			get
			{
				return this.suggestionServiceWorkflow;
			}
			set
			{
				this.suggestionServiceWorkflow = value;
			}
		}

		public string SuggestionServiceMethod
		{
			get
			{
				return this.suggestionServiceMethod;
			}
			set
			{
				this.suggestionServiceMethod = value;
			}
		}

		public string AutoSuggestionPropertyNames
		{
			get
			{
				return this.autoSuggestionPropertyNames;
			}
			set
			{
				this.autoSuggestionPropertyNames = value;
			}
		}

		public string AutoSuggestionPropertyValues
		{
			get
			{
				return this.autoSuggestionPropertyValues;
			}
			set
			{
				this.autoSuggestionPropertyValues = value;
			}
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			writer.Write("<table id='{0}' cellspacing='0' cellpadding='0' class='{1}' role=\"presentation\" >", this.ClientID + "_container", "filterTextBoxContainer");
			writer.Write("<tr>");
			writer.Write("<td class='{0}'>", "filterTextBoxTd");
			if (this.hiddenForSRLabel != null)
			{
				this.hiddenForSRLabel.RenderControl(writer);
			}
			base.RenderBeginTag(writer);
		}

		public override void RenderEndTag(HtmlTextWriter writer)
		{
			base.RenderEndTag(writer);
			writer.Write("</td>");
			writer.Write("<td class='{0} EnabledToolBarItem' id='{1}'>", "filterIndicatorTd", this.ClientID + "_indicatorTd");
			this.imageButtonFilter.RenderControl(writer);
			if (!string.IsNullOrEmpty(this.WatermarkText))
			{
				this.watermarkExtender.RenderControl(writer);
			}
			if (this.EnableAutoSuggestion)
			{
				this.autoCompleteExtender.RenderControl(writer);
			}
			writer.Write("</td>");
			writer.Write("</tr>");
			writer.Write("</table>");
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("FilterTextBox", this.ClientID);
			scriptControlDescriptor.AddProperty("SearchButtonToolTip", this.SearchButtonToolTip);
			scriptControlDescriptor.AddProperty("ClearButtonToolTip", this.ClearButtonToolTip);
			scriptControlDescriptor.AddProperty("EnableAutoSuggestion", this.EnableAutoSuggestion);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(FilterTextBox));
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			string text = "filter or search";
			if (!string.IsNullOrEmpty(this.WatermarkText))
			{
				text = this.WatermarkText;
			}
			this.hiddenForSRLabel = Util.CreateHiddenForSRLabel(text, this.ID);
			this.Controls.Add(this.hiddenForSRLabel);
			this.imageButtonFilter = new HyperLink();
			this.imageButtonFilter.NavigateUrl = "#";
			this.imageButtonFilter.Attributes.Add("onclick", "javascript:return false;");
			this.imageButtonFilter.ToolTip = this.SearchButtonToolTip;
			this.imageButtonFilter.ID = this.ID + "_SearchButton";
			CommandSprite commandSprite = new CommandSprite();
			if (this.SearchButtonImageId != null)
			{
				commandSprite.ImageId = this.SearchButtonImageId.Value;
			}
			else
			{
				commandSprite.ImageId = CommandSprite.SpriteId.SearchDefault;
			}
			commandSprite.ID = this.imageButtonFilter.ID + "_ImageSearchButton";
			commandSprite.AlternateText = this.SearchButtonToolTip;
			this.imageButtonFilter.Controls.Add(commandSprite);
			EncodingLabel encodingLabel = new EncodingLabel();
			encodingLabel.Text = RtlUtil.SearchDefaultMock;
			encodingLabel.ToolTip = this.SearchButtonToolTip;
			encodingLabel.CssClass = "filterIndicatorImageAlter";
			this.imageButtonFilter.Controls.Add(encodingLabel);
			this.Controls.Add(this.imageButtonFilter);
			this.watermarkExtender = new TextBoxWatermarkExtender();
			this.watermarkExtender.TargetControlID = this.UniqueID;
			this.watermarkExtender.WatermarkCssClass = "TextBoxWatermark";
			this.watermarkExtender.WatermarkText = this.WatermarkText;
			this.Controls.Add(this.watermarkExtender);
			if (this.enableAutoSuggestion)
			{
				this.autoCompleteExtender = new EcpAutoCompleteExtender();
				this.autoCompleteExtender.TargetControlID = this.UniqueID;
				WebServiceMethod webServiceMethod = new WebServiceMethod();
				webServiceMethod.ServiceUrl = new WebServiceReference(string.Format("{0}&workflow={1}", this.suggestionServicePath, this.SuggestionServiceWorkFlow));
				webServiceMethod.ID = this.autoCompleteExtender.ID + "WebServiceMethod";
				webServiceMethod.Method = this.SuggestionServiceMethod;
				webServiceMethod.ParameterNames = (WebServiceParameterNames)Enum.Parse(typeof(WebServiceParameterNames), "GetList");
				this.autoCompleteExtender.WebServiceMethod = webServiceMethod;
				this.autoCompleteExtender.AutoSuggestionPropertyNames = this.autoSuggestionPropertyNames;
				this.autoCompleteExtender.AutoSuggestionPropertyValues = this.autoSuggestionPropertyValues;
				this.Controls.Add(this.autoCompleteExtender);
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<FilterTextBox>(this);
			ScriptObjectBuilder.RegisterCssReferences(this);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		private const int DefaultMaxLength = 256;

		private HyperLink imageButtonFilter;

		private TextBoxWatermarkExtender watermarkExtender;

		private EcpAutoCompleteExtender autoCompleteExtender;

		private EncodingLabel hiddenForSRLabel;

		private string searchButtonToolTip = Strings.SearchButtonTooltip;

		private string clearButtonToolTip = Strings.ClearButtonTooltip;

		private string watermarkText;

		private bool enableAutoSuggestion;

		private string suggestionServicePath;

		private string suggestionServiceWorkflow = "GetSuggestion";

		private string suggestionServiceMethod = "GetList";

		private string autoSuggestionPropertyNames;

		private string autoSuggestionPropertyValues;
	}
}
