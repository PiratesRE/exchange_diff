using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangePropertyPageControl : ExchangePage, IHasPermission
	{
		public ExchangePropertyPageControl()
		{
			base.Name = "ExchangePropertyPageControl";
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(418, 396);
			}
		}

		public void Apply(CancelEventArgs e)
		{
			this.OnApplying(e);
			if (base.Context != null)
			{
				if (!base.Context.IsDirty)
				{
					base.Context.AllowNextRead();
				}
				else if (this.TryApply())
				{
					base.DataHandler.UpdateWorkUnits();
					if (base.DataHandler.TimeConsuming)
					{
						e.Cancel = !this.SaveDataContextByProgressDialog();
					}
					else
					{
						e.Cancel = !this.SaveDataContextByInvisibleForm();
					}
				}
				else
				{
					e.Cancel = true;
				}
				if (!e.Cancel)
				{
					base.IsDirty = false;
				}
			}
		}

		private bool SaveDataContextByInvisibleForm()
		{
			bool result;
			using (InvisibleForm invisibleForm = new InvisibleForm())
			{
				invisibleForm.BackgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e2)
				{
					base.Context.SaveData(new WinFormsCommandInteractionHandler(base.ShellUI));
				};
				invisibleForm.ShowDialog(this);
				bool flag = !invisibleForm.ShowErrors(Strings.PropertyPageWriteError, Strings.PropertyPageWriteWarning, base.DataHandler.WorkUnits, base.ShellUI);
				result = flag;
			}
			return result;
		}

		private bool ConfirmedToSaveDataContext()
		{
			bool result = false;
			if (base.Context.IsDirty)
			{
				using (BulkEditSummaryPage bulkEditSummaryPage = new BulkEditSummaryPage())
				{
					bulkEditSummaryPage.BindingSource.DataSource = base.Context;
					if (base.ShowDialog(bulkEditSummaryPage) == DialogResult.OK)
					{
						result = true;
					}
				}
			}
			return result;
		}

		private bool SaveDataContextByProgressDialog()
		{
			bool succeeded = false;
			if (this.ConfirmedToSaveDataContext())
			{
				using (BackgroundWorkerProgressDialog progressDialog = new BackgroundWorkerProgressDialog())
				{
					progressDialog.Text = Strings.BulkEditingProgressDialogTitle;
					progressDialog.OkEnabled = false;
					progressDialog.CancelEnabled = base.Context.DataHandler.CanCancel;
					base.Context.DataHandler.UpdateWorkUnits();
					WorkUnitCollection workUnits = base.Context.DataHandler.WorkUnits;
					progressDialog.UseMarquee = true;
					progressDialog.StatusText = workUnits.Description;
					int timeIntervalForReport = 100;
					Stopwatch reportStatus = new Stopwatch();
					reportStatus.Start();
					base.Context.DataHandler.ProgressReport += delegate(object sender, ProgressReportEventArgs progressReportEventArgs)
					{
						if (reportStatus.ElapsedMilliseconds > (long)timeIntervalForReport)
						{
							int percentProgress = workUnits.ProgressValue * 100 / workUnits.MaxProgressValue;
							progressDialog.ReportProgress(percentProgress, workUnits.Description);
							reportStatus.Reset();
							reportStatus.Start();
						}
					};
					progressDialog.DoWork += delegate(object sender, DoWorkEventArgs e2)
					{
						base.Context.SaveData(new WinFormsCommandInteractionHandler(base.ShellUI));
					};
					progressDialog.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e2)
					{
						progressDialog.ReportProgress(100, workUnits.Description);
						succeeded = !progressDialog.ShowErrors(Strings.PropertyPageWriteError, Strings.PropertyPageWriteWarning, workUnits, this.ShellUI);
						if (succeeded && workUnits.FindByStatus(WorkUnitStatus.NotStarted).Count > 0)
						{
							succeeded = false;
						}
					};
					progressDialog.FormClosing += delegate(object sender, FormClosingEventArgs e)
					{
						if (progressDialog.IsBusy && progressDialog.CancelEnabled)
						{
							progressDialog.CancelEnabled = false;
							WinformsHelper.InvokeAsync(delegate
							{
								this.Context.DataHandler.Cancel();
							}, progressDialog);
							e.Cancel = progressDialog.IsBusy;
						}
					};
					progressDialog.FormClosed += delegate(object param0, FormClosedEventArgs param1)
					{
						base.Context.DataHandler.ResetCancel();
					};
					progressDialog.ShowDialog(this);
				}
			}
			return succeeded;
		}

		internal bool TryApply()
		{
			ValidationError[] array = this.ValidateOnApply();
			if (array.Length != 0)
			{
				base.ShowError(base.CreateValidateContextErrorMessageFor(new List<ValidationError>(array)));
				return false;
			}
			return true;
		}

		protected virtual ValidationError[] ValidateOnApply()
		{
			return base.Context.Validate();
		}

		protected virtual void OnApplying(CancelEventArgs e)
		{
		}

		protected override void OnSetActive(EventArgs e)
		{
			base.OnSetActive(e);
			EventHandler test_SetActive = ExchangePropertyPageControl.Test_SetActive;
			if (test_SetActive != null)
			{
				test_SetActive(this, EventArgs.Empty);
			}
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			Button button = base.FocusedControl as Button;
			if (keyData == Keys.Return && button != null)
			{
				button.PerformClick();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		protected override void VerifyCorruptedObject()
		{
			if (base.Context.IsDataSourceCorrupted)
			{
				if (DialogResult.OK == base.ShowMessage(this.CreateErrorMessageForCorruptedObject(), MessageBoxButtons.OKCancel))
				{
					base.Context.OverrideCorruptedValuesWithDefault();
					if (base.Context.IsDirty)
					{
						AutomatedDataHandler automatedDataHandler = base.DataHandler as AutomatedDataHandler;
						if (automatedDataHandler != null)
						{
							automatedDataHandler.RefreshDataObjectStore();
						}
						base.ForceIsDirty(true);
						return;
					}
				}
				else
				{
					base.DisableRelatedPages(false);
				}
			}
		}

		private string CreateErrorMessageForCorruptedObject()
		{
			ADObject adobject = base.Context.DataHandler.DataSource as ADObject;
			string str;
			if (adobject != null && !string.IsNullOrEmpty(adobject.Name))
			{
				str = Strings.CorruptedObjectErrorMessageObjectName(string.Format("'{0}'", adobject.Name));
			}
			else
			{
				str = Strings.CorruptedObjectErrorMessageNoName;
			}
			LocalizedString localizedString = LocalizedString.Empty;
			IConfigurable configurable = base.Context.DataHandler.DataSource as IConfigurable;
			if (configurable != null)
			{
				ValidationError[] array = configurable.Validate();
				List<string> list = new List<string>(array.Length);
				foreach (ValidationError validationError in array)
				{
					PropertyValidationError propertyValidationError = validationError as PropertyValidationError;
					if (propertyValidationError != null)
					{
						string name = propertyValidationError.PropertyDefinition.Name;
						if (!list.Contains(name))
						{
							list.Add(name);
						}
					}
				}
				if (list.Count > 0)
				{
					localizedString = Strings.CorruptedObjectErrorPropertyNames(string.Join(", ", list.ToArray()));
				}
			}
			return str + Strings.CorruptedObjectErrorMessageBody.ToString() + localizedString.ToString();
		}

		protected override ValidationError[] ValidateContextOnPageTransition()
		{
			return base.Context.ValidateOnly(base.BindingSource.DataSource);
		}

		protected override bool BlockPageSwitchWithError(PropertyValidationError propertyError)
		{
			return propertyError != null && propertyError.PropertyDefinition != null && base.InputValidationProvider.IsBoundToProperty(propertyError.PropertyDefinition.Name);
		}

		public bool HasPermission()
		{
			AutomatedDataHandler automatedDataHandler = base.DataHandler as AutomatedDataHandler;
			return automatedDataHandler == null || automatedDataHandler.HasViewPermissionForPage(base.Name);
		}

		public bool HasLockedControls
		{
			get
			{
				AutomatedDataHandlerBase automatedDataHandlerBase = base.DataHandler as AutomatedDataHandlerBase;
				if (automatedDataHandlerBase != null)
				{
					BindingManagerBase bindingManagerBase = this.BindingContext[base.BindingSource];
					foreach (object obj in bindingManagerBase.Bindings)
					{
						Binding binding = (Binding)obj;
						IBulkEditor bulkEditor = binding.Control as IBulkEditor;
						if (bulkEditor != null && bulkEditor.BulkEditorAdapter[binding.PropertyName] == 3)
						{
							return true;
						}
					}
					return false;
				}
				return false;
			}
		}

		public static event EventHandler Test_SetActive;
	}
}
