using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class SlabFrame : WebControl, INamingContainer
	{
		internal SlabControl Slab { get; set; }

		private CaptionPanel CaptionPanel { get; set; }

		internal bool ShowHelp
		{
			get
			{
				return this.CaptionPanel.ShowHelp;
			}
			set
			{
				this.CaptionPanel.ShowHelp = value;
			}
		}

		internal bool PublishHelp
		{
			get
			{
				return this.CaptionPanel.AddHelpButton;
			}
			set
			{
				this.CaptionPanel.AddHelpButton = value;
			}
		}

		internal WebControl CaptionLabel
		{
			get
			{
				return this.CaptionPanel.TextLabel;
			}
		}

		internal string SaveButtonClientID
		{
			get
			{
				if (this.buttonsPanel == null)
				{
					return null;
				}
				return this.buttonsPanel.CommitButtonClientID;
			}
		}

		internal SlabFrame(SlabControl slab) : base(HtmlTextWriterTag.Div)
		{
			if (slab == null)
			{
				throw new ArgumentNullException("slab", "Slab cannot be null");
			}
			this.Slab = slab;
			this.CssClass = "slbFrm";
			if (this.Slab.HideSlabBorder)
			{
				this.CssClass = "slbFrm noBorderAlways";
			}
			else
			{
				this.CssClass = "slbFrm";
			}
			base.Style[HtmlTextWriterStyle.Visibility] = "hidden";
			this.CaptionPanel = new CaptionPanel();
			this.ShowHelp = false;
			this.PublishHelp = false;
			Unit height = slab.Height;
			if (!height.IsEmpty && height.Type == UnitType.Percentage)
			{
				base.Attributes.Add("fill", ((int)height.Value).ToString());
				return;
			}
			this.Height = slab.Height;
		}

		protected override void OnInit(EventArgs e)
		{
			this.Controls.Clear();
			this.Controls.Add(this.CaptionPanel);
			Panel panel = new Panel();
			if (this.Slab.HideSlabBorder)
			{
				panel.CssClass = "cttPane noBorderAlways";
			}
			else
			{
				panel.CssClass = "cttPane";
			}
			panel.Controls.Add(this.Slab);
			this.Controls.Add(panel);
			this.CaptionPanel.HelpId = this.Slab.HelpId;
			this.CaptionPanel.Text = (string.IsNullOrWhiteSpace(this.Slab.Caption) ? this.Slab.Title : this.Slab.Caption);
			base.Attributes.Add("testid", this.Slab.HelpId);
			this.Page.PreRenderComplete += this.Page_PreRenderComplete;
			base.OnInit(e);
		}

		private bool HideCaption
		{
			get
			{
				return this.Slab.HideSlabBorder || (this.Slab.IsPrimarySlab && !((EcpContentPage)this.Page).ShowHelp);
			}
		}

		private void Page_PreRenderComplete(object sender, EventArgs e)
		{
			if (this.HideCaption)
			{
				this.CaptionPanel.Style[HtmlTextWriterStyle.Display] = "none";
			}
			if (this.buttonsPanel != null)
			{
				this.buttonsPanel.Visible = false;
			}
		}

		internal IBaseFormContentControl PropertiesControl
		{
			get
			{
				return this.Slab.PropertiesControl;
			}
		}

		internal void InitSaveButton()
		{
			if (this.PropertiesControl != null)
			{
				this.buttonsPanel = new ButtonsPanel();
				this.buttonsPanel.State = ButtonsPanelState.Save;
				this.Controls.Add(this.buttonsPanel);
				WebServiceMethod saveWebServiceMethod = this.PropertiesControl.SaveWebServiceMethod;
				if (saveWebServiceMethod != null)
				{
					this.buttonsPanel.SaveWebServiceMethods.Add(saveWebServiceMethod);
				}
			}
		}

		internal void ShowSaveButton()
		{
			if (this.buttonsPanel != null)
			{
				this.buttonsPanel.Visible = true;
				if (!this.Slab.UsePropertyPageStyle)
				{
					SlabFrame.SetFocusCssOnSaveButton(this.buttonsPanel);
				}
			}
		}

		internal static void SetFocusCssOnSaveButton(ButtonsPanel buttonsPanel)
		{
			((HtmlButton)buttonsPanel.FindControl("btnCommit")).Attributes.Add("class", "btnSave");
		}

		private ButtonsPanel buttonsPanel;
	}
}
