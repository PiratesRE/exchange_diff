using System;
using System.ComponentModel;
using System.Drawing;
using System.Management.Automation;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class HybridConfigurationStatusPage : SetupWizardPage
	{
		public HybridConfigurationStatusPage(IHybridConfigurationDetection hybridConfigurationDetectionProvider)
		{
			this.hybridConfigurationDetectionProvider = (HybridConfigurationDetection)hybridConfigurationDetectionProvider;
			this.InitializeComponent();
			base.Name = "HybridConfigurationStatusPage";
			base.PageTitle = Strings.HybridConfigurationStatusPageTitle;
			base.SetActive += this.HybridConfigurationStatusPage_SetActive;
			base.CheckLoaded += this.HybridConfigurationStatusPage_CheckLoaded;
			base.WizardCancel += this.HybridConfigurationStatusPage_WizardCancel;
			base.WizardFailed += this.HybridConfigurationStatusPage_WizardFailed;
		}

		internal PSCredential Credential { get; set; }

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void StartHybridTest()
		{
			try
			{
				this.UpdateHybridTestStatus(Strings.HybridConfigurationCredentialsChecks);
				if (this.Credential != null)
				{
					bool flag = this.hybridConfigurationDetectionProvider.RunTenantHybridTest(this.Credential, string.Empty);
					if (flag)
					{
						this.UpdateHybridTestStatus(Strings.HybridConfigurationCredentialsFinished);
						base.SetWizardButtons(3);
						base.SetExitFlag(false);
						return;
					}
					this.UpdateHybridTestStatus(Strings.HybridConfigurationCredentialsFailed);
				}
				else
				{
					this.UpdateHybridTestStatus(Strings.InvalidCredentials);
				}
				this.SetExitButton();
			}
			catch (Exception message)
			{
				this.UpdateHybridTestStatus(message);
				this.SetExitButton();
			}
		}

		private void SetExitButton()
		{
			base.SetExitFlag(true);
			base.SetBtnNextText(Strings.btnExit);
			base.SetWizardButtons(3);
		}

		private void UpdateHybridTestStatus(object message)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new HybridConfigurationStatusPage.UpdateStatusBox(this.UpdateHybridTestStatus), new object[]
				{
					message
				});
				return;
			}
			string text = null;
			if (message is LocalizedException)
			{
				LocalizedException ex = (LocalizedException)message;
				text = ex.LocalizedString.ToString();
				SetupLogger.Log(new LocalizedString(message.ToString()));
			}
			else if (message is ILocalizedString)
			{
				text = message.ToString();
				SetupLogger.Log(new LocalizedString(text));
			}
			else if (message is HybridConfigurationDetectionException)
			{
				HybridConfigurationDetectionException ex2 = (HybridConfigurationDetectionException)message;
				SetupLogger.LogError(ex2);
				text = ex2.Message;
			}
			else if (message is Exception)
			{
				Exception e = (Exception)message;
				text = Strings.HybridConfigurationSystemExceptionText;
				SetupLogger.LogError(e);
			}
			this.progressLabel.Text = text;
		}

		private void HybridConfigurationStatusPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			base.SetWizardButtons(0);
			base.SetVisibleWizardButtons(3);
			base.SetExitFlag(false);
			base.SetBtnNextText(Strings.btnNext);
			base.EnableCheckLoadedTimer(200);
			this.StartHybridTest();
		}

		private void HybridConfigurationStatusPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.progressLabel.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void HybridConfigurationStatusPage_WizardCancel(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void HybridConfigurationStatusPage_WizardFailed(object sender, CancelEventArgs e)
		{
			ExSetupUI.ExitApplication(ExitCode.Error);
		}

		private void InitializeComponent()
		{
			this.progressLabel = new Label();
			base.SuspendLayout();
			this.progressLabel.AutoSize = true;
			this.progressLabel.FlatStyle = FlatStyle.Flat;
			this.progressLabel.Location = new Point(0, 0);
			this.progressLabel.MaximumSize = new Size(720, 0);
			this.progressLabel.Name = "progressLabel";
			this.progressLabel.Size = new Size(123, 17);
			this.progressLabel.TabIndex = 0;
			this.progressLabel.Text = "[ProgressLabelText]";
			base.Controls.Add(this.progressLabel);
			base.Name = "HybridConfigurationStatusPage";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private IContainer components;

		private Label progressLabel;

		private HybridConfigurationDetection hybridConfigurationDetectionProvider;

		private delegate void UpdateStatusBox(object msg);
	}
}
