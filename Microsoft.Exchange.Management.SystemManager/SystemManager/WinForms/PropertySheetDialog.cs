using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class PropertySheetDialog : ExchangeForm
	{
		public PropertySheetDialog()
		{
			this.InitializeComponent();
			base.Size = new Size(443, 507);
			this.cancelButton.Text = Strings.Cancel;
			this.okButton.Text = Strings.Ok;
			this.applyButton.Text = Strings.PropertySheetDialogApply;
			this.helpButton.Text = Strings.PropertySheetDialogHelp;
			if (PropertySheetDialog.lockImage == null)
			{
				Size empty = Size.Empty;
				empty.Width = Math.Min(this.lockButton.Width, this.lockButton.Height);
				empty.Height = empty.Width;
				PropertySheetDialog.lockImage = IconLibrary.ToBitmap(Icons.LockIcon, empty);
				PropertySheetDialog.commandLogPropertyExposureEnabledImage = IconLibrary.ToBitmap(Icons.CommandLogPropertyExposureEnabled, empty);
				PropertySheetDialog.commandLogPropertyExposureDisabledImage = IconLibrary.ToBitmap(Icons.CommandLogPropertyExposureDisabled, empty);
			}
			this.commandExposureButton.Image = PropertySheetDialog.commandLogPropertyExposureDisabledImage;
			this.lockButton.Image = PropertySheetDialog.lockImage;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(this.lockButton, Strings.ShowLockButtonToolTipText);
			toolTip.SetToolTip(this.commandExposureButton, Strings.ShowEMSCommand);
			this.applyButton.Enabled = false;
			this.lockButton.Visible = false;
			this.lockButton.FlatStyle = FlatStyle.Flat;
			this.lockButton.FlatAppearance.BorderSize = 0;
			this.lockButton.FlatAppearance.BorderColor = this.lockButton.BackColor;
			this.lockButton.FlatAppearance.MouseOverBackColor = this.lockButton.BackColor;
			this.lockButton.FlatAppearance.MouseDownBackColor = this.lockButton.BackColor;
			this.commandExposureButton.Enabled = false;
			this.commandExposureButton.FlatStyle = FlatStyle.Flat;
			this.commandExposureButton.FlatAppearance.BorderSize = 0;
			this.commandExposureButton.FlatAppearance.BorderColor = this.commandExposureButton.BackColor;
			this.applyButton.Click += delegate(object param0, EventArgs param1)
			{
				this.PerformApply();
				this.SetActivePage((ExchangePropertyPageControl)this.tabControl.SelectedTab.Tag);
			};
			this.commandExposureButton.MouseEnter += delegate(object param0, EventArgs param1)
			{
				if (this.commandExposureButton.Enabled)
				{
					this.commandExposureButton.FlatStyle = FlatStyle.Standard;
				}
			};
			this.commandExposureButton.MouseLeave += delegate(object param0, EventArgs param1)
			{
				this.commandExposureButton.FlatStyle = FlatStyle.Flat;
			};
			this.commandExposureButton.Click += delegate(object param0, EventArgs param1)
			{
				if (this.isValid && this.isDirty && ((ExchangePropertyPageControl)this.tabControl.SelectedTab.Tag).OnKillActive())
				{
					List<DataHandler> list = new List<DataHandler>();
					ExchangePropertyPageControl[] array = (ExchangePropertyPageControl[])this.tabControl.Tag;
					foreach (ExchangePropertyPageControl exchangePropertyPageControl in array)
					{
						if (exchangePropertyPageControl.IsHandleCreated && exchangePropertyPageControl.Context != null && exchangePropertyPageControl.Context.IsDirty)
						{
							if (!exchangePropertyPageControl.TryApply())
							{
								return;
							}
							if (!list.Contains(exchangePropertyPageControl.DataHandler))
							{
								list.Add(exchangePropertyPageControl.DataHandler);
							}
						}
					}
					StringBuilder stringBuilder = new StringBuilder();
					foreach (DataHandler dataHandler in list)
					{
						stringBuilder.Append(dataHandler.CommandToRun);
					}
					using (PropertyPageDialog propertyPageDialog = new PropertyPageDialog(new PropertyPageCommandExposureControl
					{
						CommandToShow = stringBuilder.ToString()
					}))
					{
						propertyPageDialog.CancelVisible = false;
						((ExchangePage)this.tabControl.SelectedTab.Tag).ShowDialog(propertyPageDialog);
					}
				}
			};
			this.helpButton.Click += delegate(object param0, EventArgs param1)
			{
				this.OnHelpRequested(new HelpEventArgs(Point.Empty));
			};
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled && this.tabControl.SelectedTab != null)
			{
				ExchangePage exchangePage = (ExchangePropertyPageControl)this.tabControl.SelectedTab.Tag;
				if (exchangePage != null)
				{
					ExchangeHelpService.ShowHelpFromPage(exchangePage);
					hevent.Handled = true;
				}
			}
			base.OnHelpRequested(hevent);
		}

		public PropertySheetDialog(string caption, ExchangePropertyPageControl[] pages) : this()
		{
			this.tabControl.Tag = pages;
			this.Text = caption;
			this.tabControl.Multiline = true;
			this.tabControl.SizeMode = TabSizeMode.FillToRight;
			foreach (ExchangePropertyPageControl exchangePropertyPageControl in pages)
			{
				this.maxPageSize = new Size(Math.Max(this.maxPageSize.Width, exchangePropertyPageControl.Width), Math.Max(this.maxPageSize.Height, exchangePropertyPageControl.Height));
				TabPage tabPage = new TabPage();
				tabPage.Tag = exchangePropertyPageControl;
				tabPage.Text = exchangePropertyPageControl.Text;
				tabPage.DataBindings.Add("Text", exchangePropertyPageControl, "Text");
				exchangePropertyPageControl.Dock = DockStyle.Fill;
				tabPage.Controls.Add(exchangePropertyPageControl);
				exchangePropertyPageControl.IsDirtyChanged += this.page_IsDirtyChanged;
				exchangePropertyPageControl.SetActived += this.page_SetActived;
				this.TabPages.Add(tabPage);
			}
			base.Load += this.PropertySheetDialog_Load;
			this.tabControl.Selecting += this.tabControl_Selecting;
			this.tabControl.Deselecting += this.tabControl_Deselecting;
			this.Apply += this.PropertySheetDialog_Apply;
		}

		private void PropertySheetDialog_Load(object sender, EventArgs e)
		{
			ExchangePropertyPageControl exchangePropertyPageControl = (ExchangePropertyPageControl)this.tabControl.SelectedTab.Tag;
			if (!this.maxPageSize.IsEmpty)
			{
				Size clientSize = exchangePropertyPageControl.ClientSize;
				Size sz = clientSize - this.maxPageSize;
				base.ClientSize -= sz;
			}
			this.SetActivePage(exchangePropertyPageControl);
		}

		private void page_IsDirtyChanged(object sender, EventArgs e)
		{
			this.IsDirty |= ((ExchangePropertyPageControl)sender).IsDirty;
		}

		private void page_SetActived(object sender, EventArgs e)
		{
			this.lockButton.Visible = ((ExchangePropertyPageControl)sender).HasLockedControls;
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			this.SetActivePage((ExchangePropertyPageControl)e.TabPage.Tag, this.tabControl.RequireFocusOnActivePageFirstChild);
		}

		private void tabControl_Deselecting(object sender, TabControlCancelEventArgs e)
		{
			e.Cancel = !((ExchangePropertyPageControl)e.TabPage.Tag).OnKillActive();
		}

		private void PropertySheetDialog_Apply(object sender, CancelEventArgs e)
		{
			e.Cancel = !((ExchangePropertyPageControl)this.tabControl.SelectedTab.Tag).OnKillActive();
			if (!e.Cancel)
			{
				ExchangePropertyPageControl[] array = (ExchangePropertyPageControl[])this.tabControl.Tag;
				foreach (ExchangePropertyPageControl exchangePropertyPageControl in array)
				{
					if (exchangePropertyPageControl.IsHandleCreated)
					{
						exchangePropertyPageControl.Apply(e);
						if (e.Cancel)
						{
							return;
						}
					}
				}
			}
		}

		private void CloseOnClick(object sender, EventArgs e)
		{
			base.Close();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabControl.TabPageCollection TabPages
		{
			get
			{
				return this.tabControl.TabPages;
			}
		}

		[DefaultValue(true)]
		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
			set
			{
				if (this.IsValid != value)
				{
					this.isValid = value;
					this.applyButton.Enabled = (this.IsValid && this.IsDirty);
					this.commandExposureButton.Enabled = this.applyButton.Enabled;
					this.commandExposureButton.Image = (this.commandExposureButton.Enabled ? PropertySheetDialog.commandLogPropertyExposureEnabledImage : PropertySheetDialog.commandLogPropertyExposureDisabledImage);
					this.okButton.Enabled = this.IsValid;
					this.OnIsValidChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIsValidChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[PropertySheetDialog.EventIsValidChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IsValidChanged
		{
			add
			{
				base.Events.AddHandler(PropertySheetDialog.EventIsValidChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(PropertySheetDialog.EventIsValidChanged, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[DefaultValue(false)]
		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
			set
			{
				if (this.IsDirty != value)
				{
					this.isDirty = value;
					this.applyButton.Enabled = (this.IsValid && this.IsDirty);
					this.commandExposureButton.Enabled = this.applyButton.Enabled;
					this.commandExposureButton.Image = (this.commandExposureButton.Enabled ? PropertySheetDialog.commandLogPropertyExposureEnabledImage : PropertySheetDialog.commandLogPropertyExposureDisabledImage);
					this.OnIsDirtyChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIsDirtyChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[PropertySheetDialog.EventIsDirtyChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IsDirtyChanged
		{
			add
			{
				base.Events.AddHandler(PropertySheetDialog.EventIsDirtyChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(PropertySheetDialog.EventIsDirtyChanged, value);
			}
		}

		public void PerformApply()
		{
			if (this.IsValid && this.IsDirty)
			{
				this.InternalPerformApply();
			}
		}

		private bool InternalPerformApply()
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
			this.OnApply(cancelEventArgs);
			if (!cancelEventArgs.Cancel)
			{
				this.IsDirty = false;
			}
			return !cancelEventArgs.Cancel;
		}

		protected virtual void OnApply(CancelEventArgs e)
		{
			CancelEventHandler cancelEventHandler = (CancelEventHandler)base.Events[PropertySheetDialog.EventApply];
			if (cancelEventHandler != null)
			{
				cancelEventHandler(this, e);
			}
		}

		public event CancelEventHandler Apply
		{
			add
			{
				base.Events.AddHandler(PropertySheetDialog.EventApply, value);
			}
			remove
			{
				base.Events.RemoveHandler(PropertySheetDialog.EventApply, value);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.SelectNextControl(this.tabControl, true, true, true, true);
			base.OnLoad(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!e.Cancel && base.DialogResult == DialogResult.OK)
			{
				e.Cancel = !this.InternalPerformApply();
				if (e.Cancel && !base.Modal)
				{
					base.DialogResult = DialogResult.None;
				}
			}
		}

		private void tabControl_ControlAdded(object sender, ControlEventArgs e)
		{
			TabPage tabPage = e.Control as TabPage;
			tabPage.BackColor = Color.Transparent;
			tabPage.UseVisualStyleBackColor = false;
		}

		private void SetActivePage(ExchangePage page, bool focusOnFirstChild)
		{
			base.BeginInvoke(new PropertySheetDialog.OnSetActiveMethodInvoker(page.OnSetActive), new object[]
			{
				focusOnFirstChild
			});
		}

		private void SetActivePage(ExchangePage page)
		{
			base.BeginInvoke(new PropertySheetDialog.OnSetActiveMethodInvoker(page.OnSetActive), new object[]
			{
				true
			});
		}

		private static Image commandLogPropertyExposureEnabledImage;

		private static Image commandLogPropertyExposureDisabledImage;

		private static Image lockImage;

		private Size maxPageSize = Size.Empty;

		private bool isValid = true;

		private static readonly object EventIsValidChanged = new object();

		private bool isDirty;

		private static readonly object EventIsDirtyChanged = new object();

		private static readonly object EventApply = new object();

		private delegate void OnSetActiveMethodInvoker(bool focusOnFirstChild);
	}
}
