using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class UIService : IUIService
	{
		public UIService(IWin32Window parentWindow)
		{
			this.parentWindow = parentWindow;
		}

		public bool CanShowComponentEditor(object component)
		{
			ComponentEditor componentEditor = (ComponentEditor)TypeDescriptor.GetEditor(component, typeof(ComponentEditor));
			return null != componentEditor;
		}

		public IWin32Window GetDialogOwnerWindow()
		{
			return this.parentWindow;
		}

		public virtual void SetUIDirty()
		{
		}

		public bool ShowComponentEditor(object component, IWin32Window parent)
		{
			bool result = false;
			ComponentEditor componentEditor = (ComponentEditor)TypeDescriptor.GetEditor(component, typeof(ComponentEditor));
			if (componentEditor != null)
			{
				WindowsFormsComponentEditor windowsFormsComponentEditor = componentEditor as WindowsFormsComponentEditor;
				if (windowsFormsComponentEditor != null)
				{
					if (parent == null)
					{
						parent = this.GetDialogOwnerWindow();
					}
					result = windowsFormsComponentEditor.EditComponent(component, parent);
				}
				else
				{
					result = componentEditor.EditComponent(component);
				}
			}
			return result;
		}

		protected virtual DialogResult OnShowDialog(Form form)
		{
			return form.ShowDialog(this.GetDialogOwnerWindow());
		}

		public DialogResult ShowDialog(Form form)
		{
			IContainer container = null;
			ContainerControl containerControl = this.GetDialogOwnerWindow() as ContainerControl;
			if (containerControl != null)
			{
				container = containerControl.Container;
				if (container == null && containerControl.ParentForm != null)
				{
					container = containerControl.ParentForm.Container;
				}
				if (container != null)
				{
					container.Add(form, form.Name + form.GetHashCode().ToString());
				}
			}
			if (string.IsNullOrEmpty(form.Text))
			{
				form.Text = UIService.DefaultCaption;
			}
			Control focusedControlOnPropertyPage = this.GetFocusedControlOnPropertyPage();
			DialogResult result;
			try
			{
				result = this.OnShowDialog(form);
			}
			finally
			{
				if (container != null)
				{
					container.Remove(form);
				}
				if (focusedControlOnPropertyPage != null)
				{
					focusedControlOnPropertyPage.Focus();
				}
			}
			return result;
		}

		private Control GetFocusedControlOnPropertyPage()
		{
			Control control = this.GetDialogOwnerWindow() as Control;
			if (control != null && control.Parent != null)
			{
				while (control.Parent.Parent != null)
				{
					control = control.Parent;
				}
				if (control is ExchangePropertyPageControl)
				{
					return ((ExchangePropertyPageControl)control).FocusedControl;
				}
			}
			return null;
		}

		public void ShowError(Exception ex, string message)
		{
			if (ex is ADServerSettingsChangedException || ex.InnerException is ADServerSettingsChangedException)
			{
				ADServerSettingsSingleton.GetInstance().ADServerSettings.SetDefaultSettings();
				this.ShowError(Strings.ADServerSettingsChangedException);
				return;
			}
			if (ex is CmdletInvocationException || DataAccessHelper.IsDataAccessKnownException(ex))
			{
				this.ShowError(ex.Message);
				return;
			}
			if (ExceptionHelper.IsWellknownCommandExecutionException(ex))
			{
				this.ShowError(ex.InnerException.Message);
				return;
			}
			if (ex is VersionMismatchException)
			{
				this.ShowError(ex.Message);
				return;
			}
			if (ex is SupportedVersionListFormatException)
			{
				this.ShowError(ex.Message);
				return;
			}
			if (ex is PooledConnectionOpenTimeoutException)
			{
				this.ShowError(ex.Message);
				return;
			}
			if (ex is PSRemotingTransportException)
			{
				this.ShowError(ex.Message);
				return;
			}
			if (ex is PSRemotingDataStructureException)
			{
				this.ShowError(ex.Message);
				return;
			}
			if (ex is PSInvalidOperationException)
			{
				this.ShowError(ex.Message);
				return;
			}
			if (ex is OperationCanceledException)
			{
				this.ShowError(ex.Message);
				return;
			}
			using (ExceptionDialog exceptionDialog = new ExceptionDialog())
			{
				if (ExceptionHelper.IsWellknownExceptionFromServer(ex.InnerException))
				{
					exceptionDialog.Exception = ex.InnerException;
				}
				else
				{
					exceptionDialog.Exception = ex;
				}
				if (!string.IsNullOrEmpty(message))
				{
					exceptionDialog.Message = string.Format("{0}\r\n\r\n{1}", message, exceptionDialog.Message);
				}
				((IUIService)this).ShowDialog(exceptionDialog);
			}
		}

		public void ShowError(Exception ex)
		{
			((IUIService)this).ShowError(ex, string.Empty);
		}

		internal static bool ShowError(string errorMessage, string warningMessage, IList<WorkUnit> errors, IUIService uiService)
		{
			if (errors.Count > 0)
			{
				bool flag = true;
				for (int i = 0; i < errors.Count; i++)
				{
					if (errors[i].Errors.Count > 0)
					{
						flag = false;
						break;
					}
				}
				using (TridentErrorDialog tridentErrorDialog = new TridentErrorDialog())
				{
					tridentErrorDialog.Message = (flag ? warningMessage : errorMessage);
					tridentErrorDialog.Errors = errors;
					tridentErrorDialog.IsWarningOnly = flag;
					uiService.ShowDialog(tridentErrorDialog);
				}
				return !flag;
			}
			return false;
		}

		public static string DefaultCaption
		{
			get
			{
				return Strings.MicrosoftExchange;
			}
		}

		protected static MessageBoxDefaultButton GetDefaultButton(MessageBoxButtons buttons)
		{
			MessageBoxDefaultButton result;
			switch (buttons)
			{
			default:
				result = MessageBoxDefaultButton.Button1;
				break;
			case MessageBoxButtons.OKCancel:
			case MessageBoxButtons.AbortRetryIgnore:
			case MessageBoxButtons.YesNo:
				result = MessageBoxDefaultButton.Button2;
				break;
			}
			return result;
		}

		protected virtual DialogResult MessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			DialogResult result;
			using (MessageBoxDialog messageBoxDialog = new MessageBoxDialog(message, caption, buttons, icon, UIService.GetDefaultButton(buttons)))
			{
				result = this.ShowDialog(messageBoxDialog);
			}
			return result;
		}

		public void ShowError(string message)
		{
			this.MessageBox(message, UIService.DefaultCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		public DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons)
		{
			MessageBoxIcon icon = (buttons == MessageBoxButtons.OK) ? MessageBoxIcon.Asterisk : MessageBoxIcon.Exclamation;
			return this.MessageBox(message, caption, buttons, icon);
		}

		public void ShowMessage(string message, string caption)
		{
			this.MessageBox(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		public void ShowMessage(string message)
		{
			this.MessageBox(message, UIService.DefaultCaption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		public virtual bool ShowToolWindow(Guid toolWindow)
		{
			return false;
		}

		public virtual IDictionary Styles
		{
			get
			{
				if (this.styles == null)
				{
					this.styles = new Hashtable(2);
					this.styles["DialogFont"] = SystemFonts.DialogFont;
					this.styles["HighlightColor"] = SystemColors.Highlight;
				}
				return this.styles;
			}
		}

		private IWin32Window parentWindow;

		private Hashtable styles;
	}
}
