using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeComboBox : ComboBox, IBulkEditor, IOwnerDrawBulkEditSupport, IBulkEditSupport, IFormatModeProvider, IBindableComponent, IComponent, IDisposable
	{
		public ExchangeComboBox()
		{
			object oldSelectedItem = null;
			base.DataSourceChanged += delegate(object param0, EventArgs param1)
			{
				oldSelectedItem = this.SelectedItem;
			};
			base.SelectedValueChanged += delegate(object param0, EventArgs param1)
			{
				if (oldSelectedItem != null)
				{
					this.SelectedItem = oldSelectedItem;
					oldSelectedItem = null;
				}
			};
			base.DataBindings.CollectionChanged += this.DataBindings_CollectionChanged;
		}

		private void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if (!base.DesignMode && base.DropDownStyle != ComboBoxStyle.DropDownList)
			{
				Binding binding = (Binding)e.Element;
				if (e.Action == CollectionChangeAction.Add && binding.PropertyName == "Text" && this.constraintProvider == null)
				{
					this.constraintProvider = new TextBoxConstraintProvider(this, this);
				}
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			base.Select();
		}

		protected override void OnDropDown(EventArgs e)
		{
			this.UpdateDropDownWidth();
			base.OnDropDown(e);
		}

		private void UpdateDropDownWidth()
		{
			float num = 0f;
			foreach (object item in base.Items)
			{
				num = Math.Max(num, (float)TextRenderer.MeasureText(base.GetItemText(item), this.Font).Width);
			}
			num += (float)((base.MaxDropDownItems < base.Items.Count) ? SystemInformation.VerticalScrollBarWidth : 0);
			int num2 = (int)decimal.Round((decimal)num, 0);
			num2 = Math.Min(Screen.GetWorkingArea(this).Width, num2);
			base.DropDownWidth = Math.Max(num2, base.Width);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 308)
			{
				this.PositionDropDown(m.LParam);
			}
			base.WndProc(ref m);
			if (m.Msg == 15)
			{
				this.OnPainted(EventArgs.Empty);
				return;
			}
			if (m.Msg == 7)
			{
				this.OnFocusSetted(EventArgs.Empty);
				return;
			}
			if (m.Msg == 8)
			{
				this.OnFocusKilled(EventArgs.Empty);
			}
		}

		private void PositionDropDown(IntPtr hWnd)
		{
			Rectangle workingArea = Screen.GetWorkingArea(this);
			Rectangle rectangle = base.RectangleToScreen(base.ClientRectangle);
			int num = rectangle.Left - workingArea.X;
			if (base.DropDownWidth > workingArea.Width - num && rectangle.Right < workingArea.Right)
			{
				int num2 = base.ItemHeight * Math.Min(base.Items.Count, base.MaxDropDownItems);
				int num3 = rectangle.Top - workingArea.Top - 2;
				int num4 = workingArea.Height - rectangle.Bottom;
				int num5;
				if (num2 <= num4 || num3 <= num4)
				{
					num5 = rectangle.Bottom;
				}
				else
				{
					num5 = num3 - num2 + workingArea.Top;
				}
				int num6 = workingArea.Right - base.DropDownWidth;
				UnsafeNativeMethods.SetWindowPos(hWnd, IntPtr.Zero, num6, num5, 0, 0, 1U);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<PropertyChangedEventArgs> UserModified;

		private void OnUserModified(EventArgs e)
		{
			if (this.UserModified != null)
			{
				this.UserModified(this, new PropertyChangedEventArgs("SelectedValue"));
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler Painted;

		private void OnPainted(EventArgs e)
		{
			if (this.Painted != null)
			{
				this.Painted(this, e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public event EventHandler FocusSetted;

		private void OnFocusSetted(EventArgs e)
		{
			if (this.FocusSetted != null)
			{
				this.FocusSetted(this, e);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler FocusKilled;

		private void OnFocusKilled(EventArgs e)
		{
			if (this.FocusKilled != null)
			{
				this.FocusKilled(this, e);
			}
		}

		protected override void OnSelectionChangeCommitted(EventArgs e)
		{
			base.OnSelectionChangeCommitted(e);
			this.OnUserModified(EventArgs.Empty);
		}

		BulkEditorAdapter IBulkEditor.BulkEditorAdapter
		{
			get
			{
				if (this.bulkEditorAdapter == null)
				{
					this.bulkEditorAdapter = new ComboBoxBulkEditorAdapter(this);
				}
				return this.bulkEditorAdapter;
			}
		}

		[DefaultValue(0)]
		public DisplayFormatMode FormatMode
		{
			get
			{
				return this.formatMode;
			}
			set
			{
				if (this.formatMode != value)
				{
					this.formatMode = value;
					this.OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnFormatChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ExchangeComboBox.EventFormatModeChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler FormatModeChanged
		{
			add
			{
				base.Events.AddHandler(ExchangeComboBox.EventFormatModeChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ExchangeComboBox.EventFormatModeChanged, value);
			}
		}

		void IFormatModeProvider.add_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged += A_1;
		}

		void IFormatModeProvider.remove_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged -= A_1;
		}

		private TextBoxConstraintProvider constraintProvider;

		private static readonly object EventFormatModeChanged = new object();

		private DisplayFormatMode formatMode;

		private ComboBoxBulkEditorAdapter bulkEditorAdapter;
	}
}
