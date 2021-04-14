using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class IncompleteInstallationDetectedPage : SetupWizardPage
	{
		public IncompleteInstallationDetectedPage()
		{
			this.InitializeComponent();
			base.PageTitle = Strings.IncompleteInstallationDetectedPageTitle;
			this.incompleteInstallationDetectedSummaryLabel.Text = string.Empty;
			base.WizardCancel += this.IncompleteInstallationDetectedPage_WizardCancel;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void IncompleteInstallationDetectedPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			base.SetWizardButtons(2);
			base.SetVisibleWizardButtons(2);
			this.incompleteInstallationDetectedSummaryLabel.Text = Strings.IncompleteInstallationDetectedSummaryLabelText;
			base.EnableCheckLoadedTimer(200);
		}

		private void IncompleteInstallationDetectedPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.incompleteInstallationDetectedSummaryLabel.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void IncompleteInstallationDetectedPage_WizardCancel(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void InitializeComponent()
		{
			this.incompleteInstallationDetectedSummaryLabel = new Label();
			base.SuspendLayout();
			this.incompleteInstallationDetectedSummaryLabel.AutoSize = true;
			this.incompleteInstallationDetectedSummaryLabel.BackColor = Color.Transparent;
			this.incompleteInstallationDetectedSummaryLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.incompleteInstallationDetectedSummaryLabel.Location = new Point(0, 0);
			this.incompleteInstallationDetectedSummaryLabel.MaximumSize = new Size(720, 125);
			this.incompleteInstallationDetectedSummaryLabel.Name = "incompleteInstallationDetectedSummaryLabel";
			this.incompleteInstallationDetectedSummaryLabel.Size = new Size(272, 17);
			this.incompleteInstallationDetectedSummaryLabel.TabIndex = 29;
			this.incompleteInstallationDetectedSummaryLabel.Text = "[IncompleteInstallationDetectedSummaryText]";
			base.Controls.Add(this.incompleteInstallationDetectedSummaryLabel);
			base.Name = "IncompleteInstallationDetectedPage";
			base.SetActive += this.IncompleteInstallationDetectedPage_SetActive;
			base.CheckLoaded += this.IncompleteInstallationDetectedPage_CheckLoaded;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private IContainer components;

		private Label incompleteInstallationDetectedSummaryLabel;
	}
}
