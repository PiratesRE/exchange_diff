using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CompletionWizardPage : TridentsWizardPage
	{
		public CompletionWizardPage()
		{
			this.InitializeComponent();
			this.Text = Strings.WizardCompletionTitleText;
		}

		private void InitializeComponent()
		{
			((ISupportInitialize)base.BindingSource).BeginInit();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			base.WorkUnitsPanel.TaskState = 2;
			this.CanCancel = false;
			base.Name = "CompletionWizardPage";
			((ISupportInitialize)base.BindingSource).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		[DefaultValue(false)]
		public new bool CanCancel
		{
			get
			{
				return base.CanCancel;
			}
			set
			{
				base.CanCancel = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Padding Padding
		{
			get
			{
				return base.Padding;
			}
			set
			{
				base.Padding = value;
			}
		}

		protected override void OnSetActive(EventArgs e)
		{
			base.OnSetActive(e);
			if (base.DataHandler != null)
			{
				base.WorkUnits = base.DataHandler.WorkUnits;
				base.ElapsedTimeText = base.WorkUnits.ElapsedTimeText;
				bool flag = true;
				if (base.Wizard != null && base.Wizard.CurrentPageIndex - 1 >= 0)
				{
					ProgressTridentsWizardPage progressTridentsWizardPage = base.Wizard.WizardPages[base.Wizard.CurrentPageIndex - 1] as ProgressTridentsWizardPage;
					if (progressTridentsWizardPage != null)
					{
						flag = progressTridentsWizardPage.CanGoBack;
					}
				}
				base.CanGoBack = (base.DataHandler.WorkUnits.HasFailures && flag);
				if (base.WorkUnits.Cancelled)
				{
					base.Status = Strings.TheWizardWasCancelled;
					base.ShortDescription = Strings.WizardCancelledNotAllActionsCompleted + " " + Strings.FinishWizardDescription;
					return;
				}
				base.Status = base.DataHandler.CompletionStatus;
				string text = base.DataHandler.CompletionDescription.Trim();
				if (string.IsNullOrEmpty(text))
				{
					if (base.Wizard != null && base.Wizard.ParentForm is WizardForm)
					{
						if (base.DataHandler.IsSucceeded)
						{
							text = (base.WorkUnits.AllCompleted ? Strings.WizardCompletionSucceededDescription : Strings.WizardCompletionPartialSucceededDescription);
						}
						else
						{
							text = Strings.WizardCompletionFailedDescription;
						}
					}
					text = text + " " + Strings.FinishWizardDescription;
				}
				base.ShortDescription = text;
			}
		}

		protected override void OnGoBack(CancelEventArgs e)
		{
			base.OnGoBack(e);
			if (!e.Cancel)
			{
				base.WorkUnits = null;
			}
		}
	}
}
