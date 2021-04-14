using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.Exchange.Sqm;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[DefaultEvent("KillActive")]
	[DefaultProperty("BindingSource")]
	public class ExchangePage : BindableUserControlBase
	{
		public event EventHandler ReadDataFailed;

		private void RegisterBindableControl(BindableUserControl control)
		{
			if (!this.bindableUnpagedControlList.Contains(control))
			{
				this.bindableUnpagedControlList.Add(control);
				this.InputValidationProvider.SetEnabled(control.BindingSource, true);
				this.inputValidationProvider.SetUIValidationEnabled(control, true);
			}
		}

		public ExchangePage()
		{
			this.BindingContext = new BindingContext();
			this.InitializeComponent();
			this.makeDirty = new EventHandler(this.MakeDirtyHandler);
			this.DoubleBuffered = true;
			this.bindableUnpagedControlList = new List<BindableUserControl>();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AutoValidate AutoValidate
		{
			get
			{
				return base.AutoValidate;
			}
			set
			{
				base.AutoValidate = value;
			}
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			this.inputValidationProvider = new InputValidationProvider(this.components);
			((ISupportInitialize)base.BindingSource).BeginInit();
			base.SuspendLayout();
			this.inputValidationProvider.SetEnabled(base.BindingSource, true);
			this.inputValidationProvider.ContainerControl = this;
			this.inputValidationProvider.DataBoundPropertyChanged += this.MakeDirtyHandler;
			base.Name = "ExchangePage";
			((ISupportInitialize)base.BindingSource).EndInit();
			base.ResumeLayout(false);
		}

		public InputValidationProvider InputValidationProvider
		{
			get
			{
				return this.inputValidationProvider;
			}
		}

		[DefaultValue(null)]
		public DataContext Context
		{
			get
			{
				return this.context;
			}
			set
			{
				if (value != this.Context)
				{
					if (this.Context != null)
					{
						this.Context.Pages.Remove(this);
					}
					this.context = value;
					if (this.Context != null)
					{
						this.Context.Pages.Add(this);
						AutomatedDataHandlerBase automatedDataHandlerBase = this.context.DataHandler as AutomatedDataHandlerBase;
						if (automatedDataHandlerBase != null)
						{
							base.BindingSource.DataSource = automatedDataHandlerBase.GetDataTableSchema();
						}
					}
					this.OnContextChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnContextChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ExchangePage.EventContextChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler ContextChanged
		{
			add
			{
				base.Events.AddHandler(ExchangePage.EventContextChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ExchangePage.EventContextChanged, value);
			}
		}

		public DataHandler DataHandler
		{
			get
			{
				if (this.Context == null)
				{
					return null;
				}
				return this.Context.DataHandler;
			}
		}

		public override DialogResult ShowDialog(Form form)
		{
			Control control = base.FocusedControl ?? this;
			if (!this.OnKillActive())
			{
				return DialogResult.Cancel;
			}
			DialogResult result;
			try
			{
				DialogResult dialogResult = base.ShowDialog(form);
				if (dialogResult == DialogResult.OK && (this.Context == null || this.Context.IsDirty))
				{
					this.IsDirty = true;
				}
				result = dialogResult;
			}
			finally
			{
				this.OnSetActive();
				control.Focus();
			}
			return result;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected ErrorProvider ErrorProvider
		{
			get
			{
				return this.InputValidationProvider.GetErrorProvider(base.BindingSource);
			}
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
				if (this.HelpTopic != value)
				{
					this.helpTopic = value;
					this.OnHelpTopicChanged(EventArgs.Empty);
				}
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

		private bool InDesignMode
		{
			get
			{
				bool designMode = base.DesignMode;
				if (!designMode)
				{
					Form form = base.FindForm() as WizardForm;
					if (form != null && form.Site != null)
					{
						designMode = form.Site.DesignMode;
					}
				}
				return designMode;
			}
		}

		protected virtual string DefaultHelpTopic
		{
			get
			{
				return base.GetType().FullName;
			}
		}

		protected virtual void OnHelpTopicChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ExchangePage.EventHelpTopicChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler HelpTopicChanged
		{
			add
			{
				base.Events.AddHandler(ExchangePage.EventHelpTopicChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ExchangePage.EventHelpTopicChanged, value);
			}
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled)
			{
				ExchangeHelpService.ShowHelpFromPage(this);
				hevent.Handled = true;
			}
			base.OnHelpRequested(hevent);
		}

		public event CancelEventHandler Cancel;

		protected virtual void OnCancel(CancelEventArgs e)
		{
			if (this.Cancel != null)
			{
				this.Cancel(this, e);
			}
		}

		public bool NotifyCancel()
		{
			if (this.CanCancel)
			{
				CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
				this.OnCancel(cancelEventArgs);
				return !cancelEventArgs.Cancel;
			}
			return false;
		}

		[DefaultValue(true)]
		public bool CanCancel
		{
			get
			{
				return this.canCancel;
			}
			set
			{
				if (this.CanCancel != value)
				{
					this.canCancel = value;
					this.OnCanCancelChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnCanCancelChanged(EventArgs e)
		{
			if (this.CanCancelChanged != null)
			{
				this.CanCancelChanged(this, e);
			}
		}

		public event EventHandler CanCancelChanged;

		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
			set
			{
				if (!this.suppressIsDirtyChanges && this.IsDirty != value && (this.Context == null || !this.Context.IsSaving))
				{
					this.isDirty = value;
					if (this.IsDirty && this.Context != null)
					{
						this.Context.IsDirty = true;
					}
					this.OnIsDirtyChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIsDirtyChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ExchangePage.EventIsDirtyChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IsDirtyChanged
		{
			add
			{
				SynchronizedDelegate.Combine(base.Events, ExchangePage.EventIsDirtyChanged, value);
			}
			remove
			{
				SynchronizedDelegate.Remove(base.Events, ExchangePage.EventIsDirtyChanged, value);
			}
		}

		public EventHandler MakeDirty
		{
			get
			{
				return this.makeDirty;
			}
		}

		private void MakeDirtyHandler(object sender, EventArgs e)
		{
			this.IsDirty = true;
		}

		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[DefaultValue(false)]
		public bool IgnorePageValidation
		{
			get
			{
				return this.ignorePageValidation;
			}
			set
			{
				this.ignorePageValidation = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Context = null;
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public void OnSetActive()
		{
			this.OnSetActive(true);
		}

		internal void OnSetActive(bool focusOnFirstChild)
		{
			this.suppressIsDirtyChanges = true;
			try
			{
				this.isActivePage = true;
				this.InputValidationProvider.SuspendRecordingModifiedBindingMembers = true;
				if (base.IsHandleCreated)
				{
					if (this.Context != null && !this.InDesignMode)
					{
						using (InvisibleForm invisibleForm = new InvisibleForm())
						{
							invisibleForm.BackgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
							{
								e.Result = this.Context.ReadData(new WinFormsCommandInteractionHandler(base.ShellUI), base.Name);
							};
							invisibleForm.ShowDialog(this);
							if (base.IsDisposed)
							{
								return;
							}
							bool flag = invisibleForm.ShowErrors(Strings.PropertyPageReadError, Strings.PropertyPageReadWarning, new WorkUnitCollection(), base.ShellUI);
							if (invisibleForm.AsyncResults != null)
							{
								base.BindingSource.DataSource = invisibleForm.AsyncResults;
								if (!this.CheckReadOnlyAndDisablePage())
								{
									this.VerifyCorruptedObject();
								}
							}
							else
							{
								this.DisableRelatedPages(true);
							}
							if (flag)
							{
								this.OnReadDataFailed(EventArgs.Empty);
							}
						}
					}
					this.OnSetActive(EventArgs.Empty);
					this.EnableBulkEditingOnDemand();
					EventHandler setActived = this.SetActived;
					if (setActived != null)
					{
						setActived(this, EventArgs.Empty);
					}
				}
			}
			finally
			{
				this.InputValidationProvider.SuspendRecordingModifiedBindingMembers = false;
				this.suppressIsDirtyChanges = false;
				if (focusOnFirstChild)
				{
					base.Focus();
					base.SelectNextControl(this, true, true, true, false);
				}
			}
		}

		public event EventHandler SetActived;

		private void EnableBulkEditingOnDemand()
		{
			if (!this.enabledBulkEditors)
			{
				AutomatedDataHandlerBase automatedDataHandlerBase = this.DataHandler as AutomatedDataHandlerBase;
				if (automatedDataHandlerBase != null)
				{
					if (automatedDataHandlerBase.EnableBulkEdit)
					{
						this.EnableBulkEditingBindingSource(automatedDataHandlerBase);
					}
					else if (!(automatedDataHandlerBase is AutomatedDataHandler) || (automatedDataHandlerBase as AutomatedDataHandler).SaverExecutionContextFactory is MonadCommandExecutionContextForPropertyPageFactory)
					{
						this.EnableRbacBindingSource(automatedDataHandlerBase);
					}
				}
				this.enabledBulkEditors = true;
			}
		}

		private void EnableBulkEditingBindingSource(AutomatedDataHandlerBase dataHandler)
		{
			BindingManagerBase bindingManagerBase = this.BindingContext[base.BindingSource];
			object dataSource = base.BindingSource.DataSource;
			foreach (object obj in bindingManagerBase.Bindings)
			{
				Binding binding = (Binding)obj;
				IBulkEditor bulkEditor = binding.Control as IBulkEditor;
				if (bulkEditor != null)
				{
					BulkEditorState bulkEditorState = 0;
					if (!dataHandler.IsBulkEditingModifiedParameterName(dataSource, binding.BindingMemberInfo.BindingMember))
					{
						bulkEditorState = (dataHandler.IsBulkEditingSupportedParameterName(dataSource, binding.BindingMemberInfo.BindingMember) ? 1 : 2);
					}
					bulkEditor.BulkEditorAdapter[binding.PropertyName] = bulkEditorState;
				}
			}
		}

		private void EnableRbacBindingSource(AutomatedDataHandlerBase dataHandler)
		{
			if (dataHandler != null)
			{
				BindingManagerBase bindingManagerBase = this.BindingContext[base.BindingSource];
				foreach (object obj in bindingManagerBase.Bindings)
				{
					Binding binding = (Binding)obj;
					IBulkEditor bulkEditor = binding.Control as IBulkEditor;
					if (bulkEditor != null && !dataHandler.HasPermissionForProperty(binding.BindingMemberInfo.BindingMember, binding.DataSourceUpdateMode != DataSourceUpdateMode.Never))
					{
						bulkEditor.BulkEditorAdapter[binding.PropertyName] = 3;
					}
				}
			}
			EventHandler setActived = this.SetActived;
			if (setActived != null)
			{
				setActived(this, EventArgs.Empty);
			}
		}

		protected void ForceIsDirty(bool value)
		{
			bool flag = this.suppressIsDirtyChanges;
			this.suppressIsDirtyChanges = false;
			this.IsDirty = value;
			this.suppressIsDirtyChanges = flag;
		}

		protected virtual void VerifyCorruptedObject()
		{
		}

		protected void DisableRelatedPages(bool removeContext)
		{
			List<ExchangePage> list = new List<ExchangePage>(this.Context.Pages);
			foreach (ExchangePage exchangePage in list)
			{
				exchangePage.Enabled = false;
				if (removeContext)
				{
					exchangePage.Context = null;
				}
			}
		}

		protected virtual void OnReadDataFailed(EventArgs e)
		{
			if (this.ReadDataFailed != null)
			{
				this.ReadDataFailed(this, e);
			}
		}

		protected bool CheckReadOnlyAndDisablePage()
		{
			if (this.Context != null && this.Context.DataHandler != null && this.Context.DataHandler.IsObjectReadOnly)
			{
				base.Enabled = false;
				if (this.Context.Flags.NeedToShowVersionWarning)
				{
					base.ShellUI.ShowMessage(this.Context.DataHandler.ObjectReadOnlyReason);
					this.Context.Flags.NeedToShowVersionWarning = false;
				}
			}
			return !base.Enabled;
		}

		protected virtual void OnSetActive(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ExchangePage.EventSetActive];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
			if (ManagementGuiSqmSession.Instance.Enabled)
			{
				ManagementGuiSqmSession.Instance.AddToStreamDataPoint(SqmDataID.DATAID_EMC_GUI_ACTION, new object[]
				{
					2U,
					base.Name
				});
			}
		}

		public event EventHandler SetActive
		{
			add
			{
				base.Events.AddHandler(ExchangePage.EventSetActive, value);
			}
			remove
			{
				base.Events.RemoveHandler(ExchangePage.EventSetActive, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public bool OnKillActive()
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			if (base.IsHandleCreated && base.Enabled)
			{
				cancelEventArgs.Cancel = !this.ValidateChildren(ValidationConstraints.Enabled | ValidationConstraints.Visible);
				if (!cancelEventArgs.Cancel && !this.IgnorePageValidation)
				{
					List<ValidationError> list = new List<ValidationError>();
					foreach (BindableUserControl control in this.bindableUnpagedControlList)
					{
						this.DealWithErrorForBindableUserControl(list, control);
					}
					this.OnValidating(cancelEventArgs);
					if (!cancelEventArgs.Cancel)
					{
						this.OnValidated(EventArgs.Empty);
						StringBuilder stringBuilder = new StringBuilder();
						foreach (ValidationError validationError in list)
						{
							stringBuilder.Append(" - ").AppendLine(validationError.Description);
						}
						try
						{
							this.OnKillActive(cancelEventArgs);
						}
						catch (Exception ex)
						{
							base.ShowError(ex.Message);
							cancelEventArgs.Cancel = true;
						}
						if (!cancelEventArgs.Cancel && base.Enabled)
						{
							Control control2 = null;
							string text = this.InputValidationProvider.GetErrorProviderMessages(this, ref control2);
							text += stringBuilder;
							if (!string.IsNullOrEmpty(text))
							{
								this.DisplayValidationError(text);
								if (control2 != null)
								{
									control2.Focus();
								}
								cancelEventArgs.Cancel = true;
							}
						}
					}
					if (!cancelEventArgs.Cancel)
					{
						cancelEventArgs.Cancel = !this.ValidateContext();
					}
				}
			}
			this.isActivePage = cancelEventArgs.Cancel;
			if (!cancelEventArgs.Cancel && this.DataHandler != null)
			{
				this.DataHandler.SpecifyParameterNames(this.InputValidationProvider.ModifiedBindingMembers);
				this.InputValidationProvider.ModifiedBindingMembers.Clear();
			}
			return !cancelEventArgs.Cancel;
		}

		private void DealWithErrorForBindableUserControl(List<ValidationError> unhandledErrors, BindableUserControl control)
		{
			if (!control.Visible || !control.Enabled || !string.IsNullOrEmpty(this.GetErrorMessage(control)))
			{
				return;
			}
			foreach (ValidationError validationError in control.Validator.Validate())
			{
				StrongTypeValidationError strongTypeValidationError = validationError as StrongTypeValidationError;
				if (strongTypeValidationError != null && (string.IsNullOrEmpty(strongTypeValidationError.PropertyName) || !this.InputValidationProvider.UpdateErrorProviderTextForProperty(strongTypeValidationError.Description, strongTypeValidationError.PropertyName)))
				{
					unhandledErrors.Add(validationError);
				}
			}
		}

		protected virtual void OnKillActive(CancelEventArgs e)
		{
			CancelEventHandler cancelEventHandler = (CancelEventHandler)base.Events[ExchangePage.EventKillActive];
			if (cancelEventHandler != null)
			{
				cancelEventHandler(this, e);
			}
		}

		public event CancelEventHandler KillActive
		{
			add
			{
				base.Events.AddHandler(ExchangePage.EventKillActive, value);
			}
			remove
			{
				base.Events.RemoveHandler(ExchangePage.EventKillActive, value);
			}
		}

		private bool ValidateContext()
		{
			bool result = true;
			if (!this.IgnorePageValidation && this.Context != null)
			{
				string text = null;
				try
				{
					ValidationError[] errors = this.ValidateContextOnPageTransition();
					List<ValidationError> list = this.FilterErrorsFromCurrentPage(errors);
					if (list.Count > 0)
					{
						text = this.CreateValidateContextErrorMessageFor(list);
					}
				}
				catch (LocalizedException ex)
				{
					text = ex.Message;
				}
				catch (ArgumentException ex2)
				{
					text = ex2.Message;
				}
				if (!string.IsNullOrEmpty(text))
				{
					result = false;
					this.DisplayValidationError(text);
				}
			}
			return result;
		}

		private void DisplayValidationError(string message)
		{
			string[] array = message.Split(new string[]
			{
				Environment.NewLine
			}, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string str in array)
			{
				stringBuilder.AppendLine(" - " + str);
			}
			base.ShowError(Strings.InvalidControls + stringBuilder.ToString());
		}

		protected virtual ValidationError[] ValidateContextOnPageTransition()
		{
			return this.Context.Validate();
		}

		protected LocalizedString CreateValidateContextErrorMessageFor(List<ValidationError> errors)
		{
			object[] array = new object[errors.Count];
			for (int i = 0; i < errors.Count; i++)
			{
				ValidationError validationError = errors[i];
				array[i] = validationError.Description;
				PropertyValidationError propertyValidationError = validationError as PropertyValidationError;
				if (propertyValidationError != null && propertyValidationError.PropertyDefinition != null)
				{
					this.InputValidationProvider.UpdateErrorProviderTextForProperty(propertyValidationError.Description, propertyValidationError.PropertyDefinition.Name);
				}
			}
			return LocalizedString.Join(Environment.NewLine, array);
		}

		private List<ValidationError> FilterErrorsFromCurrentPage(ValidationError[] errors)
		{
			List<ValidationError> list = new List<ValidationError>(errors.Length);
			foreach (ValidationError validationError in errors)
			{
				PropertyValidationError propertyError = validationError as PropertyValidationError;
				if (this.BlockPageSwitchWithError(propertyError))
				{
					list.Add(validationError);
				}
			}
			return list;
		}

		protected virtual bool BlockPageSwitchWithError(PropertyValidationError propertyError)
		{
			return propertyError == null || this.InputValidationProvider.IsBoundToProperty(propertyError.PropertyDefinition.Name);
		}

		protected override void OnLoad(EventArgs e)
		{
			this.RegisterAllBinableUserControl(this);
			base.OnLoad(e);
			if (this.isActivePage)
			{
				this.OnSetActive();
			}
		}

		private void RegisterAllBinableUserControl(Control parent)
		{
			if (parent == null)
			{
				return;
			}
			if (parent != this && parent is BindableUserControl)
			{
				this.RegisterBindableControl(parent as BindableUserControl);
				return;
			}
			foreach (object obj in parent.Controls)
			{
				Control parent2 = (Control)obj;
				this.RegisterAllBinableUserControl(parent2);
			}
		}

		protected override IUIService CreateUIService()
		{
			return new ExchangePage.ExchangePageUIService(this);
		}

		protected bool HasErrorsProviders
		{
			get
			{
				return !string.IsNullOrEmpty(this.GetErrorMessage(this));
			}
		}

		private string GetErrorMessage(Control topControl)
		{
			Control control = null;
			return this.InputValidationProvider.GetErrorProviderMessages(topControl, ref control);
		}

		private static readonly object EventIsDirtyChanged = new object();

		private static readonly object EventSetActive = new object();

		private static readonly object EventKillActive = new object();

		[AccessedThroughProperty("InputValidationProvider")]
		private InputValidationProvider inputValidationProvider;

		private DataContext context;

		private bool isDirty;

		private bool suppressIsDirtyChanges;

		private readonly EventHandler makeDirty;

		private bool isActivePage;

		private IContainer components;

		private bool ignorePageValidation;

		protected IList<BindableUserControl> bindableUnpagedControlList;

		private static readonly object EventContextChanged = new object();

		private string helpTopic;

		private static readonly object EventHelpTopicChanged = new object();

		private bool canCancel = true;

		private bool enabledBulkEditors;

		private class ExchangePageUIService : UIService
		{
			public ExchangePageUIService(ExchangePage page) : base(page)
			{
				this.page = page;
			}

			public override void SetUIDirty()
			{
				this.page.IsDirty = true;
			}

			private ExchangePage page;
		}
	}
}
