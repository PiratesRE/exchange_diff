using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[ToolboxItemFilter("System.Windows.Forms")]
	[ProvideProperty("Enabled", typeof(BindingSource))]
	public class InputValidationProvider : Component, IExtenderProvider
	{
		public InputValidationProvider()
		{
			this.components = new Container();
		}

		public InputValidationProvider(IContainer container) : this()
		{
			container.Add(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ContainerControl = null;
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		bool IExtenderProvider.CanExtend(object extendee)
		{
			return extendee is BindingSource;
		}

		[DefaultValue(false)]
		public bool GetEnabled(BindingSource bindingSource)
		{
			return this.enabledBindingSources.ContainsKey(bindingSource);
		}

		public void SetEnabled(BindingSource bindingSource, bool enabled)
		{
			if (enabled != this.GetEnabled(bindingSource))
			{
				if (enabled)
				{
					ExchangeErrorProvider exchangeErrorProvider = new ExchangeErrorProvider(this.components);
					((ISupportInitialize)exchangeErrorProvider).BeginInit();
					exchangeErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
					exchangeErrorProvider.ContainerControl = this.ContainerControl;
					((ISupportInitialize)exchangeErrorProvider).EndInit();
					this.enabledBindingSources.Add(bindingSource, exchangeErrorProvider);
					if (this.ContainerBindingContext != null)
					{
						this.BindToBindingSource(bindingSource);
						return;
					}
				}
				else
				{
					ExchangeErrorProvider exchangeErrorProvider2 = this.enabledBindingSources[bindingSource];
					this.components.Remove(exchangeErrorProvider2);
					exchangeErrorProvider2.Dispose();
					this.enabledBindingSources.Remove(bindingSource);
					if (this.ContainerBindingContext != null)
					{
						this.UnbindFromBindingSource(bindingSource);
					}
				}
			}
		}

		[DefaultValue(null)]
		public ContainerControl ContainerControl
		{
			get
			{
				return this.containerControl;
			}
			set
			{
				if (this.ContainerControl != value)
				{
					if (this.ContainerControl != null)
					{
						this.ContainerControl.BindingContextChanged -= this.ContainerControl_BindingContextChanged;
						this.ContainerBindingContext = null;
					}
					this.containerControl = value;
					if (this.ContainerControl != null)
					{
						this.ContainerControl.AutoValidate = AutoValidate.EnableAllowFocusChange;
						this.ContainerControl.BindingContextChanged += this.ContainerControl_BindingContextChanged;
						this.ContainerBindingContext = this.ContainerControl.BindingContext;
					}
					foreach (ExchangeErrorProvider exchangeErrorProvider in this.enabledBindingSources.Values)
					{
						exchangeErrorProvider.ContainerControl = value;
					}
				}
			}
		}

		private void ContainerControl_BindingContextChanged(object sender, EventArgs e)
		{
			this.ContainerBindingContext = this.ContainerControl.BindingContext;
		}

		private BindingContext ContainerBindingContext
		{
			get
			{
				return this.containerBindingContext;
			}
			set
			{
				if (this.ContainerBindingContext != value)
				{
					if (this.ContainerBindingContext != null)
					{
						foreach (BindingSource bindingSource in this.enabledBindingSources.Keys)
						{
							this.UnbindFromBindingSource(bindingSource);
						}
					}
					this.containerBindingContext = value;
					if (this.ContainerBindingContext != null)
					{
						foreach (BindingSource bindingSource2 in this.enabledBindingSources.Keys)
						{
							this.BindToBindingSource(bindingSource2);
						}
					}
				}
			}
		}

		private void BindToBindingSource(BindingSource bindingSource)
		{
			BindingManagerBase bindingManagerBase = this.ContainerBindingContext[bindingSource];
			bindingManagerBase.CurrentChanged += this.bindingManager_CurrentChanged;
			bindingManagerBase.BindingComplete += this.BindingCompleted;
			bindingManagerBase.Bindings.CollectionChanged += this.Bindings_CollectionChanged;
			this.removedBindings[bindingSource] = new Dictionary<Control, List<Binding>>();
			bindingSource.ListChanged += this.bindingSource_ListChanged;
			foreach (object obj in bindingManagerBase.Bindings)
			{
				Binding element = (Binding)obj;
				this.Bindings_CollectionChanged(bindingManagerBase.Bindings, new CollectionChangeEventArgs(CollectionChangeAction.Add, element));
			}
		}

		private void UnbindFromBindingSource(BindingSource bindingSource)
		{
			BindingManagerBase bindingManagerBase = this.ContainerBindingContext[bindingSource];
			bindingManagerBase.CurrentChanged -= this.bindingManager_CurrentChanged;
			bindingManagerBase.BindingComplete -= this.BindingCompleted;
			bindingManagerBase.Bindings.CollectionChanged -= this.Bindings_CollectionChanged;
			this.removedBindings.Remove(bindingSource);
			bindingSource.ListChanged -= this.bindingSource_ListChanged;
			foreach (object obj in bindingManagerBase.Bindings)
			{
				Binding element = (Binding)obj;
				this.Bindings_CollectionChanged(bindingManagerBase.Bindings, new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
			}
		}

		private void bindingSource_ListChanged(object sender, ListChangedEventArgs e)
		{
			BindingSource bindingSource = (BindingSource)sender;
			IVersionable versionable = bindingSource.DataSource as IVersionable;
			if (versionable != null)
			{
				if (!this.datasourceVersions.ContainsKey(bindingSource))
				{
					this.datasourceVersions[bindingSource] = versionable.ExchangeVersion;
					this.DisableControlsByVersion(bindingSource, versionable);
					return;
				}
				ExchangeObjectVersion exchangeObjectVersion = this.datasourceVersions[bindingSource];
				if (!exchangeObjectVersion.IsSameVersion(versionable.ExchangeVersion))
				{
					this.datasourceVersions[bindingSource] = versionable.ExchangeVersion;
					this.EnableControlsByVersion(bindingSource, versionable);
					return;
				}
			}
			else
			{
				DataTable dataTable = bindingSource.DataSource as DataTable;
				if (dataTable != null)
				{
					DataObjectStore dataObjectStore = dataTable.ExtendedProperties["DataSourceStore"] as DataObjectStore;
					if (dataObjectStore != null)
					{
						foreach (string text in dataObjectStore.GetKeys())
						{
							IVersionable versionable2 = dataObjectStore.GetDataObject(text) as IVersionable;
							if (versionable2 != null)
							{
								if (this.dataSourceInTableVersions.ContainsKey(text))
								{
									ExchangeObjectVersion exchangeObjectVersion2 = this.dataSourceInTableVersions[text];
									if (!exchangeObjectVersion2.IsSameVersion(versionable2.ExchangeVersion))
									{
										this.dataSourceInTableVersions[text] = versionable2.ExchangeVersion;
										this.EnableControlsByVersion(bindingSource, versionable2);
									}
								}
								else
								{
									this.dataSourceInTableVersions[text] = versionable2.ExchangeVersion;
									this.DisableControlsByVersion(bindingSource, versionable2);
								}
							}
						}
					}
				}
			}
		}

		private void DisableControlsByVersion(BindingSource bindingSource, IVersionable dataSource)
		{
			BindingManagerBase bindingManagerBase = this.ContainerBindingContext[bindingSource];
			for (int i = bindingManagerBase.Bindings.Count - 1; i >= 0; i--)
			{
				Binding binding = bindingManagerBase.Bindings[i];
				ExchangeObjectVersion propertyDefinitionVersion = PropertyConstraintProvider.GetPropertyDefinitionVersion(dataSource, binding.BindingMemberInfo.BindingMember);
				if (dataSource.ExchangeVersion.IsOlderThan(propertyDefinitionVersion))
				{
					if (!this.removedBindings[bindingSource].ContainsKey(binding.Control))
					{
						this.removedBindings[bindingSource][binding.Control] = new List<Binding>();
					}
					this.removedBindings[bindingSource][binding.Control].Add(binding);
					ISpecifyPropertyState specifyPropertyState = binding.Control as ISpecifyPropertyState;
					if (specifyPropertyState != null)
					{
						specifyPropertyState.SetPropertyState(binding.PropertyName, PropertyState.UnsupportedVersion, Strings.FeatureVersionMismatchDescription(propertyDefinitionVersion.ExchangeBuild));
					}
					else
					{
						binding.Control.Enabled = false;
					}
					binding.Control.DataBindings.Remove(binding);
				}
			}
		}

		private void EnableControlsByVersion(BindingSource bindingSource, IVersionable dataSource)
		{
			List<Control> list = new List<Control>();
			foreach (Control item in this.removedBindings[bindingSource].Keys)
			{
				list.Add(item);
			}
			foreach (Control control in list)
			{
				List<Binding> list2 = new List<Binding>(this.removedBindings[bindingSource][control]);
				foreach (Binding binding in list2)
				{
					ExchangeObjectVersion propertyDefinitionVersion = PropertyConstraintProvider.GetPropertyDefinitionVersion(dataSource, binding.BindingMemberInfo.BindingMember);
					if (!dataSource.ExchangeVersion.IsOlderThan(propertyDefinitionVersion))
					{
						control.DataBindings.Add(binding);
						this.removedBindings[bindingSource][control].Remove(binding);
						if (this.removedBindings[bindingSource][control].Count == 0)
						{
							ISpecifyPropertyState specifyPropertyState = control as ISpecifyPropertyState;
							if (specifyPropertyState != null)
							{
								specifyPropertyState.SetPropertyState(binding.PropertyName, PropertyState.Normal, string.Empty);
							}
							else
							{
								control.Enabled = true;
							}
						}
					}
				}
			}
		}

		private void Bindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if (CollectionChangeAction.Refresh != e.Action)
			{
				Binding binding = (Binding)e.Element;
				if (binding.BindableComponent != null)
				{
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(binding.BindableComponent)[binding.PropertyName];
					EventHandler handler = new EventHandler(this.RaiseDataBoundPropertyChanged);
					switch (e.Action)
					{
					case CollectionChangeAction.Add:
						binding.FormattingEnabled = true;
						propertyDescriptor.AddValueChanged(binding.BindableComponent, handler);
						this.AddRemoveEnabledChangedHandler(binding, true);
						if (binding.Control is ExchangeUserControl)
						{
							this.SetUIValidationEnabled(binding.Control as ExchangeUserControl, true);
							return;
						}
						break;
					case CollectionChangeAction.Remove:
					{
						propertyDescriptor.RemoveValueChanged(binding.BindableComponent, handler);
						this.AddRemoveEnabledChangedHandler(binding, false);
						ExchangeUserControl exchangeUserControl = binding.Control as ExchangeUserControl;
						if (exchangeUserControl != null && exchangeUserControl.DataBindings.Count == 1)
						{
							this.SetUIValidationEnabled(exchangeUserControl, false);
						}
						break;
					}
					default:
						return;
					}
				}
			}
		}

		private void AddRemoveEnabledChangedHandler(Binding newBinding, bool add)
		{
			if (newBinding.Control != null)
			{
				newBinding.Control.EnabledChanged -= this.Control_EnabledChanged;
				if (add)
				{
					newBinding.Control.EnabledChanged += this.Control_EnabledChanged;
				}
			}
		}

		private void Control_EnabledChanged(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			if (!control.Enabled)
			{
				foreach (object obj in control.DataBindings)
				{
					Binding binding = (Binding)obj;
					BindingSource bindingSource = binding.DataSource as BindingSource;
					if (bindingSource != null)
					{
						ExchangeErrorProvider errorProvider = this.GetErrorProvider(bindingSource);
						if (errorProvider != null)
						{
							Control errorProviderAnchor = InputValidationProvider.GetErrorProviderAnchor(control);
							errorProvider.SetError(errorProviderAnchor, string.Empty);
						}
					}
				}
			}
		}

		private void UpdateUserControlUIErrorProvider(object sender, UIValidationEventArgs e)
		{
			this.SetUserControlErrorProvider(sender, e.Errors);
		}

		private void SetUserControlErrorProvider(object sender, ICollection<UIValidationError> errors)
		{
			ExchangeUserControl exchangeUserControl = (ExchangeUserControl)sender;
			ICollection<UIValidationError> collection = exchangeUserControl.ValidationErrors ?? ((ICollection<UIValidationError>)UIValidationError.None);
			foreach (UIValidationError uivalidationError in collection)
			{
				bool flag = true;
				foreach (UIValidationError uivalidationError2 in errors)
				{
					if (uivalidationError.ErrorProviderAnchor == uivalidationError2.ErrorProviderAnchor)
					{
						this.validatingControls[exchangeUserControl].SetError(uivalidationError2.ErrorProviderAnchor, uivalidationError2.Description);
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this.validatingControls[exchangeUserControl].SetError(uivalidationError.ErrorProviderAnchor, string.Empty);
				}
			}
			foreach (UIValidationError uivalidationError3 in errors)
			{
				bool flag2 = true;
				foreach (UIValidationError uivalidationError4 in collection)
				{
					if (uivalidationError4.ErrorProviderAnchor == uivalidationError3.ErrorProviderAnchor)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					this.validatingControls[exchangeUserControl].SetError(uivalidationError3.ErrorProviderAnchor, uivalidationError3.Description);
				}
			}
		}

		private void BindingCompleted(object sender, BindingCompleteEventArgs e)
		{
			Control errorProviderAnchor = InputValidationProvider.GetErrorProviderAnchor(e.Binding.Control);
			if (e.BindingCompleteState != BindingCompleteState.Success)
			{
				if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate)
				{
					StrongTypeException ex = e.Exception as StrongTypeException;
					if ((ex == null || ex.IsTargetProperty) && e.Binding.Control.Enabled)
					{
						this.GetErrorProvider((BindingSource)e.Binding.DataSource).SetError(errorProviderAnchor, e.ErrorText);
						this.AddErrorBindingForControl(e.Binding);
					}
					e.Binding.ControlUpdateMode = ControlUpdateMode.Never;
				}
			}
			else
			{
				e.Binding.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
				if (this.RemoveSuccessfulBindingForControl(e.Binding))
				{
					this.GetErrorProvider((BindingSource)e.Binding.DataSource).SetError(errorProviderAnchor, string.Empty);
				}
			}
			e.Cancel = false;
			this.SetModifiedBindingMembers(e);
		}

		private static Control GetErrorProviderAnchor(Control control)
		{
			if (!(control is ExchangeUserControl))
			{
				return control;
			}
			return ((ExchangeUserControl)control).ErrorProviderAnchor;
		}

		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SuspendRecordingModifiedBindingMembers { get; set; }

		private void SetModifiedBindingMembers(BindingCompleteEventArgs e)
		{
			if (!this.SuspendRecordingModifiedBindingMembers && e.BindingCompleteContext.Equals(BindingCompleteContext.DataSourceUpdate))
			{
				object dataSource = e.Binding.DataSource;
				string bindingField = e.Binding.BindingMemberInfo.BindingField;
				object key;
				if (dataSource is BindingSource)
				{
					key = (dataSource as BindingSource).DataSource;
				}
				else
				{
					key = dataSource;
				}
				if (this.ModifiedBindingMembers.ContainsKey(key))
				{
					if (!this.ModifiedBindingMembers[key].Contains(bindingField))
					{
						this.ModifiedBindingMembers[key].Add(bindingField);
						return;
					}
				}
				else
				{
					List<string> list = new List<string>();
					list.Add(bindingField);
					this.ModifiedBindingMembers.Add(key, list);
				}
			}
		}

		internal Dictionary<object, List<string>> ModifiedBindingMembers
		{
			get
			{
				return this.modifiedBindingMembers;
			}
		}

		private void AddErrorBindingForControl(Binding errorBinding)
		{
			Control control = errorBinding.Control;
			if (!this.bindingErrorControls.ContainsKey(control))
			{
				this.bindingErrorControls[control] = new List<Binding>();
			}
			if (!this.bindingErrorControls[control].Contains(errorBinding))
			{
				this.bindingErrorControls[control].Add(errorBinding);
			}
		}

		private bool RemoveSuccessfulBindingForControl(Binding successfulBinding)
		{
			Control control = successfulBinding.Control;
			if (this.bindingErrorControls.ContainsKey(control) && this.bindingErrorControls[control].Contains(successfulBinding))
			{
				this.bindingErrorControls[control].Remove(successfulBinding);
			}
			return !this.bindingErrorControls.ContainsKey(control) || this.bindingErrorControls[control].Count == 0;
		}

		private void bindingManager_CurrentChanged(object sender, EventArgs e)
		{
			BindingManagerBase bindingManagerBase = (BindingManagerBase)sender;
			foreach (object obj in bindingManagerBase.Bindings)
			{
				Binding binding = (Binding)obj;
				binding.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
			}
		}

		private void RaiseDataBoundPropertyChanged(object sender, EventArgs e)
		{
			Control control = sender as Control;
			if (control != null)
			{
				foreach (ExchangeErrorProvider exchangeErrorProvider in this.enabledBindingSources.Values)
				{
					Control errorProviderAnchor = InputValidationProvider.GetErrorProviderAnchor(control);
					exchangeErrorProvider.SetError(errorProviderAnchor, "");
				}
			}
			EventHandler eventHandler = (EventHandler)base.Events[InputValidationProvider.EventDataBoundPropertyChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler DataBoundPropertyChanged
		{
			add
			{
				SynchronizedDelegate.Combine(base.Events, InputValidationProvider.EventDataBoundPropertyChanged, value);
			}
			remove
			{
				SynchronizedDelegate.Remove(base.Events, InputValidationProvider.EventDataBoundPropertyChanged, value);
			}
		}

		public string GetErrorProviderMessages(Control topControl, ref Control firstErrorControl)
		{
			if (topControl == null)
			{
				throw new ArgumentNullException("topControl");
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<Control> list = new List<Control>();
			List<ExchangeErrorProvider> list2 = new List<ExchangeErrorProvider>(this.validatingControls.Values);
			list2.AddRange(this.enabledBindingSources.Values);
			ChildrenControlCollector.GetAllControlsInActualTabOrder(topControl, list, true);
			foreach (ExchangeErrorProvider exchangeErrorProvider in list2)
			{
				foreach (Control control in list)
				{
					string error = exchangeErrorProvider.GetError(control);
					if (!string.IsNullOrEmpty(error))
					{
						stringBuilder.AppendLine(error);
						if (firstErrorControl == null)
						{
							firstErrorControl = control;
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		public void SetUIValidationEnabled(ExchangeUserControl userControl, bool enabled)
		{
			if (enabled)
			{
				if (!this.validatingControls.ContainsKey(userControl))
				{
					ExchangeErrorProvider exchangeErrorProvider = new ExchangeErrorProvider(this.components);
					((ISupportInitialize)exchangeErrorProvider).BeginInit();
					exchangeErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
					exchangeErrorProvider.ContainerControl = this.ContainerControl;
					((ISupportInitialize)exchangeErrorProvider).EndInit();
					this.validatingControls.Add(userControl, exchangeErrorProvider);
					userControl.ValidationErrorsChanging += this.UpdateUserControlUIErrorProvider;
					return;
				}
			}
			else if (this.validatingControls.ContainsKey(userControl))
			{
				this.validatingControls.Remove(userControl);
				userControl.ValidationErrorsChanging -= this.UpdateUserControlUIErrorProvider;
			}
		}

		public ExchangeErrorProvider GetErrorProvider(BindingSource bindingSource)
		{
			ExchangeErrorProvider result = null;
			this.enabledBindingSources.TryGetValue(bindingSource, out result);
			return result;
		}

		public void WriteBindings()
		{
			foreach (BindingSource dataSource in this.enabledBindingSources.Keys)
			{
				BindingManagerBase bindingManagerBase = this.ContainerBindingContext[dataSource];
				foreach (object obj in bindingManagerBase.Bindings)
				{
					Binding binding = (Binding)obj;
					if (binding.DataSourceUpdateMode != DataSourceUpdateMode.Never && binding.Control.Enabled && binding.Control.Visible)
					{
						binding.WriteValue();
					}
				}
			}
		}

		private Binding GetBindingGivenPropertyName(string propertyName)
		{
			Binding binding = null;
			foreach (BindingSource dataSource in this.enabledBindingSources.Keys)
			{
				BindingManagerBase bindingManagerBase = this.ContainerBindingContext[dataSource];
				foreach (object obj in bindingManagerBase.Bindings)
				{
					Binding binding2 = (Binding)obj;
					if (binding2.DataSourceUpdateMode != DataSourceUpdateMode.Never && binding2.Control.Enabled && binding2.Control.Visible && binding2.BindingMemberInfo.BindingMember.CompareTo(propertyName) == 0)
					{
						binding = binding2;
						break;
					}
				}
				if (binding != null)
				{
					break;
				}
			}
			return binding;
		}

		public bool IsBoundToProperty(string propertyName)
		{
			return null != this.GetBindingGivenPropertyName(propertyName);
		}

		public bool UpdateErrorProviderTextForProperty(string errorMessage, string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}
			if (string.IsNullOrEmpty(errorMessage))
			{
				throw new ArgumentNullException("errorMessage");
			}
			bool result = false;
			Binding bindingGivenPropertyName = this.GetBindingGivenPropertyName(propertyName);
			if (bindingGivenPropertyName != null)
			{
				ExchangeErrorProvider exchangeErrorProvider = this.enabledBindingSources[(BindingSource)bindingGivenPropertyName.DataSource];
				StringBuilder stringBuilder = new StringBuilder(exchangeErrorProvider.GetError(bindingGivenPropertyName.Control));
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(errorMessage);
				Control errorProviderAnchor = InputValidationProvider.GetErrorProviderAnchor(bindingGivenPropertyName.Control);
				exchangeErrorProvider.SetError(errorProviderAnchor, stringBuilder.ToString());
				result = true;
			}
			return result;
		}

		private static readonly object EventDataBoundPropertyChanged = new object();

		private IContainer components;

		private ContainerControl containerControl;

		private BindingContext containerBindingContext;

		private Dictionary<BindingSource, ExchangeErrorProvider> enabledBindingSources = new Dictionary<BindingSource, ExchangeErrorProvider>();

		private Dictionary<BindingSource, ExchangeObjectVersion> datasourceVersions = new Dictionary<BindingSource, ExchangeObjectVersion>();

		private Dictionary<string, ExchangeObjectVersion> dataSourceInTableVersions = new Dictionary<string, ExchangeObjectVersion>();

		private Dictionary<BindingSource, Dictionary<Control, List<Binding>>> removedBindings = new Dictionary<BindingSource, Dictionary<Control, List<Binding>>>();

		private Dictionary<ExchangeUserControl, ExchangeErrorProvider> validatingControls = new Dictionary<ExchangeUserControl, ExchangeErrorProvider>();

		private Dictionary<Control, List<Binding>> bindingErrorControls = new Dictionary<Control, List<Binding>>();

		private Dictionary<object, List<string>> modifiedBindingMembers = new Dictionary<object, List<string>>();
	}
}
