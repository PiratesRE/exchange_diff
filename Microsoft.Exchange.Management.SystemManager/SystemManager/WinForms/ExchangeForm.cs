using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.Sqm;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Services;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ExchangeForm : Form, IServiceProvider
	{
		[DefaultValue(true)]
		public bool NeedRestoreOwnerAfterModeless
		{
			get
			{
				return this.needRestoreOwnerAfterModeless;
			}
			set
			{
				this.needRestoreOwnerAfterModeless = value;
			}
		}

		public override RightToLeft RightToLeft
		{
			get
			{
				if (!LayoutHelper.CultureInfoIsRightToLeft)
				{
					return base.RightToLeft;
				}
				return RightToLeft.Yes;
			}
			set
			{
			}
		}

		public override bool RightToLeftLayout
		{
			get
			{
				return LayoutHelper.IsRightToLeft(this);
			}
			set
			{
			}
		}

		public IUIService ShellUI
		{
			get
			{
				IUIService iuiservice = (IUIService)this.GetService(typeof(IUIService));
				if (iuiservice == null)
				{
					iuiservice = this.CreateUIService();
				}
				return iuiservice;
			}
		}

		protected virtual IUIService CreateUIService()
		{
			return new UIService(this);
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			return this.GetService(serviceType);
		}

		public void ShowError(string message)
		{
			this.ShellUI.ShowError(message);
		}

		public void ShowMessage(string message)
		{
			this.ShellUI.ShowMessage(message);
		}

		public DialogResult ShowMessage(string message, MessageBoxButtons buttons)
		{
			return this.ShellUI.ShowMessage(message, UIService.DefaultCaption, buttons);
		}

		public IProgress CreateProgress(LocalizedString operationName)
		{
			IProgressProvider progressProvider = (IProgressProvider)this.GetService(typeof(IProgressProvider));
			if (progressProvider != null)
			{
				return progressProvider.CreateProgress(operationName);
			}
			return NullProgress.Value;
		}

		[Obsolete("Use ShowModeless instead")]
		public new void Show()
		{
		}

		[Obsolete("Use ShowModeless instead")]
		public new void Show(IWin32Window parentWindow)
		{
		}

		public void ShowModeless(IServiceProvider parentProvider)
		{
			this.ShowModeless(parentProvider, null);
		}

		public void ShowModeless(IServiceProvider parentProvider, Control owner)
		{
			ServiceContainer serviceContainer = new ServiceContainer(parentProvider);
			serviceContainer.AddService(typeof(IUIService), new UIService(this));
			ServicedContainer servicedContainer = new ServicedContainer(serviceContainer);
			servicedContainer.Add(this, this.GetHashCode().ToString());
			this.restoreAfterModeless = (owner ?? (parentProvider as Control));
			if (owner == null)
			{
				base.Show();
				return;
			}
			base.Show(owner);
		}

		public string HelpTopic
		{
			get
			{
				if (this.helpTopic == null)
				{
					this.helpTopic = this.DefaultHelpTopic;
				}
				return this.helpTopic;
			}
			set
			{
				value = (value ?? "");
				this.helpTopic = value;
			}
		}

		private bool ShouldSerializeHelpTopic()
		{
			return this.HelpTopic != this.DefaultHelpTopic;
		}

		private void ResetHelpTopic()
		{
			this.HelpTopic = this.DefaultHelpTopic;
		}

		protected virtual string DefaultHelpTopic
		{
			get
			{
				return base.GetType().FullName;
			}
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled && !string.IsNullOrEmpty(this.HelpTopic))
			{
				ExchangeHelpService.ShowHelpFromHelpTopicId(this, this.HelpTopic);
			}
			hevent.Handled = true;
			base.OnHelpRequested(hevent);
		}

		public static event EventHandler Test_FormShown;

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			if (ExchangeForm.Test_FormShown != null)
			{
				ExchangeForm.Test_FormShown(this, EventArgs.Empty);
			}
			if (ManagementGuiSqmSession.Instance.Enabled)
			{
				ManagementGuiSqmSession.Instance.AddToStreamDataPoint(SqmDataID.DATAID_EMC_GUI_ACTION, new object[]
				{
					3U,
					base.GetType().Name
				});
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.ExchangeFormOwner != null)
			{
				this.ExchangeFormOwner.OnExchangeFormLoad(this);
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			ExchangeForm.AddToOpenForms(this);
			base.OnHandleCreated(e);
		}

		private IExchangeFormOwner ExchangeFormOwner
		{
			get
			{
				return (IExchangeFormOwner)this.GetService(typeof(IExchangeFormOwner));
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);
			if (this.ExchangeFormOwner != null)
			{
				this.ExchangeFormOwner.OnExchangeFormClosed(this);
			}
			this.BringRestoredWindowToFront();
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			ExchangeForm.RemoveFromOpenForms(this);
			base.OnHandleDestroyed(e);
		}

		private void BringRestoredWindowToFront()
		{
			if (this.restoreAfterModeless != null && this.NeedRestoreOwnerAfterModeless)
			{
				this.restoreAfterModeless.Focus();
				this.restoreAfterModeless = null;
			}
		}

		private static List<ExchangeForm> OpenForms
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				if (ExchangeForm.openForms == null)
				{
					ExchangeForm.openForms = new List<ExchangeForm>();
				}
				return ExchangeForm.openForms;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void AddToOpenForms(ExchangeForm form)
		{
			if (!ExchangeForm.OpenForms.Contains(form))
			{
				ExchangeForm.OpenForms.Add(form);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void RemoveFromOpenForms(ExchangeForm form)
		{
			if (ExchangeForm.OpenForms.Contains(form))
			{
				ExchangeForm.OpenForms.Remove(form);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static ExchangeForm GetOpenForm(string formName)
		{
			foreach (ExchangeForm exchangeForm in ExchangeForm.OpenForms)
			{
				if (exchangeForm.Name == formName)
				{
					return exchangeForm;
				}
			}
			return null;
		}

		internal static bool ActivateSingleInstanceForm(string formName)
		{
			ExchangeForm openForm = ExchangeForm.GetOpenForm(formName);
			if (openForm != null)
			{
				openForm.Activate();
			}
			return openForm != null;
		}

		private bool needRestoreOwnerAfterModeless = true;

		private string helpTopic;

		private Control restoreAfterModeless;

		private static List<ExchangeForm> openForms;
	}
}
