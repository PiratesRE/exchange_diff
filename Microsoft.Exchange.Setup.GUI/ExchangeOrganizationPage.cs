using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class ExchangeOrganizationPage : SetupWizardPage
	{
		public ExchangeOrganizationPage(InstallModeDataHandler installModeDataHandler)
		{
			this.installOrgCfgDataHandler = installModeDataHandler.InstallOrgCfgDataHandler;
			this.InitializeComponent();
			base.PageTitle = Strings.ExchangeOrganizationPageTitle;
			this.exchangeOrgNameLabel.Text = Strings.ExchangeOrganizationName;
			this.adSplitPermissionCheckBox.Text = Strings.ActiveDirectorySplitPermissions;
			this.adSplitPermissionsDescripitionLabel.Text = Strings.ActiveDirectorySplitPermissionsDescription;
			IOrganizationName organizationName = this.installOrgCfgDataHandler.OrganizationName;
			this.exchangeOrgNameTextBox.Text = ((organizationName == null) ? string.Empty : organizationName.EscapedName);
			this.adSplitPermissionCheckBox.CheckedChanged += delegate(object param0, EventArgs param1)
			{
				this.OnIsADSplitPermissionCheckedChanged(EventArgs.Empty);
			};
			this.IsADSplitPermissionCheckedChanged += this.ADSplitPermission_Checked;
			this.exchangeOrgNameTextBox.Leave += this.ExchangeOrgName_Leave;
			this.exchangeOrgNameTextBox.Enter += this.ExchangeOrgName_Enter;
			base.WizardCancel += this.ExchangeOrganizationPage_WizardCancel;
		}

		internal event EventHandler IsADSplitPermissionCheckedChanged;

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void ExchangeOrganizationPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			base.SetWizardButtons(3);
			base.SetVisibleWizardButtons(3);
			this.adSplitPermissionCheckBox.Checked = false;
			base.EnableCheckLoadedTimer(200);
		}

		private void ExchangeOrganizationPage_WizardCancel(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void ExchangeOrganizationPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.adSplitPermissionsDescripitionLabel.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void ADSplitPermission_Checked(object sender, EventArgs e)
		{
			if (this.adSplitPermissionCheckBox.Checked)
			{
				this.installOrgCfgDataHandler.ActiveDirectorySplitPermissions = new bool?(true);
			}
			else
			{
				this.installOrgCfgDataHandler.ActiveDirectorySplitPermissions = new bool?(false);
			}
			if (this.newOrgName == null)
			{
				MessageBoxHelper.ShowError(Strings.ExchangeOrganizationNameError);
				base.SetWizardButtons(0);
			}
		}

		private void ExchangeOrgName_Leave(object sender, EventArgs e)
		{
			this.newOrgName = this.SetOrganizationName();
			if (this.newOrgName == null)
			{
				MessageBoxHelper.ShowError(Strings.ExchangeOrganizationNameError);
				base.SetWizardButtons(0);
				return;
			}
			this.installOrgCfgDataHandler.OrganizationName = this.newOrgName;
		}

		private NewOrganizationName SetOrganizationName()
		{
			NewOrganizationName result = null;
			if (!string.IsNullOrEmpty(this.exchangeOrgNameTextBox.Text))
			{
				try
				{
					result = new NewOrganizationName(this.exchangeOrgNameTextBox.Text);
				}
				catch (FormatException)
				{
				}
			}
			return result;
		}

		private void ExchangeOrgName_Enter(object sender, EventArgs e)
		{
			base.SetWizardButtons(3);
		}

		private void OnIsADSplitPermissionCheckedChanged(EventArgs e)
		{
			if (this.IsADSplitPermissionCheckedChanged != null)
			{
				this.IsADSplitPermissionCheckedChanged(this, e);
			}
		}

		private void InitializeComponent()
		{
			this.exchangeOrgNameLabel = new Label();
			this.exchangeOrgNameTextBox = new TextBox();
			this.adSplitPermissionCheckBox = new CustomCheckbox();
			this.adSplitPermissionsDescripitionLabel = new Label();
			base.SuspendLayout();
			this.exchangeOrgNameLabel.AutoSize = true;
			this.exchangeOrgNameLabel.BackColor = Color.Transparent;
			this.exchangeOrgNameLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.exchangeOrgNameLabel.Location = new Point(0, 0);
			this.exchangeOrgNameLabel.Margin = new Padding(3);
			this.exchangeOrgNameLabel.MaximumSize = new Size(720, 0);
			this.exchangeOrgNameLabel.Name = "exchangeOrgNameLabel";
			this.exchangeOrgNameLabel.Size = new Size(187, 15);
			this.exchangeOrgNameLabel.TabIndex = 19;
			this.exchangeOrgNameLabel.Text = "[ExchangeOrganizationNameText]";
			this.exchangeOrgNameTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.exchangeOrgNameTextBox.BorderStyle = BorderStyle.FixedSingle;
			this.exchangeOrgNameTextBox.ForeColor = Color.FromArgb(51, 51, 51);
			this.exchangeOrgNameTextBox.Location = new Point(3, 21);
			this.exchangeOrgNameTextBox.Name = "exchangeOrgNameTextBox";
			this.exchangeOrgNameTextBox.Size = new Size(670, 23);
			this.exchangeOrgNameTextBox.TabIndex = 20;
			this.adSplitPermissionCheckBox.BackColor = Color.Transparent;
			this.adSplitPermissionCheckBox.Checked = false;
			this.adSplitPermissionCheckBox.DisabledColor = SetupWizardPage.DefaultDisabledColor;
			this.adSplitPermissionCheckBox.ForeColor = SetupWizardPage.DefaultNormalColor;
			this.adSplitPermissionCheckBox.HighlightedColor = SetupWizardPage.DefaultHighlightColor;
			this.adSplitPermissionCheckBox.Highligted = false;
			this.adSplitPermissionCheckBox.Location = new Point(3, 73);
			this.adSplitPermissionCheckBox.Name = "adSplitPermissionCheckBox";
			this.adSplitPermissionCheckBox.NormalColor = SetupWizardPage.DefaultNormalColor;
			this.adSplitPermissionCheckBox.Size = new Size(670, 19);
			this.adSplitPermissionCheckBox.TabIndex = 23;
			this.adSplitPermissionCheckBox.Text = "[ADSplitPermissionCheckBox]";
			this.adSplitPermissionCheckBox.TextGap = 10;
			this.adSplitPermissionsDescripitionLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.adSplitPermissionsDescripitionLabel.AutoSize = true;
			this.adSplitPermissionsDescripitionLabel.BackColor = Color.Transparent;
			this.adSplitPermissionsDescripitionLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
			this.adSplitPermissionsDescripitionLabel.Location = new Point(0, 102);
			this.adSplitPermissionsDescripitionLabel.MaximumSize = new Size(720, 0);
			this.adSplitPermissionsDescripitionLabel.Name = "adSplitPermissionsDescripitionLabel";
			this.adSplitPermissionsDescripitionLabel.Size = new Size(208, 15);
			this.adSplitPermissionsDescripitionLabel.TabIndex = 22;
			this.adSplitPermissionsDescripitionLabel.Text = "[ADSplitPermissionsDescripitionLabel]";
			base.Controls.Add(this.adSplitPermissionsDescripitionLabel);
			base.Controls.Add(this.adSplitPermissionCheckBox);
			base.Controls.Add(this.exchangeOrgNameTextBox);
			base.Controls.Add(this.exchangeOrgNameLabel);
			base.Name = "ExchangeOrganizationPage";
			base.SetActive += this.ExchangeOrganizationPage_SetActive;
			base.CheckLoaded += this.ExchangeOrganizationPage_CheckLoaded;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private Label exchangeOrgNameLabel;

		private TextBox exchangeOrgNameTextBox;

		private CustomCheckbox adSplitPermissionCheckBox;

		private Label adSplitPermissionsDescripitionLabel;

		private InstallOrgCfgDataHandler installOrgCfgDataHandler;

		private IContainer components;

		private NewOrganizationName newOrgName;
	}
}
