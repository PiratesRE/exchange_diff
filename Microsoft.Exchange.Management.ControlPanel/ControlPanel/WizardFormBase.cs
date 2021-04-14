using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class WizardFormBase : BaseForm
	{
		public WizardFormBase()
		{
			base.FooterPanel.State = ButtonsPanelState.Wizard;
		}

		[DefaultValue(true)]
		[Bindable(true)]
		[Category("Behavior")]
		public bool CloseWindowOnCancel { get; set; }

		protected override void OnPreRender(EventArgs e)
		{
			base.AdditionalContentPanelStyle = " positionStatic " + ((!string.IsNullOrEmpty(base.AdditionalContentPanelStyle)) ? base.AdditionalContentPanelStyle : string.Empty);
			base.OnPreRender(e);
			base.FooterPanel.CloseWindowOnCancel = this.CloseWindowOnCancel;
		}

		[DefaultValue(false)]
		public bool ShowWizardStepTitle { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.ShowWizardStepTitle)
			{
				this.lblStepInfo = new Label();
				this.lblStepInfo.CssClass = "stepTxt";
				this.lblStepInfo.ID = "lblStepInfo";
				base.CaptionPanel.Controls.Add(this.lblStepInfo);
			}
		}

		public string ShowWizardStepClientID
		{
			get
			{
				this.EnsureChildControls();
				return this.lblStepInfo.ClientID;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddElementProperty("BackButton", this.BackButtonClientID, true);
			descriptor.AddElementProperty("NextButton", this.NextButtonClientID, true);
			if (this.ShowWizardStepTitle)
			{
				descriptor.AddProperty("ShowWizardStepTitle", true);
				descriptor.AddElementProperty("ShowWizardStepLabel", this.ShowWizardStepClientID, true);
			}
			base.BuildScriptDescriptor(descriptor);
		}

		public string BackButtonClientID
		{
			get
			{
				this.EnsureChildControls();
				return base.FooterPanel.BackButtonClientID;
			}
		}

		public string NextButtonClientID
		{
			get
			{
				this.EnsureChildControls();
				return base.FooterPanel.NextButtonClientID;
			}
		}

		protected override void SetFormDefaultButton()
		{
		}

		private Label lblStepInfo;
	}
}
