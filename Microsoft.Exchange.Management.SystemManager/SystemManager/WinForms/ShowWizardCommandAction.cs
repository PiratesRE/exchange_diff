using System;
using System.Windows.Forms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ShowWizardCommandAction : ResultsCommandAction
	{
		public ShowWizardCommandAction()
		{
		}

		protected override void OnExecute()
		{
			base.OnExecute();
			string sharedFormName = this.GetSharedFormName();
			if (!string.IsNullOrEmpty(sharedFormName))
			{
				if (!ExchangeForm.ActivateSingleInstanceForm(sharedFormName))
				{
					this.ShowWizardForm(this.InternalCreateWizardForm(), sharedFormName);
					return;
				}
			}
			else
			{
				this.ShowWizardForm(this.InternalCreateWizardForm());
			}
		}

		private void ShowWizardForm(WizardForm wizardForm, string formName)
		{
			wizardForm.Name = formName;
			this.ShowWizardForm(wizardForm);
		}

		private void ShowWizardForm(WizardForm wizardForm)
		{
			wizardForm.ShowModeless(base.ResultPane);
		}

		protected virtual string GetSharedFormName()
		{
			return string.Empty;
		}

		private WizardForm InternalCreateWizardForm()
		{
			WizardForm wizardForm = this.CreateWizardForm();
			IRefreshable defaultRefreshObject = base.GetDefaultRefreshObject();
			wizardForm.OriginatingCommand = base.Command;
			wizardForm.FormClosed += delegate(object param0, FormClosedEventArgs param1)
			{
				if (wizardForm.Wizard.IsDataChanged)
				{
					this.RefreshResultsThreadSafely(this.GetDataContextForRefresh(wizardForm, defaultRefreshObject));
				}
			};
			wizardForm.Load += delegate(object param0, EventArgs param1)
			{
				string text = this.CheckPreCondition((wizardForm.Context != null) ? wizardForm.Context.DataHandler.DataSource : null);
				if (!string.IsNullOrEmpty(text))
				{
					wizardForm.Close();
					this.ResultPane.ShowError(text);
				}
			};
			return wizardForm;
		}

		protected virtual string CheckPreCondition(object dataSource)
		{
			return string.Empty;
		}

		private DataContext GetDataContextForRefresh(WizardForm wizardForm, IRefreshable defaultRefreshObject)
		{
			WizardPage wizardPage = wizardForm.WizardPages[wizardForm.WizardPages.Count - 1];
			DataContext context = wizardPage.Context;
			if (wizardForm.RefreshOnFinish != null)
			{
				context.RefreshOnSave = wizardForm.RefreshOnFinish;
			}
			else
			{
				context.RefreshOnSave = (context.RefreshOnSave ?? defaultRefreshObject);
			}
			return context;
		}

		protected override void RefreshResults(DataContext context)
		{
			if (context != null && context.RefreshOnSave == null)
			{
				if (context.DataHandler != null && context.DataHandler.SavedResults != null && context.DataHandler.SavedResults.Count > 0)
				{
					DataListViewResultPane dataListViewResultPane = base.ResultPane as DataListViewResultPane;
					if (context.DataHandler.SavedResults.Count == 1)
					{
						dataListViewResultPane.RefreshObject(context.DataHandler.SavedResults[0]);
						return;
					}
					dataListViewResultPane.RefreshObjects(context.DataHandler.SavedResults);
					return;
				}
			}
			else
			{
				base.RefreshResults(context);
			}
		}

		protected abstract WizardForm CreateWizardForm();
	}
}
