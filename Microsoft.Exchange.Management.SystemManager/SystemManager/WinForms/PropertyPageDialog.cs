using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class PropertyPageDialog : ExchangeDialog
	{
		public PropertyPageDialog()
		{
			this.InitializeComponent();
			this.AutoSizeControl = true;
		}

		public PropertyPageDialog(ExchangePropertyPageControl propertyPage) : this()
		{
			this.RegisterPropertyPage(propertyPage);
		}

		protected override string DefaultHelpTopic
		{
			get
			{
				return string.Empty;
			}
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled && base.HelpVisible && this.Control != null && string.IsNullOrEmpty(base.HelpTopic))
			{
				ExchangeHelpService.ShowHelpFromPage(this.Control);
				hevent.Handled = true;
			}
			base.OnHelpRequested(hevent);
		}

		[DefaultValue(null)]
		public ExchangePropertyPageControl Control
		{
			get
			{
				return this.control;
			}
		}

		protected void RegisterPropertyPage(ExchangePropertyPageControl ctrl)
		{
			if (ctrl != null)
			{
				ctrl.TabIndex = 0;
				base.Controls.Add(ctrl);
				base.Controls.SetChildIndex(ctrl, 0);
				ctrl.IsDirtyChanged += this.page_IsDirtyChanged;
				base.Name += ctrl.Name;
				this.Text = ctrl.Text;
				ctrl.SetActived += this.page_SetActived;
				ctrl.OnSetActive();
				this.control = ctrl;
			}
		}

		private void page_SetActived(object sender, EventArgs e)
		{
			base.LockVisible = ((ExchangePropertyPageControl)sender).HasLockedControls;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.SuspendLayout();
			this.FitSizeToContent();
			base.ResumeLayout(true);
			base.OnLoad(e);
		}

		internal void FitSizeToContent()
		{
			if (this.Control != null)
			{
				int num = base.ClientSize.Height - (this.GetPreferredHeightForControl() + base.ButtonsPanel.Height);
				base.Height -= num;
				this.Control.Dock = DockStyle.Fill;
			}
		}

		[DefaultValue(true)]
		public bool AutoSizeControl { get; set; }

		private int GetPreferredHeightForControl()
		{
			Size size = this.Control.Size;
			if (this.AutoSizeControl)
			{
				this.Control.Dock = DockStyle.Top;
				size = this.Control.GetPreferredSize(new Size(base.ClientSize.Width, 0));
			}
			return size.Height;
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			base.HelpVisible = (base.HelpVisible && (!string.IsNullOrEmpty(base.HelpTopic) || (this.Control != null && !string.IsNullOrEmpty(this.Control.HelpTopic))));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (base.DialogResult == DialogResult.OK && this.Control != null)
			{
				e.Cancel = !this.Control.OnKillActive();
				if (!e.Cancel && this.Control.IsDirty)
				{
					this.Control.Apply(e);
				}
			}
			base.OnClosing(e);
		}

		private void page_IsDirtyChanged(object sender, EventArgs e)
		{
			this.IsDirty |= ((ExchangePropertyPageControl)sender).IsDirty;
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
					if (this.LinkIsDirtyToOkEnabled)
					{
						base.OkEnabled = (this.IsValid && this.IsDirty);
					}
					this.OnIsValidChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIsValidChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[PropertyPageDialog.EventIsValidChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IsValidChanged
		{
			add
			{
				base.Events.AddHandler(PropertyPageDialog.EventIsValidChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(PropertyPageDialog.EventIsValidChanged, value);
			}
		}

		[DefaultValue(false)]
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
				if (this.IsDirty != value)
				{
					this.isDirty = value;
					if (this.LinkIsDirtyToOkEnabled)
					{
						base.OkEnabled = (this.IsValid && this.IsDirty);
					}
					this.OnIsDirtyChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIsDirtyChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[PropertyPageDialog.EventIsDirtyChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IsDirtyChanged
		{
			add
			{
				base.Events.AddHandler(PropertyPageDialog.EventIsDirtyChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(PropertyPageDialog.EventIsDirtyChanged, value);
			}
		}

		[DefaultValue(false)]
		public bool LinkIsDirtyToOkEnabled
		{
			get
			{
				return this.linkIsDirtyToOkEnabled;
			}
			set
			{
				if (value != this.linkIsDirtyToOkEnabled)
				{
					this.linkIsDirtyToOkEnabled = value;
					if (value)
					{
						base.OkEnabled = (this.IsValid && this.IsDirty);
					}
				}
			}
		}

		private ExchangePropertyPageControl control;

		private bool isValid = true;

		private static readonly object EventIsValidChanged = new object();

		private bool isDirty;

		private static readonly object EventIsDirtyChanged = new object();

		private bool linkIsDirtyToOkEnabled;
	}
}
