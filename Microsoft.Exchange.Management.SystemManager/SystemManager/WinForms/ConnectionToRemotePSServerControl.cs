using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ConnectionToRemotePSServerControl : GeneralPropertyPageControl
	{
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ChangeNotifyingCollection<GeneralPageSummaryInfo> GeneralPageSummaryInfoCollection
		{
			get
			{
				return base.GeneralPageSummaryInfoCollection;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Icon ObjectIcon
		{
			get
			{
				return base.ObjectIcon;
			}
			set
			{
				base.ObjectIcon = value;
			}
		}

		[DefaultValue(false)]
		public new bool CanChangeHeaderText
		{
			get
			{
				return base.CanChangeHeaderText;
			}
			set
			{
				base.CanChangeHeaderText = value;
			}
		}

		public ConnectionToRemotePSServerControl(Icon icon)
		{
			this.InitializeComponent();
			base.HelpTopic = HelpId.ConnectionToRemotePSServerControlProperty.ToString();
			this.titleLabel.Text = Strings.ConnectToPSServerDescription;
			this.automaticallyRadioButton.Text = Strings.AutomaticallySelectedText;
			this.manualRadioButton.Text = Strings.ManuallySelectedText;
			this.userInfo.Text = Strings.AccountNameText;
			this.ObjectIcon = icon;
			this.modifiedInfo = null;
			base.Header.DataBindings.Add("Text", base.BindingSource, "DisplayName");
			base.Header.CanChangeHeaderText = false;
			base.BindingSource.DataSource = typeof(PSRemoteServer);
			this.automaticallyRadioButton.DataBindings.Add("Checked", base.BindingSource, "AutomaticallySelect", true, DataSourceUpdateMode.OnPropertyChanged);
			this.manualRadioButton.DataBindings.Add("Checked", base.BindingSource, "AutomaticallySelect", true, DataSourceUpdateMode.Never).Format += delegate(object sender, ConvertEventArgs e)
			{
				e.Value = !(bool)e.Value;
			};
			AutomatedObjectPicker automatedObjectPicker = new AutomatedObjectPicker("ExchangeServerConfigurable");
			automatedObjectPicker.InputValue("MinVersion", 14);
			automatedObjectPicker.InputValue("DesiredServerRoleBitMask", ServerRole.Mailbox | ServerRole.ClientAccess | ServerRole.UnifiedMessaging | ServerRole.HubTransport);
			automatedObjectPicker.InputValue("ExactVersion", this.GetExchangeVersion());
			this.pickerLauncherTextBox.Picker = automatedObjectPicker;
			this.pickerLauncherTextBox.ValueMember = "Fqdn";
			this.pickerLauncherTextBox.DataBindings.Add("SelectedValue", base.BindingSource, "RemotePSServer", true, DataSourceUpdateMode.OnPropertyChanged);
			this.pickerLauncherTextBox.DataBindings.Add("Enabled", this.manualRadioButton, "Checked", true, DataSourceUpdateMode.Never);
		}

		public ConnectionToRemotePSServerControl() : this(Icons.Exchange)
		{
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.titleLabel = new Label();
			this.automaticallyRadioButton = new AutoHeightRadioButton();
			this.userInfo = new GeneralPageSummaryInfo();
			this.manualRadioButton = new AutoHeightRadioButton();
			this.pickerLauncherTextBox = new PickerLauncherTextBox();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.titleLabel, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.automaticallyRadioButton, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.manualRadioButton, 0, 2);
			this.tableLayoutPanel.Controls.Add(this.pickerLauncherTextBox, 1, 3);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 90);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(13, 0, 16, 12);
			this.tableLayoutPanel.RowCount = 4;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(557, 161);
			this.tableLayoutPanel.TabIndex = 6;
			this.titleLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.titleLabel.AutoSize = true;
			this.tableLayoutPanel.SetColumnSpan(this.titleLabel, 2);
			this.titleLabel.Location = new Point(13, 12);
			this.titleLabel.Margin = new Padding(0);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new Size(528, 17);
			this.titleLabel.TabIndex = 0;
			this.titleLabel.Text = "Select a server to connect to for remote PowerShell:";
			this.automaticallyRadioButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.automaticallyRadioButton, 2);
			this.automaticallyRadioButton.Checked = true;
			this.automaticallyRadioButton.Location = new Point(16, 32);
			this.automaticallyRadioButton.Margin = new Padding(3, 3, 0, 0);
			this.automaticallyRadioButton.Name = "automaticallyRadioButton";
			this.automaticallyRadioButton.Size = new Size(525, 21);
			this.automaticallyRadioButton.TabIndex = 1;
			this.automaticallyRadioButton.TabStop = true;
			this.automaticallyRadioButton.Text = "Connect to a server automatically selected";
			this.automaticallyRadioButton.UseVisualStyleBackColor = true;
			this.manualRadioButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.manualRadioButton, 2);
			this.manualRadioButton.Location = new Point(16, 95);
			this.manualRadioButton.Margin = new Padding(3, 12, 0, 0);
			this.manualRadioButton.Name = "manualRadioButton";
			this.manualRadioButton.Size = new Size(525, 21);
			this.manualRadioButton.TabIndex = 3;
			this.manualRadioButton.TabStop = true;
			this.manualRadioButton.Text = "Specify a server to connect to";
			this.manualRadioButton.UseVisualStyleBackColor = true;
			this.pickerLauncherTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.pickerLauncherTextBox.AutoSize = true;
			this.pickerLauncherTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pickerLauncherTextBox.Location = new Point(29, 122);
			this.pickerLauncherTextBox.Margin = new Padding(0, 6, 0, 0);
			this.pickerLauncherTextBox.Name = "pickerLauncherTextBox";
			this.pickerLauncherTextBox.Size = new Size(512, 27);
			this.pickerLauncherTextBox.TabIndex = 4;
			this.userInfo.BindingSource = base.BindingSource;
			this.userInfo.PropertyName = "UserAccount";
			this.userInfo.Text = "userInfo";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.tableLayoutPanel);
			this.GeneralPageSummaryInfoCollection.Add(this.userInfo);
			base.Name = "ConnectionToRemotePSServerControl";
			base.Size = new Size(418, 396);
			base.Controls.SetChildIndex(this.tableLayoutPanel, 0);
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private string GetExchangeVersion()
		{
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";
			int num = (int)(Registry.GetValue(keyName, "MsiProductMajor", null) ?? 0);
			int num2 = (int)(Registry.GetValue(keyName, "MsiProductMinor", null) ?? 0);
			int num3 = (int)(Registry.GetValue(keyName, "MsiBuildMajor", null) ?? 0);
			int num4 = (int)(Registry.GetValue(keyName, "MsiBuildMinor", null) ?? 0);
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				num,
				num2,
				num3,
				num4
			});
		}

		private AutoTableLayoutPanel tableLayoutPanel;

		private Label titleLabel;

		private AutoHeightRadioButton automaticallyRadioButton;

		private AutoHeightRadioButton manualRadioButton;

		private PickerLauncherTextBox pickerLauncherTextBox;

		private GeneralPageSummaryInfo userInfo;
	}
}
