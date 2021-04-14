using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class RoleSelectionPage : SetupWizardPage
	{
		public RoleSelectionPage(RootDataHandler rootDataHandler)
		{
			this.previousSelection = false;
			this.modeDataHandler = rootDataHandler.ModeDatahandler;
			this.InitializeComponent();
			base.PageTitle = Strings.RoleSelectionPageTitle;
			this.roleSelectionLabel.Text = Strings.RoleSelectionLabelText;
			this.installWindowsPrereqCheckBox.Text = Strings.InstallWindowsPrereqCheckBoxText;
			this.mailboxRoleCheckBox.Text = Strings.MailboxRole;
			this.clientAccessRoleCheckBox.Text = Strings.ClientAccessRole;
			this.managementToolsCheckBox.Text = Strings.AdminToolsRole;
			this.edgeRoleCheckBox.Text = Strings.EdgeRole;
			this.mailboxRoleCheckBox.Checked = false;
			this.clientAccessRoleCheckBox.Checked = false;
			this.edgeRoleCheckBox.Checked = false;
			this.installWindowsPrereqCheckBox.Checked = false;
			this.mailboxRoleCheckBox.CheckedChanged += delegate(object param0, EventArgs param1)
			{
				this.OnIsMailboxCheckedChanged(EventArgs.Empty);
			};
			this.clientAccessRoleCheckBox.CheckedChanged += delegate(object param0, EventArgs param1)
			{
				this.OnIsClientAccessCheckedChanged(EventArgs.Empty);
			};
			this.edgeRoleCheckBox.CheckedChanged += delegate(object param0, EventArgs param1)
			{
				this.OnIsEdgeRoleCheckedChanged(EventArgs.Empty);
			};
			this.managementToolsCheckBox.CheckedChanged += delegate(object param0, EventArgs param1)
			{
				this.OnIsManagementToolsCheckedChanged(EventArgs.Empty);
			};
			this.IsMailboxCheckedChanged += this.MailboxRole_Checked;
			this.IsClientAccessCheckedChanged += this.ClientAccessRole_Checked;
			this.IsEdgeCheckedChanged += this.EdgeRole_Checked;
			this.IsManagementToolsCheckedChanged += this.ManagementTools_Checked;
			this.installWindowsPrereqCheckBox.CheckedChanged += delegate(object param0, EventArgs param1)
			{
				this.OnIsInstallWindowsPrereqCheckedChanged(EventArgs.Empty);
			};
			this.IsInstallWindowsPrereqCheckedChanged += this.InstallWindowsPrereq_Checked;
			base.WizardCancel += this.RoleSelectionPage_WizardCancel;
		}

		internal event EventHandler IsMailboxCheckedChanged;

		internal event EventHandler IsClientAccessCheckedChanged;

		internal event EventHandler IsEdgeCheckedChanged;

		internal event EventHandler IsManagementToolsCheckedChanged;

		internal event EventHandler IsInstallWindowsPrereqCheckedChanged;

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void RoleSelectionPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			base.SetWizardButtons(3);
			base.SetVisibleWizardButtons(3);
			this.managementToolsCheckBox.Checked = true;
			if (this.previousSelection)
			{
				this.GetPreviousSelection();
			}
			else
			{
				this.mailboxRoleCheckBox.Checked = false;
				this.clientAccessRoleCheckBox.Checked = false;
				this.edgeRoleCheckBox.Checked = false;
				if (SetupHelper.IsClientVersionOS())
				{
					base.DisableCustomCheckbox(this.installWindowsPrereqCheckBox, true);
				}
				else
				{
					this.installWindowsPrereqCheckBox.Checked = true;
				}
			}
			this.previousSelection = true;
			base.EnableCheckLoadedTimer(200);
		}

		private void InsertProtectionSettingPage()
		{
			this.protectionSettingPage = (ProtectionSettingsPage)base.FindPage("ProtectionSettingsPage");
			if (this.protectionSettingPage == null)
			{
				this.protectionSettingPage = new ProtectionSettingsPage((InstallModeDataHandler)this.modeDataHandler);
				string text = "PreCheckPage";
				base.InsertPage(this.protectionSettingPage, base.FindPage(text));
			}
		}

		private void RemoveProtectionSettingPage()
		{
			this.protectionSettingPage = (ProtectionSettingsPage)base.FindPage("ProtectionSettingsPage");
			if (this.protectionSettingPage != null)
			{
				base.RemovePage(this.protectionSettingPage);
				this.protectionSettingPage = null;
			}
		}

		private void InsertExchangeOrganizationPage()
		{
			this.exchangeOrganizationPage = (ExchangeOrganizationPage)base.FindPage("ExchangeOrganizationPage");
			if (this.exchangeOrganizationPage == null)
			{
				this.exchangeOrganizationPage = new ExchangeOrganizationPage((InstallModeDataHandler)this.modeDataHandler);
				string text = "PreCheckPage";
				base.InsertPage(this.exchangeOrganizationPage, base.FindPage(text));
			}
		}

		private void RemoveExchangeOrganizationPage()
		{
			this.exchangeOrganizationPage = (ExchangeOrganizationPage)base.FindPage("ExchangeOrganizationPage");
			if (this.exchangeOrganizationPage != null)
			{
				base.RemovePage(this.exchangeOrganizationPage);
				this.exchangeOrganizationPage = null;
			}
		}

		private void GetPreviousSelection()
		{
			if (this.modeDataHandler.IsMailboxChecked && this.modeDataHandler.IsBridgeheadChecked && this.modeDataHandler.IsUnifiedMessagingChecked)
			{
				this.mailboxRoleCheckBox.Checked = true;
				this.InsertProtectionSettingPage();
			}
			else
			{
				this.mailboxRoleCheckBox.Checked = false;
				this.RemoveProtectionSettingPage();
			}
			if (this.modeDataHandler.IsCafeChecked && this.modeDataHandler.IsFrontendTransportChecked)
			{
				this.clientAccessRoleCheckBox.Checked = true;
			}
			else
			{
				this.clientAccessRoleCheckBox.Checked = false;
			}
			if (SetupHelper.IsClientVersionOS())
			{
				base.DisableCustomCheckbox(this.installWindowsPrereqCheckBox, true);
				return;
			}
			if (this.modeDataHandler.InstallWindowsComponents)
			{
				this.installWindowsPrereqCheckBox.Checked = true;
				return;
			}
			this.installWindowsPrereqCheckBox.Checked = false;
		}

		private void RoleSelectionPage_WizardCancel(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void RoleSelectionPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.installWindowsPrereqCheckBox.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void OnIsMailboxCheckedChanged(EventArgs e)
		{
			if (this.IsMailboxCheckedChanged != null)
			{
				this.IsMailboxCheckedChanged(this, e);
			}
		}

		private void OnIsClientAccessCheckedChanged(EventArgs e)
		{
			if (this.IsClientAccessCheckedChanged != null)
			{
				this.IsClientAccessCheckedChanged(this, e);
			}
		}

		private void OnIsEdgeRoleCheckedChanged(EventArgs e)
		{
			if (this.IsEdgeCheckedChanged != null)
			{
				this.IsEdgeCheckedChanged(this, e);
			}
		}

		private void OnIsManagementToolsCheckedChanged(EventArgs e)
		{
			if (this.IsManagementToolsCheckedChanged != null)
			{
				this.IsManagementToolsCheckedChanged(this, e);
			}
		}

		private void OnIsInstallWindowsPrereqCheckedChanged(EventArgs e)
		{
			if (this.IsInstallWindowsPrereqCheckedChanged != null)
			{
				this.IsInstallWindowsPrereqCheckedChanged(this, e);
			}
		}

		private void MailboxRole_Checked(object sender, EventArgs e)
		{
			if (this.mailboxRoleCheckBox.Checked)
			{
				this.modeDataHandler.IsMailboxChecked = true;
				this.modeDataHandler.IsClientAccessChecked = true;
				this.modeDataHandler.IsBridgeheadChecked = true;
				this.modeDataHandler.IsUnifiedMessagingChecked = true;
				this.modeDataHandler.IsGatewayChecked = false;
				this.edgeRoleCheckBox.Enabled = false;
				this.InsertProtectionSettingPage();
				return;
			}
			this.modeDataHandler.IsMailboxChecked = false;
			this.modeDataHandler.IsClientAccessChecked = false;
			this.modeDataHandler.IsBridgeheadChecked = false;
			this.modeDataHandler.IsUnifiedMessagingChecked = false;
			if (!this.clientAccessRoleCheckBox.Checked)
			{
				this.edgeRoleCheckBox.Enabled = true;
			}
			this.RemoveProtectionSettingPage();
		}

		private void ClientAccessRole_Checked(object sender, EventArgs e)
		{
			if (this.clientAccessRoleCheckBox.Checked)
			{
				this.modeDataHandler.IsCafeChecked = true;
				this.modeDataHandler.IsFrontendTransportChecked = true;
				this.modeDataHandler.IsGatewayChecked = false;
				this.edgeRoleCheckBox.Enabled = false;
				return;
			}
			this.modeDataHandler.IsCafeChecked = false;
			this.modeDataHandler.IsFrontendTransportChecked = false;
			if (!this.mailboxRoleCheckBox.Checked)
			{
				this.edgeRoleCheckBox.Enabled = true;
			}
		}

		private void EdgeRole_Checked(object sender, EventArgs e)
		{
			if (this.edgeRoleCheckBox.Checked)
			{
				this.modeDataHandler.IsGatewayChecked = true;
				this.modeDataHandler.IsMailboxChecked = false;
				this.modeDataHandler.IsClientAccessChecked = false;
				this.modeDataHandler.IsBridgeheadChecked = false;
				this.modeDataHandler.IsUnifiedMessagingChecked = false;
				this.modeDataHandler.IsCafeChecked = false;
				this.modeDataHandler.IsFrontendTransportChecked = false;
				this.mailboxRoleCheckBox.Enabled = false;
				this.clientAccessRoleCheckBox.Enabled = false;
				this.RemoveExchangeOrganizationPage();
				return;
			}
			this.modeDataHandler.IsGatewayChecked = false;
			this.mailboxRoleCheckBox.Enabled = true;
			this.clientAccessRoleCheckBox.Enabled = true;
			this.InsertExchangeOrganizationPage();
		}

		private void ManagementTools_Checked(object sender, EventArgs e)
		{
			this.managementToolsCheckBox.Enabled = false;
			this.modeDataHandler.IsAdminToolsChecked = true;
		}

		private void InstallWindowsPrereq_Checked(object sender, EventArgs e)
		{
			if (this.installWindowsPrereqCheckBox.Checked)
			{
				this.modeDataHandler.InstallWindowsComponents = true;
				return;
			}
			this.modeDataHandler.InstallWindowsComponents = false;
		}

		private void InitializeComponent()
		{
			this.roleSelectionLabel = new Label();
			this.mailboxRoleCheckBox = new CustomCheckbox();
			this.clientAccessRoleCheckBox = new CustomCheckbox();
			this.managementToolsCheckBox = new CustomCheckbox();
			this.installWindowsPrereqCheckBox = new CustomCheckbox();
			this.edgeRoleCheckBox = new CustomCheckbox();
			base.SuspendLayout();
			this.roleSelectionLabel.AutoSize = true;
			this.roleSelectionLabel.BackColor = Color.Transparent;
			this.roleSelectionLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.roleSelectionLabel.Location = new Point(0, 0);
			this.roleSelectionLabel.MaximumSize = new Size(740, 0);
			this.roleSelectionLabel.Name = "roleSelectionLabel";
			this.roleSelectionLabel.Size = new Size(108, 15);
			this.roleSelectionLabel.TabIndex = 18;
			this.roleSelectionLabel.Text = "[RoleSelectionText]";
			this.mailboxRoleCheckBox.BackColor = Color.Transparent;
			this.mailboxRoleCheckBox.Checked = false;
			this.mailboxRoleCheckBox.DisabledColor = SetupWizardPage.DefaultDisabledColor;
			this.mailboxRoleCheckBox.ForeColor = SetupWizardPage.DefaultNormalColor;
			this.mailboxRoleCheckBox.HighlightedColor = SetupWizardPage.DefaultHighlightColor;
			this.mailboxRoleCheckBox.Highligted = false;
			this.mailboxRoleCheckBox.Location = new Point(0, 25);
			this.mailboxRoleCheckBox.Name = "mailboxRoleCheckBox";
			this.mailboxRoleCheckBox.NormalColor = SetupWizardPage.DefaultNormalColor;
			this.mailboxRoleCheckBox.Size = new Size(700, 19);
			this.mailboxRoleCheckBox.TabIndex = 23;
			this.mailboxRoleCheckBox.Text = "[MailboxRoleCheckBoxText]";
			this.mailboxRoleCheckBox.TextGap = 10;
			this.clientAccessRoleCheckBox.BackColor = Color.Transparent;
			this.clientAccessRoleCheckBox.Checked = false;
			this.clientAccessRoleCheckBox.DisabledColor = SetupWizardPage.DefaultDisabledColor;
			this.clientAccessRoleCheckBox.ForeColor = SetupWizardPage.DefaultNormalColor;
			this.clientAccessRoleCheckBox.HighlightedColor = SetupWizardPage.DefaultHighlightColor;
			this.clientAccessRoleCheckBox.Highligted = false;
			this.clientAccessRoleCheckBox.Location = new Point(0, 54);
			this.clientAccessRoleCheckBox.Name = "clientAccessRoleCheckBox";
			this.clientAccessRoleCheckBox.NormalColor = SetupWizardPage.DefaultNormalColor;
			this.clientAccessRoleCheckBox.Size = new Size(700, 19);
			this.clientAccessRoleCheckBox.TabIndex = 24;
			this.clientAccessRoleCheckBox.Text = "[ClientAccessRoleCheckBoxText]";
			this.clientAccessRoleCheckBox.TextGap = 10;
			this.managementToolsCheckBox.BackColor = Color.Transparent;
			this.managementToolsCheckBox.Checked = false;
			this.managementToolsCheckBox.DisabledColor = SetupWizardPage.DefaultDisabledColor;
			this.managementToolsCheckBox.ForeColor = SetupWizardPage.DefaultNormalColor;
			this.managementToolsCheckBox.HighlightedColor = SetupWizardPage.DefaultHighlightColor;
			this.managementToolsCheckBox.Highligted = false;
			this.managementToolsCheckBox.Location = new Point(0, 83);
			this.managementToolsCheckBox.Name = "managementToolsCheckBox";
			this.managementToolsCheckBox.NormalColor = SetupWizardPage.DefaultNormalColor;
			this.managementToolsCheckBox.Size = new Size(700, 19);
			this.managementToolsCheckBox.TabIndex = 25;
			this.managementToolsCheckBox.Text = "[ManagementToolsCheckBoxText]";
			this.managementToolsCheckBox.TextGap = 10;
			this.installWindowsPrereqCheckBox.BackColor = Color.Transparent;
			this.installWindowsPrereqCheckBox.Checked = false;
			this.installWindowsPrereqCheckBox.DisabledColor = SetupWizardPage.DefaultDisabledColor;
			this.installWindowsPrereqCheckBox.ForeColor = SetupWizardPage.DefaultNormalColor;
			this.installWindowsPrereqCheckBox.HighlightedColor = SetupWizardPage.DefaultHighlightColor;
			this.installWindowsPrereqCheckBox.Highligted = false;
			this.installWindowsPrereqCheckBox.Location = new Point(0, 146);
			this.installWindowsPrereqCheckBox.Name = "installWindowsPrereqCheckBox";
			this.installWindowsPrereqCheckBox.NormalColor = SetupWizardPage.DefaultNormalColor;
			this.installWindowsPrereqCheckBox.Size = new Size(700, 19);
			this.installWindowsPrereqCheckBox.TabIndex = 26;
			this.installWindowsPrereqCheckBox.Text = "[installWindowsPrereqText]";
			this.installWindowsPrereqCheckBox.TextGap = 10;
			this.edgeRoleCheckBox.BackColor = Color.Transparent;
			this.edgeRoleCheckBox.Checked = false;
			this.edgeRoleCheckBox.DisabledColor = SetupWizardPage.DefaultDisabledColor;
			this.edgeRoleCheckBox.ForeColor = SetupWizardPage.DefaultNormalColor;
			this.edgeRoleCheckBox.HighlightedColor = SetupWizardPage.DefaultHighlightColor;
			this.edgeRoleCheckBox.Highligted = false;
			this.edgeRoleCheckBox.Location = new Point(0, 112);
			this.edgeRoleCheckBox.Name = "edgeRoleCheckBox";
			this.edgeRoleCheckBox.NormalColor = SetupWizardPage.DefaultNormalColor;
			this.edgeRoleCheckBox.Size = new Size(700, 19);
			this.edgeRoleCheckBox.TabIndex = 27;
			this.edgeRoleCheckBox.Text = "[EdgeRoleCheckBoxText]";
			this.edgeRoleCheckBox.TextGap = 10;
			base.Controls.Add(this.edgeRoleCheckBox);
			base.Controls.Add(this.installWindowsPrereqCheckBox);
			base.Controls.Add(this.managementToolsCheckBox);
			base.Controls.Add(this.clientAccessRoleCheckBox);
			base.Controls.Add(this.mailboxRoleCheckBox);
			base.Controls.Add(this.roleSelectionLabel);
			base.Name = "RoleSelectionPage";
			base.SetActive += this.RoleSelectionPage_SetActive;
			base.CheckLoaded += this.RoleSelectionPage_CheckLoaded;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private readonly ModeDataHandler modeDataHandler;

		private IContainer components;

		private CustomCheckbox clientAccessRoleCheckBox;

		private CustomCheckbox mailboxRoleCheckBox;

		private CustomCheckbox edgeRoleCheckBox;

		private CustomCheckbox managementToolsCheckBox;

		private CustomCheckbox installWindowsPrereqCheckBox;

		private Label roleSelectionLabel;

		private bool previousSelection;

		private ProtectionSettingsPage protectionSettingPage;

		private ExchangeOrganizationPage exchangeOrganizationPage;
	}
}
