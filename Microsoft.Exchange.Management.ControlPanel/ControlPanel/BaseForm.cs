using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("BaseForm", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class BaseForm : EcpContentPage
	{
		public BaseForm()
		{
			this.footerPanel = new ButtonsPanel();
			this.contentPanel = new Panel();
			this.inPagePanel = new Panel();
			this.ShowHeader = true;
			this.captionPanel = new Panel();
			this.captionPanel.CssClass = "prpgCap";
			this.CaptionLabel = new EllipsisLabel();
			this.CaptionLabel.ID = "caption";
			WebControl textContainer = ((EllipsisLabel)this.CaptionLabel).TextContainer;
			textContainer.CssClass += " capEllipsisDivLabel";
			this.captionPanel.Controls.Add(this.CaptionLabel);
			this.HelpControl = new HelpControl();
			this.HelpControl.ID = "helpCtrl";
			this.HelpControl.CssClass = "prpgHlp";
			this.contentPanel.ID = "contentPanel";
			this.contentPanel.CssClass = "cttPane";
			this.inPagePanel.ID = "inPagePanel";
			this.inPagePanel.CssClass = "baseFrm prpg";
			this.inPagePanel.Controls.Add(this.captionPanel);
			this.inPagePanel.Controls.Add(this.HelpControl);
			this.inPagePanel.Controls.Add(this.contentPanel);
			this.inPagePanel.Controls.Add(this.footerPanel);
			this.ReservedSpaceForFVA = true;
			this.SetInitialFocus = true;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("CommitButton", this.CommitButtonClientID, true);
			descriptor.AddElementProperty("CancelButton", this.CancelButtonClientID, true);
			descriptor.AddElementProperty("ContentPanel", this.ContentPanel.ClientID, true);
			descriptor.AddProperty("HideFieldValidationAssistant", this.HideFieldValidationAssistant, true);
			descriptor.AddProperty("FvaEnabled", this.ReservedSpaceForFVA && !this.HideFieldValidationAssistant, true);
			descriptor.AddProperty("FvaResource", this.FVAResource);
			descriptor.AddProperty("PassingDataOnClient", this.PassingDataOnClient, true);
			descriptor.AddProperty("HideDefaultCancelAction", this.HideDefaultCancelAction, false);
			descriptor.AddProperty("AlwaysCheckDataLoss", this.AlwaysCheckDataLoss, false);
			descriptor.AddProperty("SetInitialFocus", this.SetInitialFocus, false);
		}

		private HelpControl HelpControl { get; set; }

		[Bindable(true)]
		[DefaultValue(true)]
		[Category("Appearance")]
		public bool ShowHeader { get; set; }

		[Localizable(true)]
		[Category("Appearance")]
		[Bindable(true)]
		[DefaultValue("")]
		public string Caption
		{
			get
			{
				return ((EllipsisLabel)this.CaptionLabel).Text;
			}
			set
			{
				((EllipsisLabel)this.CaptionLabel).Text = value;
			}
		}

		public WebControl CaptionLabel { get; private set; }

		[Bindable(true)]
		[Category("Behavior")]
		[DefaultValue("")]
		public string HelpId
		{
			get
			{
				return this.HelpControl.HelpId;
			}
			set
			{
				this.HelpControl.HelpId = value;
			}
		}

		[DefaultValue("")]
		[Bindable(true)]
		[Category("Behavior")]
		public string AdditionalContentPanelStyle { get; set; }

		public string FVAResource { get; set; }

		[DefaultValue(true)]
		public bool ReservedSpaceForFVA { get; set; }

		public bool HideFieldValidationAssistant { get; set; }

		public bool HideDefaultCancelAction { get; set; }

		public bool AlwaysCheckDataLoss { get; set; }

		[DefaultValue(true)]
		public bool SetInitialFocus { get; set; }

		public string FieldValidationAssistantCanvas { get; set; }

		protected Panel ContentPanel
		{
			get
			{
				return this.contentPanel;
			}
		}

		protected Panel CaptionPanel
		{
			get
			{
				return this.captionPanel;
			}
		}

		protected ContentPlaceHolder ContentPlaceHolder
		{
			get
			{
				return this.iMasterPage.ContentPlaceHolder;
			}
		}

		protected Panel InPagePanel
		{
			get
			{
				return this.inPagePanel;
			}
		}

		internal virtual bool PassingDataOnClient
		{
			get
			{
				return false;
			}
		}

		[Localizable(true)]
		[DefaultValue("")]
		[Bindable(true)]
		[Category("Appearance")]
		public string CommitButtonText
		{
			get
			{
				return this.footerPanel.CommitButtonText;
			}
			set
			{
				this.footerPanel.CommitButtonText = value;
			}
		}

		public HtmlButton CommitButton
		{
			get
			{
				return this.footerPanel.CommitButton;
			}
		}

		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue("")]
		[Bindable(true)]
		public string CancelButtonText
		{
			get
			{
				return this.footerPanel.CancelButtonText;
			}
			set
			{
				this.footerPanel.CancelButtonText = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		[Bindable(true)]
		public string CancelButtonTextTagName
		{
			get
			{
				return this.footerPanel.CancelButtonTextTagName;
			}
			set
			{
				this.footerPanel.CancelButtonTextTagName = value;
			}
		}

		public HtmlButton CancelButton
		{
			get
			{
				return this.footerPanel.CancelButton;
			}
		}

		public HtmlButton BackButton
		{
			get
			{
				this.EnsureChildControls();
				return this.FooterPanel.BackButton;
			}
		}

		[Bindable(true)]
		[DefaultValue("")]
		[Category("Appearance")]
		[Localizable(true)]
		public string BackButtonText
		{
			get
			{
				return this.FooterPanel.BackButtonText;
			}
			set
			{
				this.FooterPanel.BackButtonText = value;
			}
		}

		public HtmlButton NextButton
		{
			get
			{
				this.EnsureChildControls();
				return this.FooterPanel.NextButton;
			}
		}

		[Bindable(true)]
		[Localizable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		public string NextButtonText
		{
			get
			{
				return this.FooterPanel.NextButtonText;
			}
			set
			{
				this.FooterPanel.NextButtonText = value;
			}
		}

		protected IBaseFormContentControl ContentControl
		{
			get
			{
				this.EnsureChildControls();
				if (this.contentControl == null)
				{
					foreach (object obj in this.ContentPanel.Controls)
					{
						Control control = (Control)obj;
						IBaseFormContentControl baseFormContentControl = control as IBaseFormContentControl;
						if (baseFormContentControl != null)
						{
							this.contentControl = baseFormContentControl;
							break;
						}
					}
				}
				return this.contentControl;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.ContentControl != null && this.ContentControl.SaveWebServiceMethod != null)
			{
				this.footerPanel.SaveWebServiceMethods.Add(this.ContentControl.SaveWebServiceMethod);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (!string.IsNullOrEmpty(this.AdditionalContentPanelStyle))
			{
				Panel panel = this.contentPanel;
				panel.CssClass = panel.CssClass + " " + this.AdditionalContentPanelStyle;
			}
			if (this.ShowHeader)
			{
				Panel panel2 = this.inPagePanel;
				panel2.CssClass += " sHdr";
				this.HelpControl.ShowHelp = base.ShowHelp;
				string caption = this.Caption;
				if (string.IsNullOrEmpty(caption))
				{
					this.CaptionLabel.Attributes.Add("data-value", "{DefaultCaptionText, Mode=OneWay}");
				}
				else if (caption.IsBindingExpression())
				{
					this.CaptionLabel.Attributes.Add("data-value", caption);
					this.Caption = string.Empty;
				}
				this.captionPanel.Attributes.Add("data-control", "ContainerControl");
				this.captionPanel.Attributes.Add("data-cssbinder-uppercase", "{ShowCapitalCaption, Mode=OneWay}");
			}
			else
			{
				Panel panel3 = this.inPagePanel;
				panel3.CssClass += " noHdr";
				this.captionPanel.Visible = false;
				this.CaptionLabel.Visible = false;
				this.HelpControl.Visible = false;
			}
			this.footerPanel.CloseWindowOnCancel = false;
			this.SetFormDefaultButton();
			if (!string.IsNullOrEmpty(this.FVAResource))
			{
				base.ScriptManager.EnableScriptLocalization = true;
				((ToolkitScriptManager)base.ScriptManager).CombineScript(this.FVAResource);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			SectionCollection sectionCollection = (this.ContentControl != null) ? this.ContentControl.Sections : null;
			if (sectionCollection != null && sectionCollection.Count > 1)
			{
				Bookmark bookmark = new Bookmark();
				for (int i = 0; i < sectionCollection.Count; i++)
				{
					this.SetInitialFocus = false;
					Section section = sectionCollection[i];
					if (section.Visible)
					{
						bookmark.AddEntry(section.ID, section.Title, section.WorkflowName, section.ClientVisibilityBinding);
						if (i != 0)
						{
							section.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "none");
						}
					}
				}
				this.inPagePanel.Controls.AddAt(2, bookmark);
				Panel panel = this.inPagePanel;
				panel.CssClass += " sBmk";
			}
			else
			{
				Panel panel2 = this.inPagePanel;
				panel2.CssClass += " nobmk";
			}
			if (!this.ReadOnly && this.ContentControl != null && this.ContentControl.ReadOnly)
			{
				this.ReadOnly = true;
			}
			base.Render(writer);
		}

		protected virtual void SetFormDefaultButton()
		{
			this.SetPanelDefaultButton(this.inPagePanel, this.ReadOnly ? this.footerPanel.CancelButtonUniqueID : this.footerPanel.CommitButtonUniqueID);
		}

		private void SetPanelDefaultButton(Panel panel, string buttonUniqueId)
		{
			if (panel.NamingContainer == this.Page)
			{
				panel.DefaultButton = buttonUniqueId;
				return;
			}
			string uniqueID = panel.NamingContainer.UniqueID;
			int num = uniqueID.Length + 1;
			panel.DefaultButton = buttonUniqueId.Substring(num, buttonUniqueId.Length - num);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (!base.DesignMode)
			{
				MasterPage master = base.Master;
				while (master != null && this.iMasterPage == null)
				{
					this.iMasterPage = (master as IMasterPage);
					master = master.Master;
				}
			}
			if (base.Form != null)
			{
				Control contentPlaceHolder = this.ContentPlaceHolder;
				if (contentPlaceHolder != null)
				{
					this.InjectDefaultLayoutControls(contentPlaceHolder);
					this.InitExtenderParameters();
				}
			}
			if (!string.IsNullOrEmpty(this.SetRoles) && !LoginUtil.IsInRoles(this.Context.User, this.SetRoles.Split(new char[]
			{
				','
			})))
			{
				this.ReadOnly = true;
			}
		}

		private void InjectDefaultLayoutControls(Control rootControl)
		{
			Control[] array = new Control[rootControl.Controls.Count];
			rootControl.Controls.CopyTo(array, 0);
			rootControl.Controls.Clear();
			foreach (Control child in array)
			{
				this.ContentPanel.Controls.Add(child);
			}
			rootControl.Controls.Add(this.inPagePanel);
			this.contentControl = null;
		}

		private void InitExtenderParameters()
		{
			if (!this.HideFieldValidationAssistant)
			{
				FieldValidationAssistantExtender fieldValidationAssistantExtender = new FieldValidationAssistantExtender();
				fieldValidationAssistantExtender.HelpId = this.HelpId;
				fieldValidationAssistantExtender.LocStringsResource = this.FVAResource;
				fieldValidationAssistantExtender.TargetControlID = this.ContentPanel.UniqueID;
				fieldValidationAssistantExtender.Canvas = (this.FieldValidationAssistantCanvas ?? this.ContentPanel.ClientID);
				fieldValidationAssistantExtender.IndentCssClass = "baseFrmFvaIndent";
				this.inPagePanel.Controls.Add(fieldValidationAssistantExtender);
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.footerPanel.State == ButtonsPanelState.ReadOnly;
			}
			set
			{
				this.footerPanel.State = (value ? ButtonsPanelState.ReadOnly : ButtonsPanelState.SaveCancel);
			}
		}

		public string CommitButtonClientID
		{
			get
			{
				this.EnsureChildControls();
				return this.footerPanel.CommitButtonClientID;
			}
		}

		public string CancelButtonClientID
		{
			get
			{
				this.EnsureChildControls();
				return this.footerPanel.CancelButtonClientID;
			}
		}

		public string SetRoles { get; set; }

		protected ButtonsPanel FooterPanel
		{
			get
			{
				return this.footerPanel;
			}
		}

		private Panel inPagePanel;

		private Panel contentPanel;

		private Panel captionPanel;

		private ButtonsPanel footerPanel;

		private IMasterPage iMasterPage;

		private IBaseFormContentControl contentControl;
	}
}
