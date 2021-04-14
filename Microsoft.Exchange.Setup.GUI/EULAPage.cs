using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class EULAPage : SetupWizardPage
	{
		public EULAPage(RootDataHandler rootDataHandler)
		{
			this.rdh = rootDataHandler;
			this.isLanguagePackOperation = (this.rdh.IsUmLanguagePackOperation || this.rdh.IsLanguagePackOperation);
			this.InitializeComponent();
			base.PageTitle = Strings.EULAPageText;
			this.eulaLabel.Text = Strings.EulaLabelText;
			this.eulaAgreeRadioButton.Text = Strings.AcceptEULAText;
			this.eulaNotAgreeRadioButton.Text = Strings.NotAcceptEULAText;
			this.eulaDisplayRichTextBox.Text = string.Empty;
			this.eulaAgreeRadioButton.CheckedChanged += this.CustomRadioButtonCheckChanged;
			base.WizardCancel += this.EULAPage_WizardCancel;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void EULAPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			if (this.isLanguagePackOperation)
			{
				base.SetVisibleWizardButtons(2);
				if (this.eulaAgreeRadioButton.Checked)
				{
					base.SetWizardButtons(2);
				}
			}
			else if (this.eulaAgreeRadioButton.Checked)
			{
				base.SetWizardButtons(3);
				base.SetVisibleWizardButtons(2);
			}
			else
			{
				base.SetWizardButtons(1);
			}
			this.licenseFile = LicenseAgreementFactory.GetLicenseFileFullPathName(this.rdh.Mode);
			if (!string.IsNullOrEmpty(this.licenseFile))
			{
				this.eulaDisplayRichTextBox.LoadFile(this.licenseFile);
				this.eulaAgreeRadioButton.Checked = false;
				this.eulaNotAgreeRadioButton.Checked = true;
				if (this.isLanguagePackOperation)
				{
					base.SetWizardButtons(0);
					base.SetVisibleWizardButtons(2);
				}
				else
				{
					base.SetWizardButtons(1);
					base.SetVisibleWizardButtons(3);
				}
				base.EnablePrintButton(true);
				base.SetPrintDocument(this.licenseFile);
			}
			else
			{
				this.eulaDisplayRichTextBox.Text = Strings.NoEndUserLicenseAgreement;
				base.SetWizardButtons(0);
				this.eulaAgreeRadioButton.Enabled = false;
				this.eulaNotAgreeRadioButton.Enabled = false;
			}
			this.eulaDisplayRichTextBox.InitializeCustomScrollbar();
			base.EnableCheckLoadedTimer(200);
		}

		private void EULAPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.eulaNotAgreeRadioButton.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void EULAPage_WizardCancel(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void CustomRadioButtonCheckChanged(object sender, EventArgs e)
		{
			CustomRadiobutton customRadiobutton = (CustomRadiobutton)sender;
			if (customRadiobutton.Checked)
			{
				if (customRadiobutton == this.eulaAgreeRadioButton)
				{
					this.eulaNotAgreeRadioButton.Checked = false;
					if (this.isLanguagePackOperation)
					{
						base.SetWizardButtons(2);
					}
					else
					{
						base.SetWizardButtons(3);
					}
					base.SetVisibleWizardButtons(2);
					return;
				}
				this.eulaAgreeRadioButton.Checked = false;
				if (this.isLanguagePackOperation)
				{
					base.SetWizardButtons(0);
					return;
				}
				base.SetWizardButtons(1);
			}
		}

		private void InitializeComponent()
		{
			this.descriptionPanel = new Panel();
			this.eulaLabel = new Label();
			this.outerPanel = new Panel();
			this.eulaDisplayRichTextBox = new CustomRichTextBox();
			this.eulaAgreeRadioButton = new CustomRadiobutton();
			this.eulaNotAgreeRadioButton = new CustomRadiobutton();
			this.optionsPanel = new Panel();
			this.spacerPanel = new Panel();
			this.spacerBottomPanel = new Panel();
			this.titlePanel = new Panel();
			this.outerPanel.SuspendLayout();
			this.optionsPanel.SuspendLayout();
			this.titlePanel.SuspendLayout();
			base.SuspendLayout();
			this.descriptionPanel.AutoSize = true;
			this.descriptionPanel.Dock = DockStyle.Top;
			this.descriptionPanel.Location = new Point(0, 37);
			this.descriptionPanel.Name = "descriptionPanel";
			this.descriptionPanel.Size = new Size(721, 0);
			this.descriptionPanel.TabIndex = 23;
			this.eulaLabel.AutoSize = true;
			this.eulaLabel.BackColor = Color.Transparent;
			this.eulaLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.eulaLabel.ForeColor = Color.FromArgb(51, 51, 51);
			this.eulaLabel.Location = new Point(0, 0);
			this.eulaLabel.MaximumSize = new Size(741, 125);
			this.eulaLabel.Name = "eulaLabel";
			this.eulaLabel.Size = new Size(64, 17);
			this.eulaLabel.TabIndex = 3;
			this.eulaLabel.Text = "[EulaText]";
			this.outerPanel.BackColor = SystemColors.ControlLightLight;
			this.outerPanel.Controls.Add(this.eulaDisplayRichTextBox);
			this.outerPanel.Dock = DockStyle.Fill;
			this.outerPanel.Location = new Point(0, 37);
			this.outerPanel.Name = "outerPanel";
			this.outerPanel.Size = new Size(721, 325);
			this.outerPanel.TabIndex = 16;
			this.eulaDisplayRichTextBox.BackColor = SystemColors.ControlLightLight;
			this.eulaDisplayRichTextBox.Dock = DockStyle.Fill;
			this.eulaDisplayRichTextBox.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.eulaDisplayRichTextBox.Location = new Point(0, 0);
			this.eulaDisplayRichTextBox.Margin = new Padding(0);
			this.eulaDisplayRichTextBox.Name = "eulaDisplayRichTextBox";
			this.eulaDisplayRichTextBox.Size = new Size(721, 325);
			this.eulaDisplayRichTextBox.TabIndex = 12;
			this.eulaDisplayRichTextBox.Text = "[EulaRichTextBoxText]";
			this.eulaAgreeRadioButton.AutoSize = true;
			this.eulaAgreeRadioButton.BackColor = Color.Transparent;
			this.eulaAgreeRadioButton.Checked = false;
			this.eulaAgreeRadioButton.DisabledColor = SetupWizardPage.DefaultDisabledColor;
			this.eulaAgreeRadioButton.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.eulaAgreeRadioButton.ForeColor = SetupWizardPage.DefaultNormalColor;
			this.eulaAgreeRadioButton.HighlightedColor = SetupWizardPage.DefaultHighlightColor;
			this.eulaAgreeRadioButton.Highligted = false;
			this.eulaAgreeRadioButton.Location = new Point(0, 3);
			this.eulaAgreeRadioButton.Margin = new Padding(2);
			this.eulaAgreeRadioButton.Name = "eulaAgreeRadioButton";
			this.eulaAgreeRadioButton.NormalColor = SetupWizardPage.DefaultNormalColor;
			this.eulaAgreeRadioButton.Size = new Size(730, 19);
			this.eulaAgreeRadioButton.TabIndex = 16;
			this.eulaAgreeRadioButton.Text = "[EulaAgreeRadioButton]";
			this.eulaAgreeRadioButton.TextGap = 10;
			this.eulaAgreeRadioButton.CheckedChanged += this.CustomRadioButtonCheckChanged;
			this.eulaNotAgreeRadioButton.AutoSize = true;
			this.eulaNotAgreeRadioButton.BackColor = Color.Transparent;
			this.eulaNotAgreeRadioButton.Checked = false;
			this.eulaNotAgreeRadioButton.DisabledColor = SetupWizardPage.DefaultDisabledColor;
			this.eulaNotAgreeRadioButton.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.eulaNotAgreeRadioButton.ForeColor = SetupWizardPage.DefaultNormalColor;
			this.eulaNotAgreeRadioButton.HighlightedColor = SetupWizardPage.DefaultHighlightColor;
			this.eulaNotAgreeRadioButton.Highligted = false;
			this.eulaNotAgreeRadioButton.Location = new Point(0, 32);
			this.eulaNotAgreeRadioButton.Margin = new Padding(2);
			this.eulaNotAgreeRadioButton.Name = "eulaNotAgreeRadioButton";
			this.eulaNotAgreeRadioButton.NormalColor = SetupWizardPage.DefaultNormalColor;
			this.eulaNotAgreeRadioButton.Size = new Size(730, 19);
			this.eulaNotAgreeRadioButton.TabIndex = 17;
			this.eulaNotAgreeRadioButton.Text = "[EulaNotAgreeRadioButton]";
			this.eulaNotAgreeRadioButton.TextGap = 10;
			this.eulaNotAgreeRadioButton.CheckedChanged += this.CustomRadioButtonCheckChanged;
			this.optionsPanel.AutoSize = true;
			this.optionsPanel.Controls.Add(this.eulaAgreeRadioButton);
			this.optionsPanel.Controls.Add(this.eulaNotAgreeRadioButton);
			this.optionsPanel.Dock = DockStyle.Bottom;
			this.optionsPanel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.optionsPanel.Location = new Point(0, 382);
			this.optionsPanel.Name = "optionsPanel";
			this.optionsPanel.Size = new Size(721, 53);
			this.optionsPanel.TabIndex = 24;
			this.spacerPanel.Dock = DockStyle.Top;
			this.spacerPanel.Location = new Point(0, 17);
			this.spacerPanel.Name = "spacerPanel";
			this.spacerPanel.Size = new Size(721, 20);
			this.spacerPanel.TabIndex = 25;
			this.spacerBottomPanel.Dock = DockStyle.Bottom;
			this.spacerBottomPanel.Location = new Point(0, 362);
			this.spacerBottomPanel.Name = "spacerBottomPanel";
			this.spacerBottomPanel.Size = new Size(721, 20);
			this.spacerBottomPanel.TabIndex = 26;
			this.titlePanel.AutoSize = true;
			this.titlePanel.Controls.Add(this.eulaLabel);
			this.titlePanel.Dock = DockStyle.Top;
			this.titlePanel.Location = new Point(0, 0);
			this.titlePanel.Name = "titlePanel";
			this.titlePanel.Size = new Size(721, 17);
			this.titlePanel.TabIndex = 27;
			base.Controls.Add(this.descriptionPanel);
			base.Controls.Add(this.outerPanel);
			base.Controls.Add(this.spacerPanel);
			base.Controls.Add(this.titlePanel);
			base.Controls.Add(this.spacerBottomPanel);
			base.Controls.Add(this.optionsPanel);
			base.Name = "EULAPage";
			base.SetActive += this.EULAPage_SetActive;
			base.CheckLoaded += this.EULAPage_CheckLoaded;
			this.outerPanel.ResumeLayout(false);
			this.optionsPanel.ResumeLayout(false);
			this.optionsPanel.PerformLayout();
			this.titlePanel.ResumeLayout(false);
			this.titlePanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private readonly bool isLanguagePackOperation;

		private CustomRichTextBox eulaDisplayRichTextBox;

		private Panel outerPanel;

		private CustomRadiobutton eulaAgreeRadioButton;

		private CustomRadiobutton eulaNotAgreeRadioButton;

		private IContainer components;

		private string licenseFile;

		private Label eulaLabel;

		private Panel descriptionPanel;

		private Panel optionsPanel;

		private Panel spacerPanel;

		private Panel spacerBottomPanel;

		private Panel titlePanel;

		private RootDataHandler rdh;
	}
}
