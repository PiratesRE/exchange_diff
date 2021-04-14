using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ConnectToServerControl : ExchangePropertyPageControl
	{
		public ConnectToServerControl()
		{
			this.InitializeComponent();
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.serverPicker = this.CreateServerPicker();
			this.serverPickerLauncherTextBox.Picker = this.serverPicker;
			this.serverPickerLauncherTextBox.ValueMember = "Fqdn";
			base.BindingSource.DataSource = typeof(ConnectToServerParams);
			this.setDefaultServerCheckBox.DataBindings.Add("Checked", base.BindingSource, "SetAsDefaultServer");
			this.serverPickerLauncherTextBox.DataBindings.Add("SelectedValue", base.BindingSource, "ServerName");
			this.serverPickerLauncherTextBox.SelectedValueChanged += delegate(object param0, EventArgs param1)
			{
				this.setDefaultServerCheckBox.Enabled = !string.IsNullOrEmpty(this.serverPickerLauncherTextBox.SelectedValue.ToString());
			};
		}

		protected virtual AutomatedObjectPicker CreateServerPicker()
		{
			return new AutomatedObjectPicker("ExchangeServerConfigurable");
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return Size.Add(this.tableLayoutPanel.Size, base.Padding.Size);
		}

		[DefaultValue(true)]
		public override bool AutoSize
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

		[DefaultValue("connectServerLabel")]
		public string ConnectServerLabelText
		{
			get
			{
				return this.connectServerLabel.Text;
			}
			set
			{
				this.connectServerLabel.Text = value;
			}
		}

		[DefaultValue("setDefaultServerCheckBox")]
		public string SetDefaultServerCheckBoxText
		{
			get
			{
				return this.setDefaultServerCheckBox.Text;
			}
			set
			{
				this.setDefaultServerCheckBox.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ServerRole ServerRoleToPicker
		{
			get
			{
				return (ServerRole)this.serverPicker.GetValue("DesiredServerRoleBitMask");
			}
			set
			{
				this.serverPicker.InputValue("DesiredServerRoleBitMask", value);
			}
		}

		protected override Size DefaultMinimumSize
		{
			get
			{
				return new Size(328, 79);
			}
		}

		private void InitializeComponent()
		{
			this.connectServerLabel = new Label();
			this.serverPickerLauncherTextBox = new PickerLauncherTextBox();
			this.setDefaultServerCheckBox = new AutoHeightCheckBox();
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			this.connectServerLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.connectServerLabel.AutoSize = true;
			this.connectServerLabel.Location = new Point(13, 12);
			this.connectServerLabel.Margin = new Padding(0);
			this.connectServerLabel.Name = "connectServerLabel";
			this.connectServerLabel.Size = new Size(289, 13);
			this.connectServerLabel.TabIndex = 0;
			this.connectServerLabel.Text = "connectServerLabel";
			this.serverPickerLauncherTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.serverPickerLauncherTextBox.AutoSize = true;
			this.serverPickerLauncherTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.serverPickerLauncherTextBox.Location = new Point(13, 28);
			this.serverPickerLauncherTextBox.Margin = new Padding(0, 3, 0, 0);
			this.serverPickerLauncherTextBox.Name = "serverPickerLauncherTextBox";
			this.serverPickerLauncherTextBox.Size = new Size(289, 23);
			this.serverPickerLauncherTextBox.TabIndex = 1;
			this.setDefaultServerCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.setDefaultServerCheckBox.Enabled = false;
			this.setDefaultServerCheckBox.Location = new Point(16, 62);
			this.setDefaultServerCheckBox.Margin = new Padding(3, 11, 0, 0);
			this.setDefaultServerCheckBox.Name = "setDefaultServerCheckBox";
			this.setDefaultServerCheckBox.Size = new Size(286, 17);
			this.setDefaultServerCheckBox.TabIndex = 2;
			this.setDefaultServerCheckBox.Text = "setDefaultServerCheckBox";
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ContainerType = ContainerType.Control;
			this.tableLayoutPanel.Controls.Add(this.setDefaultServerCheckBox, 0, 2);
			this.tableLayoutPanel.Controls.Add(this.serverPickerLauncherTextBox, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.connectServerLabel, 0, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(13, 12, 16, 0);
			this.tableLayoutPanel.RowCount = 3;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.MinimumSize = new Size(318, 79);
			this.tableLayoutPanel.Size = new Size(318, 79);
			this.tableLayoutPanel.TabIndex = 0;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "ConnectToServerControl";
			base.Size = new Size(318, 79);
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private AutomatedObjectPicker serverPicker;

		private Label connectServerLabel;

		private PickerLauncherTextBox serverPickerLauncherTextBox;

		private AutoTableLayoutPanel tableLayoutPanel;

		private AutoHeightCheckBox setDefaultServerCheckBox;
	}
}
