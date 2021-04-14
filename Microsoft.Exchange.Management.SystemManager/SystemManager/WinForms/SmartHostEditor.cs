using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SmartHostEditor : StrongTypeEditor<SmartHost>
	{
		public SmartHostEditor()
		{
			this.InitializeComponent();
			this.isIpAddressRadioButton.Text = Strings.IPAddressText;
			this.isHostNameRadioButton.Text = Strings.FullyQualifiedDomainNameText;
			this.ipAddressLabel.Text = Strings.ExampleIPAddressText;
			this.hostNameLabel.Text = Strings.ExampleDomainNameText;
			this.isIpAddressRadioButton.Checked = true;
			base.BindingSource.DataSource = typeof(SmartHost);
			this.isIpAddressRadioButton.DataBindings.Add("Checked", base.BindingSource, "IsIpAddress", true, DataSourceUpdateMode.OnPropertyChanged);
			this.isHostNameRadioButton.DataBindings.Add("Checked", base.BindingSource, "IsIpAddress", true, DataSourceUpdateMode.Never).Format += SmartHostEditor.ConvertBoolNot;
			this.addressTextBox.DataBindings.Add("Text", base.BindingSource, "Address", true, DataSourceUpdateMode.OnValidation);
			this.domain.DataBindings.Add("Text", base.BindingSource, "Domain", true, DataSourceUpdateMode.OnValidation);
			base.DataBindings.Add("IsCloudOrganizationMode", base.BindingSource, "IsCloudOrganizationMode", true, DataSourceUpdateMode.Never);
		}

		protected override void OnValidating(CancelEventArgs e)
		{
			if (this.isHostNameRadioButton.Checked)
			{
				this.domain.DataBindings[0].WriteValue();
			}
			else
			{
				this.addressTextBox.DataBindings[0].WriteValue();
			}
			base.OnValidating(e);
		}

		internal static void ConvertBoolNot(object sender, ConvertEventArgs e)
		{
			e.Value = !(bool)e.Value;
		}

		private void isIpAddress_CheckedChanged(object sender, EventArgs e)
		{
			this.addressTextBox.Enabled = this.isIpAddressRadioButton.Checked;
			this.domain.Enabled = !this.isIpAddressRadioButton.Checked;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override Size MinimumSize
		{
			get
			{
				return base.MinimumSize;
			}
			set
			{
				base.MinimumSize = value;
			}
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

		[DefaultValue(false)]
		public bool IsCloudOrganizationMode
		{
			get
			{
				return !this.isIpAddressRadioButton.Enabled;
			}
			set
			{
				if (this.IsCloudOrganizationMode != value)
				{
					this.isIpAddressRadioButton.Checked = (this.isIpAddressRadioButton.Enabled = (this.addressTextBox.Enabled = (this.ipAddressLabel.Enabled = !value)));
					this.isHostNameRadioButton.Checked = value;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.isIpAddressRadioButton = new AutoHeightRadioButton();
			this.addressTextBox = new ExchangeTextBox();
			this.isHostNameRadioButton = new AutoHeightRadioButton();
			this.domain = new ExchangeTextBox();
			this.tableLayoutPanel = new TableLayoutPanel();
			this.ipAddressLabel = new Label();
			this.hostNameLabel = new Label();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.isIpAddressRadioButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.isIpAddressRadioButton.Checked = true;
			this.tableLayoutPanel.SetColumnSpan(this.isIpAddressRadioButton, 2);
			this.isIpAddressRadioButton.Location = new Point(3, 0);
			this.isIpAddressRadioButton.Margin = new Padding(3, 0, 0, 0);
			this.isIpAddressRadioButton.Name = "isIpAddressRadioButton";
			this.isIpAddressRadioButton.Size = new Size(399, 17);
			this.isIpAddressRadioButton.TabIndex = 0;
			this.isIpAddressRadioButton.TabStop = true;
			this.isIpAddressRadioButton.Text = "IP Address";
			this.isIpAddressRadioButton.UseVisualStyleBackColor = true;
			this.isIpAddressRadioButton.CheckedChanged += this.isIpAddress_CheckedChanged;
			this.addressTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.addressTextBox.Location = new Point(19, 25);
			this.addressTextBox.Margin = new Padding(3, 8, 0, 0);
			this.addressTextBox.Name = "address";
			this.addressTextBox.Size = new Size(383, 20);
			this.addressTextBox.TabIndex = 1;
			this.isHostNameRadioButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.isHostNameRadioButton, 2);
			this.isHostNameRadioButton.Location = new Point(3, 78);
			this.isHostNameRadioButton.Margin = new Padding(3, 12, 0, 0);
			this.isHostNameRadioButton.Name = "isHostNameRadioButton";
			this.isHostNameRadioButton.Size = new Size(399, 17);
			this.isHostNameRadioButton.TabIndex = 3;
			this.isHostNameRadioButton.Text = "Host name";
			this.isHostNameRadioButton.UseVisualStyleBackColor = true;
			this.domain.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.domain.Enabled = false;
			this.domain.Location = new Point(19, 103);
			this.domain.Margin = new Padding(3, 8, 0, 0);
			this.domain.Name = "domain";
			this.domain.Size = new Size(383, 20);
			this.domain.TabIndex = 4;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.tableLayoutPanel.Controls.Add(this.domain, 1, 4);
			this.tableLayoutPanel.Controls.Add(this.isIpAddressRadioButton, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.isHostNameRadioButton, 0, 3);
			this.tableLayoutPanel.Controls.Add(this.addressTextBox, 1, 1);
			this.tableLayoutPanel.Controls.Add(this.ipAddressLabel, 1, 2);
			this.tableLayoutPanel.Controls.Add(this.hostNameLabel, 1, 5);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 6;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(418, 144);
			this.tableLayoutPanel.TabIndex = 5;
			this.ipAddressLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.ipAddressLabel.AutoSize = true;
			this.ipAddressLabel.Location = new Point(16, 53);
			this.ipAddressLabel.Margin = new Padding(0, 8, 0, 0);
			this.ipAddressLabel.Name = "ipAddressLabel";
			this.ipAddressLabel.Size = new Size(386, 13);
			this.ipAddressLabel.TabIndex = 2;
			this.ipAddressLabel.Text = "label1";
			this.hostNameLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.hostNameLabel.AutoSize = true;
			this.hostNameLabel.Location = new Point(16, 131);
			this.hostNameLabel.Margin = new Padding(0, 8, 0, 0);
			this.hostNameLabel.Name = "hostNameLabel";
			this.hostNameLabel.Size = new Size(386, 13);
			this.hostNameLabel.TabIndex = 5;
			this.hostNameLabel.Text = "label1";
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.tableLayoutPanel);
			this.MinimumSize = new Size(418, 0);
			base.Name = "SmartHostEditor";
			base.Size = new Size(418, 144);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private IContainer components;

		private AutoHeightRadioButton isIpAddressRadioButton;

		private ExchangeTextBox addressTextBox;

		private AutoHeightRadioButton isHostNameRadioButton;

		private ExchangeTextBox domain;

		private TableLayoutPanel tableLayoutPanel;

		private Label ipAddressLabel;

		private Label hostNameLabel;
	}
}
