using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[DefaultEvent("Validating")]
	[DefaultProperty("Text")]
	[ToolboxItem(false)]
	public class WizardPage : ExchangePage
	{
		public WizardPage()
		{
			base.Name = "WizardPage";
			this.childPages = new WizardPageCollection(this);
		}

		protected override Size DefaultSize
		{
			get
			{
				return WizardPage.defaultSize;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public Wizard Wizard
		{
			get
			{
				if (base.Parent == null)
				{
					return null;
				}
				return (Wizard)base.Parent;
			}
		}

		protected override void OnSetActive(EventArgs e)
		{
			if (base.CheckReadOnlyAndDisablePage() && this.Wizard.WizardPages.Count == 2 && this.Wizard.WizardPages[0] == this)
			{
				this.CanGoForward = false;
				base.CanCancel = true;
			}
			base.OnSetActive(e);
		}

		protected override void OnKillActive(CancelEventArgs e)
		{
			base.InputValidationProvider.WriteBindings();
			base.OnKillActive(e);
		}

		public event CancelEventHandler GoBack;

		protected virtual void OnGoBack(CancelEventArgs e)
		{
			if (this.GoBack != null)
			{
				this.GoBack(this, e);
			}
		}

		public bool NotifyGoBack()
		{
			if (this.backupIgnorePageValidationOnGoBack == null)
			{
				this.backupIgnorePageValidationOnGoBack = new bool?(base.IgnorePageValidation);
			}
			base.IgnorePageValidation = true;
			CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
			this.OnGoBack(cancelEventArgs);
			return !cancelEventArgs.Cancel;
		}

		[DefaultValue(true)]
		public bool CanGoBack
		{
			get
			{
				return this.canGoBack;
			}
			set
			{
				if (this.CanGoBack != value)
				{
					this.canGoBack = value;
					this.OnCanGoBackChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnCanGoBackChanged(EventArgs e)
		{
			if (this.CanGoBackChanged != null)
			{
				this.CanGoBackChanged(this, e);
			}
		}

		public event EventHandler CanGoBackChanged;

		public event CancelEventHandler GoForward;

		protected virtual void OnGoForward(CancelEventArgs e)
		{
			if (this.GoForward != null)
			{
				this.GoForward(this, e);
			}
		}

		public bool NotifyGoForward()
		{
			if (this.backupIgnorePageValidationOnGoBack != null)
			{
				base.IgnorePageValidation = this.backupIgnorePageValidationOnGoBack.Value;
			}
			CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
			this.OnGoForward(cancelEventArgs);
			return !cancelEventArgs.Cancel;
		}

		[DefaultValue(true)]
		public bool CanGoForward
		{
			get
			{
				return this.canGoForward;
			}
			set
			{
				if (this.CanGoForward != value)
				{
					this.canGoForward = value;
					this.OnCanGoForwardChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnCanGoForwardChanged(EventArgs e)
		{
			if (this.CanGoForwardChanged != null)
			{
				this.CanGoForwardChanged(this, e);
			}
		}

		public event EventHandler CanGoForwardChanged;

		public event CancelEventHandler Finish;

		protected virtual void OnFinish(CancelEventArgs e)
		{
			if (this.Finish != null)
			{
				this.Finish(this, e);
			}
		}

		public bool NotifyFinish()
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
			this.OnFinish(cancelEventArgs);
			return !cancelEventArgs.Cancel;
		}

		[DefaultValue(true)]
		public bool CanFinish
		{
			get
			{
				return this.canFinish;
			}
			set
			{
				if (this.CanFinish != value)
				{
					this.canFinish = value;
					this.OnCanFinishChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnCanFinishChanged(EventArgs e)
		{
			if (this.CanFinishChanged != null)
			{
				this.CanFinishChanged(this, e);
			}
		}

		public event EventHandler CanFinishChanged;

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			this.OnNextButtonTextChanged(EventArgs.Empty);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public LocalizedString NextButtonText
		{
			get
			{
				if (!base.Enabled)
				{
					return Strings.Next;
				}
				return this.nextButtonText;
			}
			set
			{
				value = ((value == LocalizedString.Empty) ? Strings.Next : value);
				if (value != this.NextButtonText)
				{
					this.nextButtonText = value;
					this.OnNextButtonTextChanged(EventArgs.Empty);
				}
			}
		}

		private void ResetNextButtonText()
		{
			this.NextButtonText = Strings.Next;
		}

		public event EventHandler NextButtonTextChanged;

		protected void OnNextButtonTextChanged(EventArgs e)
		{
			if (this.NextButtonTextChanged != null)
			{
				this.NextButtonTextChanged(this, e);
			}
		}

		[DefaultValue(null)]
		public WizardPage ParentPage
		{
			get
			{
				return this.parentPage;
			}
			internal set
			{
				if (value != this.ParentPage)
				{
					this.parentPage = value;
				}
			}
		}

		public WizardPageCollection ChildPages
		{
			get
			{
				return this.childPages;
			}
		}

		internal int GetChildCount()
		{
			int num = this.ChildPages.Count;
			foreach (WizardPage wizardPage in this.ChildPages)
			{
				num += wizardPage.GetChildCount();
			}
			return num;
		}

		protected override void OnCancel(CancelEventArgs e)
		{
			if (this.Wizard != null && this.Wizard.WizardPages.Count >= 2 && this.Wizard.IsDirty)
			{
				DialogResult dialogResult = base.ShowMessage(Strings.CancelWizardConfirmationMessage, MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.No)
				{
					e.Cancel = true;
				}
			}
			base.OnCancel(e);
		}

		protected override void OnReadDataFailed(EventArgs e)
		{
			base.BeginInvoke(new EventHandler(this.CloseWizard));
		}

		private void CloseWizard(object sender, EventArgs e)
		{
			Form parentForm = base.ParentForm;
			if (parentForm != null)
			{
				parentForm.DialogResult = DialogResult.Cancel;
				parentForm.Close();
			}
		}

		private bool? backupIgnorePageValidationOnGoBack;

		internal static Size defaultSize = new Size(454, 398);

		private bool canGoBack = true;

		private bool canGoForward = true;

		private bool canFinish = true;

		private LocalizedString nextButtonText = Strings.Next;

		private WizardPage parentPage;

		private WizardPageCollection childPages;
	}
}
