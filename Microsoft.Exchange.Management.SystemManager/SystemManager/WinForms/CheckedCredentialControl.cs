using System;
using System.ComponentModel;
using System.Drawing;
using System.Management.Automation;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CheckedCredentialControl : ExchangeUserControl
	{
		public CheckedCredentialControl()
		{
			this.InitializeComponent();
			this.Caption = Strings.MasterAccountPageUseFollowingAccount;
			this.credentialControl.AllowPasswordConfirmation = false;
			this.checkBox.CheckedChanged += this.checkBox_CheckedChanged;
			this.credentialControl.StrongTypeChanged += this.credentialControl_StrongTypeChanged;
		}

		[DefaultValue(true)]
		public new bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
		}

		[DefaultValue(AutoSizeMode.GrowAndShrink)]
		public new AutoSizeMode AutoSizeMode
		{
			get
			{
				return base.AutoSizeMode;
			}
			set
			{
				base.AutoSizeMode = value;
			}
		}

		[DefaultValue(true)]
		public bool CheckBoxEnabled
		{
			get
			{
				return this.checkBox.Enabled;
			}
			set
			{
				if (value != this.CheckBoxEnabled)
				{
					if (!value)
					{
						this.checkBox.Checked = true;
					}
					this.checkBox.Enabled = value;
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Caption
		{
			get
			{
				return this.checkBox.Text;
			}
			set
			{
				this.checkBox.Text = value;
			}
		}

		private void credentialControl_StrongTypeChanged(object sender, EventArgs e)
		{
			if (this.checkBox.Checked)
			{
				this.OnSelectedValueChanged(EventArgs.Empty);
			}
		}

		private void checkBox_CheckedChanged(object sender, EventArgs e)
		{
			bool @checked = this.checkBox.Checked;
			this.credentialControl.Enabled = @checked;
			if (!@checked || this.credentialControl.StrongType != null)
			{
				this.OnSelectedValueChanged(EventArgs.Empty);
			}
		}

		[DefaultValue(null)]
		public PSCredential SelectedValue
		{
			get
			{
				if (!this.checkBox.Checked)
				{
					return null;
				}
				return this.credentialControl.StrongType;
			}
			set
			{
				if (value != this.SelectedValue)
				{
					this.suspendChangeNotification = true;
					this.checkBox.Checked = (null != value);
					if (this.checkBox.Checked)
					{
						this.credentialControl.StrongType = value;
					}
					this.suspendChangeNotification = false;
					this.OnSelectedValueChanged(EventArgs.Empty);
				}
			}
		}

		private void OnSelectedValueChanged(EventArgs e)
		{
			if (!this.suspendChangeNotification && this.SelectedValueChanged != null)
			{
				this.SelectedValueChanged(this, e);
			}
		}

		public event EventHandler SelectedValueChanged;

		public override Size GetPreferredSize(Size proposedSize)
		{
			proposedSize.Width = base.Width;
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.checkBox = new AutoHeightCheckBox();
			this.credentialControl = new CredentialControl();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.tableLayoutPanel.Controls.Add(this.checkBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.credentialControl, 1, 1);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(100, 170);
			this.tableLayoutPanel.TabIndex = 0;
			this.checkBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.checkBox, 3);
			this.checkBox.Location = new Point(3, 0);
			this.checkBox.Margin = new Padding(3, 0, 0, 0);
			this.checkBox.Name = "checkBox";
			this.checkBox.Size = new Size(97, 17);
			this.checkBox.TabIndex = 0;
			this.checkBox.Text = "checkBox";
			this.credentialControl.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.credentialControl, 2);
			this.credentialControl.Enabled = false;
			this.credentialControl.Location = new Point(16, 25);
			this.credentialControl.Margin = new Padding(0, 8, 0, 0);
			this.credentialControl.Name = "credentialControl";
			this.credentialControl.Size = new Size(84, 145);
			this.credentialControl.TabIndex = 1;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "CheckedCredentialControl";
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Size = new Size(100, 170);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private AutoHeightCheckBox checkBox;

		private CredentialControl credentialControl;

		private AutoTableLayoutPanel tableLayoutPanel;

		private bool suspendChangeNotification;
	}
}
