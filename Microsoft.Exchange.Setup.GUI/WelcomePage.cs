using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class WelcomePage : SetupWizardPage
	{
		public WelcomePage(RootDataHandler rootDataHandler)
		{
			this.InitializeComponent();
			base.PageTitle = rootDataHandler.IntroTitle;
			this.welcomeLabel.Text = rootDataHandler.IntroDescription;
			this.planDeploymentLabel.Text = Strings.PlanDeploymentLabel;
			this.planDeploymentLinkLabel1.Text = Strings.PlanDeploymentLinkLabel1Text;
			this.planDeploymentLinkLabel1.Links.Add(0, 0, Strings.PlanDeploymentLinkLabel1Link);
			this.planDeploymentLinkLabel2.Text = Strings.PlanDeploymentLinkLabel2Text;
			this.planDeploymentLinkLabel2.Links.Add(0, 0, Strings.PlanDeploymentLinkLabel2Link);
			this.planDeploymentLinkLabel3.Text = Strings.PlanDeploymentLinkLabel3Text;
			this.planDeploymentLinkLabel3.Links.Add(0, 0, Strings.PlanDeploymentLinkLabel3Link);
			base.WizardCancel += this.WelcomePage_WizardCancel;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void WelcomePage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			base.SetWizardButtons(2);
			base.SetVisibleWizardButtons(2);
			base.EnableCheckLoadedTimer(200);
		}

		private void WelcomePage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.planDeploymentLinkLabel2.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void PlanDeploymentLinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.planDeploymentLinkLabel1.Links[this.planDeploymentLinkLabel1.Links.IndexOf(e.Link)].Visited = true;
			Process.Start(e.Link.LinkData.ToString());
		}

		private void PlanDeploymentLinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.planDeploymentLinkLabel2.Links[this.planDeploymentLinkLabel2.Links.IndexOf(e.Link)].Visited = true;
			Process.Start(e.Link.LinkData.ToString());
		}

		private void PlanDeploymentLinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.planDeploymentLinkLabel3.Links[this.planDeploymentLinkLabel3.Links.IndexOf(e.Link)].Visited = true;
			Process.Start(e.Link.LinkData.ToString());
		}

		private void WelcomePage_WizardCancel(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void InitializeComponent()
		{
			this.planDeploymentLinkLabel3 = new LinkLabel();
			this.planDeploymentLinkLabel1 = new LinkLabel();
			this.planDeploymentLinkLabel2 = new LinkLabel();
			this.welcomeLabel = new Label();
			this.planDeploymentLabel = new Label();
			base.SuspendLayout();
			this.planDeploymentLinkLabel3.AutoSize = true;
			this.planDeploymentLinkLabel3.BackColor = Color.Transparent;
			this.planDeploymentLinkLabel3.LinkBehavior = LinkBehavior.NeverUnderline;
			this.planDeploymentLinkLabel3.LinkColor = Color.FromArgb(0, 114, 198);
			this.planDeploymentLinkLabel3.Location = new Point(0, 321);
			this.planDeploymentLinkLabel3.Name = "planDeploymentLinkLabel3";
			this.planDeploymentLinkLabel3.Size = new Size(170, 17);
			this.planDeploymentLinkLabel3.TabIndex = 15;
			this.planDeploymentLinkLabel3.TabStop = true;
			this.planDeploymentLinkLabel3.Text = "[PlanDeploymentLinkLabel3]";
			this.planDeploymentLinkLabel3.LinkClicked += this.PlanDeploymentLinkLabel3_LinkClicked;
			this.planDeploymentLinkLabel1.AutoSize = true;
			this.planDeploymentLinkLabel1.BackColor = Color.Transparent;
			this.planDeploymentLinkLabel1.LinkBehavior = LinkBehavior.NeverUnderline;
			this.planDeploymentLinkLabel1.LinkColor = Color.FromArgb(0, 114, 198);
			this.planDeploymentLinkLabel1.Location = new Point(0, 267);
			this.planDeploymentLinkLabel1.Name = "planDeploymentLinkLabel1";
			this.planDeploymentLinkLabel1.Size = new Size(170, 17);
			this.planDeploymentLinkLabel1.TabIndex = 13;
			this.planDeploymentLinkLabel1.TabStop = true;
			this.planDeploymentLinkLabel1.Text = "[PlanDeploymentLinkLabel1]";
			this.planDeploymentLinkLabel1.LinkClicked += this.PlanDeploymentLinkLabel1_LinkClicked;
			this.planDeploymentLinkLabel2.AutoSize = true;
			this.planDeploymentLinkLabel2.BackColor = Color.Transparent;
			this.planDeploymentLinkLabel2.LinkBehavior = LinkBehavior.NeverUnderline;
			this.planDeploymentLinkLabel2.LinkColor = Color.FromArgb(0, 114, 198);
			this.planDeploymentLinkLabel2.Location = new Point(0, 294);
			this.planDeploymentLinkLabel2.Name = "planDeploymentLinkLabel2";
			this.planDeploymentLinkLabel2.Size = new Size(170, 17);
			this.planDeploymentLinkLabel2.TabIndex = 14;
			this.planDeploymentLinkLabel2.TabStop = true;
			this.planDeploymentLinkLabel2.Text = "[PlanDeploymentLinkLabel2]";
			this.planDeploymentLinkLabel2.LinkClicked += this.PlanDeploymentLinkLabel2_LinkClicked;
			this.welcomeLabel.AutoSize = true;
			this.welcomeLabel.BackColor = Color.Transparent;
			this.welcomeLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.welcomeLabel.Location = new Point(0, 0);
			this.welcomeLabel.Margin = new Padding(1, 0, 1, 0);
			this.welcomeLabel.MaximumSize = new Size(720, 0);
			this.welcomeLabel.Name = "welcomeLabel";
			this.welcomeLabel.Size = new Size(94, 17);
			this.welcomeLabel.TabIndex = 18;
			this.welcomeLabel.Text = "[WelcomeText]";
			this.planDeploymentLabel.AutoSize = true;
			this.planDeploymentLabel.BackColor = Color.Transparent;
			this.planDeploymentLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.planDeploymentLabel.Location = new Point(0, 240);
			this.planDeploymentLabel.Margin = new Padding(1);
			this.planDeploymentLabel.MaximumSize = new Size(720, 0);
			this.planDeploymentLabel.Name = "planDeploymentLabel";
			this.planDeploymentLabel.Size = new Size(176, 17);
			this.planDeploymentLabel.TabIndex = 19;
			this.planDeploymentLabel.Text = "[PlanDeploymmentLabelText]";
			base.Controls.Add(this.planDeploymentLabel);
			base.Controls.Add(this.planDeploymentLinkLabel3);
			base.Controls.Add(this.planDeploymentLinkLabel2);
			base.Controls.Add(this.planDeploymentLinkLabel1);
			base.Controls.Add(this.welcomeLabel);
			base.Name = "WelcomePage";
			base.SetActive += this.WelcomePage_SetActive;
			base.CheckLoaded += this.WelcomePage_CheckLoaded;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private IContainer components;

		private LinkLabel planDeploymentLinkLabel3;

		private LinkLabel planDeploymentLinkLabel1;

		private Label welcomeLabel;

		private LinkLabel planDeploymentLinkLabel2;

		private Label planDeploymentLabel;
	}
}
