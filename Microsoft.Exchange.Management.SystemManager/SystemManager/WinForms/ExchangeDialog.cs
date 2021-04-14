using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ExchangeDialog : ExchangeForm
	{
		public ExchangeDialog()
		{
			this.InitializeComponent();
			this.cancelButton.Text = Strings.Cancel;
			this.okButton.Text = Strings.Ok;
			this.helpButton.Text = Strings.Help;
			this.buttonsPanel.TabIndex = int.MaxValue;
			if (ExchangeDialog.lockImage == null)
			{
				Size empty = Size.Empty;
				empty.Width = Math.Min(this.lockButton.Width, this.lockButton.Height);
				empty.Height = empty.Width;
				ExchangeDialog.lockImage = IconLibrary.ToBitmap(Icons.LockIcon, empty);
			}
			this.lockButton.Image = ExchangeDialog.lockImage;
			this.lockButton.Visible = false;
			this.lockButton.FlatStyle = FlatStyle.Flat;
			this.lockButton.FlatAppearance.BorderSize = 0;
			this.lockButton.FlatAppearance.BorderColor = this.lockButton.BackColor;
			this.lockButton.FlatAppearance.MouseOverBackColor = this.lockButton.BackColor;
			this.lockButton.FlatAppearance.MouseDownBackColor = this.lockButton.BackColor;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(this.lockButton, Strings.ShowLockButtonToolTipText);
			this.helpButton.Click += delegate(object param0, EventArgs param1)
			{
				this.OnHelpRequested(new HelpEventArgs(Point.Empty));
			};
			this.helpButton.Visible = false;
		}

		[DefaultValue(true)]
		public bool OkEnabled
		{
			get
			{
				return this.okButton.Enabled;
			}
			set
			{
				this.okButton.Enabled = value;
			}
		}

		[DefaultValue(true)]
		public bool OkVisible
		{
			get
			{
				return this.okButton.Visible;
			}
			set
			{
				this.okButton.Visible = value;
			}
		}

		[DefaultValue(true)]
		public bool CancelEnabled
		{
			get
			{
				return this.cancelButton.Enabled;
			}
			set
			{
				this.cancelButton.Enabled = value;
			}
		}

		[DefaultValue(true)]
		public bool CancelVisible
		{
			get
			{
				return this.cancelButton.Visible;
			}
			set
			{
				this.cancelButton.Visible = value;
			}
		}

		[DefaultValue(false)]
		public bool LockVisible
		{
			get
			{
				return this.lockButton.Visible;
			}
			set
			{
				this.lockButton.Visible = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string OkButtonText
		{
			get
			{
				return this.okButton.Text;
			}
			set
			{
				this.okButton.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string CancelButtonText
		{
			get
			{
				return this.cancelButton.Text;
			}
			set
			{
				this.cancelButton.Text = value;
			}
		}

		[DefaultValue(false)]
		public bool HelpVisible
		{
			get
			{
				return this.helpButton.Visible;
			}
			set
			{
				this.helpButton.Visible = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public TableLayoutPanel ButtonsPanel
		{
			get
			{
				return this.buttonsPanel;
			}
		}

		private void CloseOnClick(object sender, EventArgs e)
		{
			base.Close();
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			this.buttonsPanel.SendToBack();
			base.OnLayout(levent);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			if (base.Owner == null)
			{
				this.HelpVisible = true;
			}
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!this.HelpVisible)
			{
				hevent.Handled = true;
			}
			base.OnHelpRequested(hevent);
		}

		private static Bitmap lockImage;
	}
}
