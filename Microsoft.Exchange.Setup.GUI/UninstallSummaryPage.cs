using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class UninstallSummaryPage : SetupWizardPage
	{
		public UninstallSummaryPage()
		{
			this.InitializeComponent();
			base.PageTitle = Strings.UninstallWelcomeTitle;
			this.uninstallSummaryLabel.Text = Strings.UninstallWelcomeDiscription;
			base.WizardCancel += this.UninstallSummaryPage_WizardCancel;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void UninstallSummaryPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			base.SetWizardButtons(2);
			base.SetVisibleWizardButtons(2);
			base.EnableCheckLoadedTimer(200);
		}

		private void UninstallSummaryPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.uninstallSummaryLabel.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void UninstallSummaryPage_WizardCancel(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void InitializeComponent()
		{
			this.uninstallSummaryLabel = new Label();
			base.SuspendLayout();
			this.uninstallSummaryLabel.AutoSize = true;
			this.uninstallSummaryLabel.BackColor = Color.Transparent;
			this.uninstallSummaryLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.uninstallSummaryLabel.Location = new Point(0, 0);
			this.uninstallSummaryLabel.MaximumSize = new Size(720, 125);
			this.uninstallSummaryLabel.Name = "uninstallSummaryLabel";
			this.uninstallSummaryLabel.Size = new Size(143, 17);
			this.uninstallSummaryLabel.TabIndex = 26;
			this.uninstallSummaryLabel.Text = "[UninstallSummaryText]";
			base.Controls.Add(this.uninstallSummaryLabel);
			base.Name = "UninstallSummaryPage";
			base.SetActive += this.UninstallSummaryPage_SetActive;
			base.CheckLoaded += this.UninstallSummaryPage_CheckLoaded;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private Label uninstallSummaryLabel;

		private IContainer components;
	}
}
