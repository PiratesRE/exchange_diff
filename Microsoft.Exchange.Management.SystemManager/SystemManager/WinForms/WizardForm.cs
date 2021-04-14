using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class WizardForm : ExchangeForm
	{
		public WizardForm()
		{
			this.InitializeComponent();
			this.title.Font = new Font(Control.DefaultFont.FontFamily, Control.DefaultFont.SizeInPoints + 2f, FontStyle.Bold);
			this.buttons.SuspendLayout();
			this.help.Command = this.wizard.Help;
			this.reset.Command = this.wizard.Reset;
			this.back.Command = this.wizard.Back;
			this.next.Command = this.wizard.Next;
			this.finish.Command = this.wizard.Finish;
			this.cancel.Command = this.wizard.Cancel;
			this.next.VisibleChanged += this.buttons_StateChanged;
			this.next.EnabledChanged += this.buttons_StateChanged;
			this.finish.VisibleChanged += this.buttons_StateChanged;
			this.finish.EnabledChanged += this.buttons_StateChanged;
			this.buttons.ResumeLayout();
			this.wizard.UpdatingButtons += delegate(object param0, EventArgs param1)
			{
				this.buttons.SuspendLayout();
			};
			this.wizard.ButtonsUpdated += delegate(object param0, EventArgs param1)
			{
				this.buttons.ResumeLayout();
			};
			this.wizard.TextChanged += delegate(object param0, EventArgs param1)
			{
				this.pageTitle.Text = this.wizard.Text;
			};
			Extensions.EnsureDoubleBuffer(this);
			Theme.UseVisualEffectsChanged += this.Theme_UseVisualEffectsChanged;
			this.Theme_UseVisualEffectsChanged(null, EventArgs.Empty);
			this.backgroundPanel.MouseDown += delegate(object sender, MouseEventArgs e)
			{
				if (e.Button == MouseButtons.Left)
				{
					UnsafeNativeMethods.ReleaseCapture();
					UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 161, 2, 0U);
				}
			};
		}

		public override bool RightToLeftLayout
		{
			get
			{
				return false;
			}
		}

		private void Theme_UseVisualEffectsChanged(object sender, EventArgs e)
		{
			if (Theme.UseVisualEffects)
			{
				this.BackgroundImage = (LayoutHelper.IsRightToLeft(this) ? Icons.SilverWizardRTL : Icons.SilverWizard);
				this.BackgroundImageLayout = ImageLayout.Stretch;
				base.FormBorderStyle = FormBorderStyle.None;
				this.transparencyMask.SetTransparencyImage(this, LayoutHelper.IsRightToLeft(this) ? Icons.SilverWizardRTL : Icons.SilverWizard);
				this.transparencyMask.SetTransparencyKey(this, Color.Fuchsia);
				if (LayoutHelper.IsRightToLeft(this))
				{
					base.Region.Translate(-7, 0);
					if (Application.RenderWithVisualStyles)
					{
						base.Region.Translate(-10, 0);
						return;
					}
				}
			}
			else
			{
				this.BackgroundImage = null;
				this.transparencyMask.SetTransparencyImage(this, null);
				base.FormBorderStyle = FormBorderStyle.FixedDialog;
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			this.title.Text = this.Text;
		}

		public Wizard Wizard
		{
			get
			{
				return this.wizard;
			}
		}

		[DefaultValue(null)]
		public new Icon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (value != this.Icon)
				{
					base.Icon = value;
					this.icon = value;
					if (this.Image != null)
					{
						this.Image.Dispose();
					}
					this.Image = IconLibrary.ToBitmap(value, new Size(64, 64));
				}
			}
		}

		private Image Image
		{
			get
			{
				return this.pictureBox.Image;
			}
			set
			{
				this.pictureBox.Image = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public WizardPage CurrentPage
		{
			get
			{
				return this.wizard.CurrentPage;
			}
			set
			{
				this.wizard.CurrentPage = value;
			}
		}

		[DefaultValue(null)]
		public DataContext Context
		{
			get
			{
				return this.wizard.Context;
			}
			set
			{
				this.wizard.Context = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TypedControlCollection<WizardPage> WizardPages
		{
			get
			{
				return this.wizard.WizardPages;
			}
		}

		[DefaultValue(null)]
		public IRefreshable RefreshOnFinish
		{
			get
			{
				return this.refreshOnFinish;
			}
			set
			{
				this.refreshOnFinish = value;
			}
		}

		[DefaultValue(null)]
		public Command OriginatingCommand
		{
			get
			{
				return this.originatingCommand;
			}
			set
			{
				this.originatingCommand = value;
			}
		}

		private void wizard_CurrentPageChanged(object sender, EventArgs e)
		{
			WizardPage currentPage = this.CurrentPage;
			if (currentPage == null || !currentPage.Enabled || currentPage.ActiveControl == null)
			{
				if (this.next.Enabled && this.next.Visible)
				{
					this.next.Select();
					return;
				}
				if (this.finish.Enabled && this.finish.Visible)
				{
					this.finish.Select();
					return;
				}
				if (this.cancel.Enabled && this.cancel.Visible)
				{
					this.cancel.Select();
				}
			}
		}

		private void buttons_StateChanged(object sender, EventArgs e)
		{
			if (this.next.Enabled && this.next.Visible)
			{
				base.AcceptButton = this.next;
				if (this.cancel.Focused)
				{
					this.next.Select();
					return;
				}
			}
			else
			{
				if (this.finish.Enabled && this.finish.Visible)
				{
					base.AcceptButton = this.finish;
					this.finish.Select();
					return;
				}
				base.AcceptButton = null;
				this.wizard.Select();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			int msg = m.Msg;
			if (msg != 274)
			{
				base.WndProc(ref m);
				return;
			}
			int num = NativeMethods.LOWORD(m.WParam) & 65520;
			if (num == 61488 || num == 61472)
			{
				m.Result = IntPtr.Zero;
				return;
			}
			base.WndProc(ref m);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if ((Keys)131139 == keyData)
			{
				TridentsWizardPage tridentsWizardPage = this.CurrentPage as TridentsWizardPage;
				if (tridentsWizardPage != null)
				{
					WinformsHelper.SetDataObjectToClipboard(tridentsWizardPage.GetSummaryText(), false);
					return true;
				}
			}
			if ((Keys)262259 == keyData)
			{
				this.Cancel();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled && this.CurrentPage != null)
			{
				ExchangeHelpService.ShowHelpFromPage(this.CurrentPage);
				hevent.Handled = true;
			}
			base.OnHelpRequested(hevent);
		}

		public void ShowHelp()
		{
			this.help.PerformClick();
		}

		public void GoBack()
		{
			this.back.PerformClick();
		}

		public void GoForward()
		{
			this.next.PerformClick();
		}

		public void Finish()
		{
			this.finish.PerformClick();
		}

		public void Cancel()
		{
			this.cancel.PerformClick();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public bool CanGoBack
		{
			get
			{
				return this.back.Enabled;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanGoForward
		{
			get
			{
				return this.next.Enabled;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public bool CanFinish
		{
			get
			{
				return this.finish.Enabled;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public bool CanCancel
		{
			get
			{
				return this.cancel.Enabled;
			}
		}

		private Icon icon;

		private IRefreshable refreshOnFinish;

		private Command originatingCommand;

		private class AntiAliasedLabel : Label
		{
			public AntiAliasedLabel()
			{
				base.Name = "AntiAliasedLabel";
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				base.OnPaint(e);
			}

			[EditorBrowsable(EditorBrowsableState.Advanced)]
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			protected override void WndProc(ref Message m)
			{
				int msg = m.Msg;
				if (msg == 132)
				{
					m.Result = (IntPtr)(-1);
					return;
				}
				base.WndProc(ref m);
			}
		}

		private class WizardIcon : ExchangePictureBox
		{
			public WizardIcon()
			{
				base.Name = "WizardIcon";
			}

			[EditorBrowsable(EditorBrowsableState.Advanced)]
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			protected override void WndProc(ref Message m)
			{
				int msg = m.Msg;
				if (msg == 132)
				{
					m.Result = (IntPtr)(-1);
					return;
				}
				base.WndProc(ref m);
			}
		}
	}
}
