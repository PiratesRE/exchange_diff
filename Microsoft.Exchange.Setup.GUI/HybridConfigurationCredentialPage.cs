using System;
using System.ComponentModel;
using System.Drawing;
using System.Management.Automation;
using System.Security;
using System.Windows.Forms;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class HybridConfigurationCredentialPage : SetupWizardPage
	{
		public HybridConfigurationCredentialPage()
		{
			this.InitializeComponent();
			base.PageTitle = Strings.HybridConfigurationCredentialPageTitle;
			this.enterCredentialLabel.Text = Strings.HybridConfigurationEnterCredentialLabelText;
			this.enterCredentialForAccountLabel.Text = Strings.HybridConfigurationEnterCredentialForAccountLabelText;
			this.userNameLabel.Text = Strings.UserNameLabelText;
			this.passwordLabel.Text = Strings.PasswordLabelText;
			this.userNameTextBox.Text = string.Empty;
			this.passwordTextBox.Text = string.Empty;
			this.passwordTextBox.SecureText.Clear();
			this.passwordTextBox.UseSystemPasswordChar = true;
			base.WizardCancel += this.HybridConfigurationCredentialPage_WizardCancel;
			base.WizardNext += new WizardPageEventHandler(this.HybridConfigurationCredentialPage_Next);
			this.passwordTextBox.TextChanged += this.Password_TextChanged;
			this.userNameTextBox.TextChanged += this.UserName_TextChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void HybridConfigurationCredentialPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			base.SetWizardButtons(1);
			base.SetVisibleWizardButtons(3);
			base.SetBtnNextText(Strings.btnNext);
			this.passwordTextBox.Text = string.Empty;
			this.passwordTextBox.SecureText.Clear();
			base.EnableCheckLoadedTimer(200);
		}

		private void HybridConfigurationCredentialPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.passwordLabel.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void HybridConfigurationCredentialPage_WizardCancel(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void UserName_TextChanged(object sender, EventArgs e)
		{
			this.EnableButtons();
		}

		private void Password_TextChanged(object sender, EventArgs e)
		{
			this.EnableButtons();
		}

		private void EnableButtons()
		{
			if (!string.IsNullOrEmpty(this.passwordTextBox.Text) && !string.IsNullOrEmpty(this.userNameTextBox.Text))
			{
				base.SetWizardButtons(3);
			}
		}

		private void HybridConfigurationCredentialPage_Next(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.passwordTextBox.Text) && !string.IsNullOrEmpty(this.userNameTextBox.Text))
			{
				try
				{
					PSCredential credential = new PSCredential(this.userNameTextBox.Text, this.passwordTextBox.SecureText);
					HybridConfigurationStatusPage hybridConfigurationStatusPage = base.FindPage("HybridConfigurationStatusPage") as HybridConfigurationStatusPage;
					hybridConfigurationStatusPage.Credential = credential;
					return;
				}
				catch (PSArgumentException)
				{
					MessageBoxHelper.ShowError(Strings.InvalidCredentials);
					return;
				}
			}
			MessageBoxHelper.ShowError(Strings.InvalidCredentials);
		}

		private void InitializeComponent()
		{
			SecureString secureText = new SecureString();
			this.enterCredentialLabel = new Label();
			this.passwordTextBox = new SecureTextBox();
			this.passwordLabel = new Label();
			this.userNameLabel = new Label();
			this.userNameTextBox = new TextBox();
			this.enterCredentialForAccountLabel = new Label();
			base.SuspendLayout();
			this.enterCredentialLabel.AutoSize = true;
			this.enterCredentialLabel.BackColor = Color.Transparent;
			this.enterCredentialLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.enterCredentialLabel.Location = new Point(0, 0);
			this.enterCredentialLabel.Margin = new Padding(1, 0, 1, 0);
			this.enterCredentialLabel.MaximumSize = new Size(720, 0);
			this.enterCredentialLabel.Name = "enterCredentialLabel";
			this.enterCredentialLabel.Size = new Size(129, 17);
			this.enterCredentialLabel.TabIndex = 18;
			this.enterCredentialLabel.Text = "[EnterCredentialText]";
			this.passwordTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.passwordTextBox.BorderStyle = BorderStyle.FixedSingle;
			this.passwordTextBox.ForeColor = Color.FromArgb(51, 51, 51);
			this.passwordTextBox.Location = new Point(171, 145);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.SecureText = secureText;
			this.passwordTextBox.Size = new Size(240, 23);
			this.passwordTextBox.TabIndex = 28;
			this.passwordLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.passwordLabel.AutoSize = true;
			this.passwordLabel.BackColor = Color.Transparent;
			this.passwordLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.passwordLabel.ForeColor = Color.FromArgb(102, 102, 102);
			this.passwordLabel.Location = new Point(0, 145);
			this.passwordLabel.MaximumSize = new Size(420, 0);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new Size(127, 17);
			this.passwordLabel.TabIndex = 27;
			this.passwordLabel.Text = "[PasswordLabelText]";
			this.userNameLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.userNameLabel.AutoSize = true;
			this.userNameLabel.BackColor = Color.Transparent;
			this.userNameLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.userNameLabel.ForeColor = Color.FromArgb(102, 102, 102);
			this.userNameLabel.Location = new Point(0, 112);
			this.userNameLabel.MaximumSize = new Size(420, 0);
			this.userNameLabel.Name = "userNameLabel";
			this.userNameLabel.Size = new Size(133, 17);
			this.userNameLabel.TabIndex = 25;
			this.userNameLabel.Text = "[UserNameLabelText]";
			this.userNameTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.userNameTextBox.BorderStyle = BorderStyle.FixedSingle;
			this.userNameTextBox.ForeColor = Color.FromArgb(51, 51, 51);
			this.userNameTextBox.Location = new Point(171, 112);
			this.userNameTextBox.Margin = new Padding(3, 3, 0, 0);
			this.userNameTextBox.Name = "userNameTextBox";
			this.userNameTextBox.Size = new Size(240, 23);
			this.userNameTextBox.TabIndex = 24;
			this.enterCredentialForAccountLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.enterCredentialForAccountLabel.AutoSize = true;
			this.enterCredentialForAccountLabel.BackColor = Color.Transparent;
			this.enterCredentialForAccountLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.enterCredentialForAccountLabel.Location = new Point(0, 80);
			this.enterCredentialForAccountLabel.Margin = new Padding(1, 0, 1, 0);
			this.enterCredentialForAccountLabel.MaximumSize = new Size(720, 0);
			this.enterCredentialForAccountLabel.Name = "enterCredentialForAccountLabel";
			this.enterCredentialForAccountLabel.Size = new Size(225, 17);
			this.enterCredentialForAccountLabel.TabIndex = 23;
			this.enterCredentialForAccountLabel.Text = "[EnterCredentialForAccountLabelText]";
			base.Controls.Add(this.passwordTextBox);
			base.Controls.Add(this.passwordLabel);
			base.Controls.Add(this.enterCredentialLabel);
			base.Controls.Add(this.userNameLabel);
			base.Controls.Add(this.userNameTextBox);
			base.Controls.Add(this.enterCredentialForAccountLabel);
			base.Name = "HybridConfigurationCredentialPage";
			base.SetActive += this.HybridConfigurationCredentialPage_SetActive;
			base.CheckLoaded += this.HybridConfigurationCredentialPage_CheckLoaded;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private Label enterCredentialLabel;

		private TextBox userNameTextBox;

		private Label enterCredentialForAccountLabel;

		private Label userNameLabel;

		private Label passwordLabel;

		private SecureTextBox passwordTextBox;

		private IContainer components;
	}
}
