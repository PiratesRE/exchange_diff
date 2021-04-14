using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Services;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeUserControl : CustomUserControl, IServiceProvider, IBulkEditor, IBulkEditSupport
	{
		public ICollection<UIValidationError> ValidationErrors
		{
			get
			{
				return this.validationErrors;
			}
			protected set
			{
				if (this.validationErrors != value)
				{
					if (this.ValidationErrorsChanging != null)
					{
						this.ValidationErrorsChanging(this, new UIValidationEventArgs(value));
					}
					this.validationErrors = value;
				}
			}
		}

		public virtual Control ErrorProviderAnchor
		{
			get
			{
				return this;
			}
		}

		public event ExchangeUserControl.UIValidationEventHandler ValidationErrorsChanging;

		public ExchangeUserControl()
		{
			base.SetStyle(Theme.UserPaintStyle | ControlStyles.CacheText, true);
			base.Name = "ExchangeUserControl";
		}

		protected bool HasComponents
		{
			get
			{
				return this.components != null && this.components.Count != 0;
			}
		}

		protected IContainer Components
		{
			get
			{
				if (this.components == null)
				{
					this.components = new ServicedContainer(base.DesignMode ? null : this);
				}
				return this.components;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public Control FocusedControl
		{
			get
			{
				Control focusedControl = ExchangeUserControl.GetFocusedControl();
				Control control = focusedControl;
				while (control != null && control != this)
				{
					control = control.Parent;
				}
				if (control != this)
				{
					return null;
				}
				return focusedControl;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			if (serviceType == typeof(IUIService))
			{
				return this.ShellUI;
			}
			return this.GetService(serviceType);
		}

		protected ISelectionService GetSelectionService()
		{
			return (ISelectionService)this.GetService(typeof(ISelectionService));
		}

		protected IServiceContainer GetServiceContainer()
		{
			return (IServiceContainer)this.GetService(typeof(IServiceContainer));
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

		public virtual DialogResult ShowDialog(Form form)
		{
			return this.ShellUI.ShowDialog(form);
		}

		public DialogResult ShowDialog(ExchangePropertyPageControl propertyPage)
		{
			return this.ShowDialog(ExchangeUserControl.WrapPageAsDialog(propertyPage));
		}

		public static ExchangePropertyPageControl WrapUserControlAsPage(BindableUserControl control)
		{
			ExchangePropertyPageControl exchangePropertyPageControl = new ExchangePropertyPageControl();
			exchangePropertyPageControl.SuspendLayout();
			control.Dock = DockStyle.Fill;
			exchangePropertyPageControl.Padding = new Padding(13, 12, 0, 12);
			exchangePropertyPageControl.Controls.Add(control);
			exchangePropertyPageControl.Text = control.Text;
			exchangePropertyPageControl.AutoSize = true;
			exchangePropertyPageControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			exchangePropertyPageControl.AutoScaleDimensions = ExchangeUserControl.DefaultAutoScaleDimension;
			exchangePropertyPageControl.AutoScaleMode = AutoScaleMode.Font;
			exchangePropertyPageControl.ResumeLayout(false);
			exchangePropertyPageControl.PerformLayout();
			exchangePropertyPageControl.HelpTopic = control.GetType().FullName;
			return exchangePropertyPageControl;
		}

		public static PropertyPageDialog WrapUserControlAsDialog(BindableUserControl control)
		{
			return ExchangeUserControl.WrapPageAsDialog(ExchangeUserControl.WrapUserControlAsPage(control));
		}

		public static PropertyPageDialog WrapPageAsDialog(ExchangePropertyPageControl page)
		{
			return new PropertyPageDialog(page);
		}

		public DialogResult ShowDialog(BindableUserControl control)
		{
			return this.ShowDialog(ExchangeUserControl.WrapUserControlAsDialog(control));
		}

		public DialogResult ShowDialog(string caption, string dialogHelpTopic, ExchangePropertyPageControl[] pages)
		{
			foreach (ExchangePropertyPageControl exchangePropertyPageControl in pages)
			{
				exchangePropertyPageControl.AutoScaleDimensions = ExchangeUserControl.DefaultAutoScaleDimension;
				exchangePropertyPageControl.AutoScaleMode = AutoScaleMode.Font;
			}
			DialogResult result;
			using (PropertySheetDialog propertySheetDialog = new PropertySheetDialog(caption, pages))
			{
				propertySheetDialog.HelpTopic = dialogHelpTopic;
				result = this.ShowDialog(propertySheetDialog);
			}
			return result;
		}

		public IProgress CreateProgress(string operationName)
		{
			IProgressProvider progressProvider = (IProgressProvider)this.GetService(typeof(IProgressProvider));
			if (progressProvider != null)
			{
				return progressProvider.CreateProgress(operationName);
			}
			return NullProgress.Value;
		}

		public static string RemoveAccelerator(string value)
		{
			if (ExchangeUserControl.removeDBCSAcceleratorRegEx.IsMatch(value))
			{
				return ExchangeUserControl.removeDBCSAcceleratorRegEx.Replace(value, "");
			}
			return ExchangeUserControl.removeAcceleratorRegEx.Replace(value, "$1");
		}

		public static Control GetFocusedControl()
		{
			IntPtr focus = UnsafeNativeMethods.GetFocus();
			if (!(focus == IntPtr.Zero))
			{
				return Control.FromChildHandle(focus);
			}
			return null;
		}

		protected override void OnValidating(CancelEventArgs e)
		{
			this.UpdateError(true);
			base.OnValidating(e);
		}

		public void UpdateError()
		{
			this.UpdateError(false);
		}

		private void UpdateError(bool force)
		{
			if (force || (this.ValidationErrors != null && this.ValidationErrors.Count > 0))
			{
				UIValidationError[] array = this.GetValidationErrors() ?? UIValidationError.None;
				if (force)
				{
					this.ValidationErrors = array;
					return;
				}
				List<UIValidationError> list = new List<UIValidationError>();
				foreach (UIValidationError uivalidationError in array)
				{
					foreach (UIValidationError uivalidationError2 in this.ValidationErrors)
					{
						if (uivalidationError.ErrorProviderAnchor == uivalidationError2.ErrorProviderAnchor)
						{
							list.Add(uivalidationError);
						}
					}
				}
				this.ValidationErrors = list;
			}
		}

		protected virtual UIValidationError[] GetValidationErrors()
		{
			return UIValidationError.None;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			if (!base.Enabled)
			{
				this.ValidationErrors = UIValidationError.None;
			}
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			bool vscroll = base.VScroll;
			base.OnLayout(e);
			if (!vscroll && base.VScroll)
			{
				this.AdjustFormScrollbars(this.AutoScroll);
			}
		}

		public new Padding Padding
		{
			get
			{
				return this.originPadding;
			}
			set
			{
				this.originPadding = value;
				base.Padding = LayoutHelper.RTLPadding(this.originPadding, this);
			}
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.Padding = LayoutHelper.RTLPadding(this.originPadding, this);
			base.OnRightToLeftChanged(e);
		}

		BulkEditorAdapter IBulkEditor.BulkEditorAdapter
		{
			get
			{
				if (this.bulkEditorAdapter == null)
				{
					this.bulkEditorAdapter = this.CreateBulkEditorAdapter();
				}
				return this.bulkEditorAdapter;
			}
		}

		protected virtual BulkEditorAdapter CreateBulkEditorAdapter()
		{
			return new UserControlBulkEditorAdapter(this);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Dictionary<string, HashSet<Control>> ExposedPropertyRelatedControls
		{
			get
			{
				Dictionary<string, HashSet<Control>> dictionary = new Dictionary<string, HashSet<Control>>();
				if (!string.IsNullOrEmpty(this.ExposedPropertyName))
				{
					dictionary.Add(this.ExposedPropertyName, this.GetChildControls());
				}
				return dictionary;
			}
		}

		protected virtual string ExposedPropertyName
		{
			get
			{
				return string.Empty;
			}
		}

		protected HashSet<Control> GetChildControls()
		{
			HashSet<Control> directChildControls = this.GetDirectChildControls(this);
			foreach (object obj in base.Controls)
			{
				Control control = (Control)obj;
				if (typeof(TableLayoutPanel).IsAssignableFrom(control.GetType()))
				{
					directChildControls.UnionWith(this.GetDirectChildControls(control));
				}
			}
			return directChildControls;
		}

		private HashSet<Control> GetDirectChildControls(Control parentControl)
		{
			HashSet<Control> hashSet = new HashSet<Control>();
			foreach (object obj in parentControl.Controls)
			{
				Control control = (Control)obj;
				Control control2 = control;
				if (!typeof(TableLayoutPanel).IsAssignableFrom(control2.GetType()) && !typeof(Label).IsAssignableFrom(control2.GetType()))
				{
					if (control2 is AutoSizePanel && control2.Controls.Count == 1 && control2.Controls[0] is ExchangeTextBox)
					{
						control2 = control2.Controls[0];
					}
					hashSet.Add(control2);
				}
			}
			return hashSet;
		}

		protected void NotifyExposedPropertyIsModified()
		{
			this.NotifyExposedPropertyIsModified(this.ExposedPropertyName);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public event EventHandler<PropertyChangedEventArgs> UserModified;

		protected void NotifyExposedPropertyIsModified(string propertyName)
		{
			if (this.UserModified != null && !string.IsNullOrEmpty(propertyName))
			{
				this.UserModified(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		internal static readonly SizeF DefaultAutoScaleDimension = new SizeF(6f, 13f);

		private ICollection<UIValidationError> validationErrors;

		private ServicedContainer components;

		private static Regex removeAcceleratorRegEx = new Regex("&([^&])", RegexOptions.Compiled);

		private static Regex removeDBCSAcceleratorRegEx = new Regex("\\(&([^&])\\)", RegexOptions.Compiled);

		private Padding originPadding;

		protected BulkEditorAdapter bulkEditorAdapter;

		public delegate void UIValidationEventHandler(object sender, UIValidationEventArgs e);
	}
}
