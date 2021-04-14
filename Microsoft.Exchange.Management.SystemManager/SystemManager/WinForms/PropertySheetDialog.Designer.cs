namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class PropertySheetDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeForm
	{
		private void InitializeComponent()
		{
			this.tabControl = new global::Microsoft.ManagementGUI.WinForms.ExchangeTabControl();
			this.actionsPanel = new global::System.Windows.Forms.TableLayoutPanel();
			this.buttonsPanel = new global::System.Windows.Forms.FlowLayoutPanel();
			this.helpButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.applyButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.cancelButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.okButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.commandExposureButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.lockButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.actionsPanel.SuspendLayout();
			this.buttonsPanel.SuspendLayout();
			base.SuspendLayout();
			this.tabControl.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new global::System.Drawing.Point(6, 6);
			this.tabControl.Margin = new global::System.Windows.Forms.Padding(4, 3, 4, 3);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new global::System.Drawing.Size(514, 413);
			this.tabControl.TabIndex = 0;
			this.tabControl.ControlAdded += new global::System.Windows.Forms.ControlEventHandler(this.tabControl_ControlAdded);
			this.actionsPanel.AutoSize = true;
			this.actionsPanel.ColumnCount = 3;
			this.actionsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.actionsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.actionsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.actionsPanel.Controls.Add(this.buttonsPanel, 2, 0);
			this.actionsPanel.Controls.Add(this.commandExposureButton, 0, 0);
			this.actionsPanel.Controls.Add(this.lockButton, 1, 0);
			this.actionsPanel.Dock = global::System.Windows.Forms.DockStyle.Bottom;
			this.actionsPanel.Location = new global::System.Drawing.Point(6, 419);
			this.actionsPanel.Name = "actionsPanel";
			this.actionsPanel.Padding = new global::System.Windows.Forms.Padding(0, 3, 0, 0);
			this.actionsPanel.RowCount = 1;
			this.actionsPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.actionsPanel.Size = new global::System.Drawing.Size(514, 38);
			this.actionsPanel.TabIndex = 1;
			this.buttonsPanel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.buttonsPanel.AutoSize = true;
			this.buttonsPanel.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonsPanel.Controls.Add(this.helpButton);
			this.buttonsPanel.Controls.Add(this.applyButton);
			this.buttonsPanel.Controls.Add(this.cancelButton);
			this.buttonsPanel.Controls.Add(this.okButton);
			this.buttonsPanel.FlowDirection = global::System.Windows.Forms.FlowDirection.RightToLeft;
			this.buttonsPanel.Location = new global::System.Drawing.Point(190, 6);
			this.buttonsPanel.Name = "buttonsPanel";
			this.buttonsPanel.Size = new global::System.Drawing.Size(321, 29);
			this.buttonsPanel.TabIndex = 1;
			this.helpButton.AutoSize = true;
			this.helpButton.FocusedAlwaysOnClick = false;
			this.helpButton.Location = new global::System.Drawing.Point(246, 3);
			this.helpButton.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 3);
			this.helpButton.Name = "helpButton";
			this.helpButton.Size = new global::System.Drawing.Size(75, 23);
			this.helpButton.TabIndex = 5;
			this.helpButton.Text = "[help]";
			this.applyButton.AutoSize = true;
			this.applyButton.FocusedAlwaysOnClick = false;
			this.applyButton.Location = new global::System.Drawing.Point(165, 3);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new global::System.Drawing.Size(75, 23);
			this.applyButton.TabIndex = 4;
			this.applyButton.Text = "[apply]";
			this.cancelButton.AutoSize = true;
			this.cancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FocusedAlwaysOnClick = false;
			this.cancelButton.Location = new global::System.Drawing.Point(84, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new global::System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "[cancel]";
			this.cancelButton.Click += new global::System.EventHandler(this.CloseOnClick);
			this.okButton.AutoSize = true;
			this.okButton.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.okButton.FocusedAlwaysOnClick = false;
			this.okButton.Location = new global::System.Drawing.Point(3, 3);
			this.okButton.Name = "okButton";
			this.okButton.Size = new global::System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "[ok]";
			this.okButton.Click += new global::System.EventHandler(this.CloseOnClick);
			this.commandExposureButton.Anchor = global::System.Windows.Forms.AnchorStyles.Left;
			this.commandExposureButton.AutoSize = true;
			this.commandExposureButton.FocusedAlwaysOnClick = false;
			this.commandExposureButton.Location = new global::System.Drawing.Point(3, 9);
			this.commandExposureButton.Name = "commandExposureButton";
			this.commandExposureButton.Size = new global::System.Drawing.Size(23, 23);
			this.commandExposureButton.TabIndex = 0;
			this.lockButton.AutoSize = true;
			this.lockButton.Anchor = global::System.Windows.Forms.AnchorStyles.Left;
			this.lockButton.Location = new global::System.Drawing.Point(3, 9);
			this.lockButton.Name = "lockButton";
			this.lockButton.Size = new global::System.Drawing.Size(23, 23);
			this.lockButton.TabStop = false;
			base.AcceptButton = this.okButton;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new global::System.Drawing.Size(526, 463);
			base.Controls.Add(this.tabControl);
			base.Controls.Add(this.actionsPanel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Margin = new global::System.Windows.Forms.Padding(4, 3, 4, 3);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PropertySheetDialog";
			base.Padding = new global::System.Windows.Forms.Padding(6);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.actionsPanel.ResumeLayout(false);
			this.actionsPanel.PerformLayout();
			this.buttonsPanel.ResumeLayout(false);
			this.buttonsPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::Microsoft.ManagementGUI.WinForms.ExchangeTabControl tabControl;

		private global::System.Windows.Forms.TableLayoutPanel actionsPanel;

		private global::System.Windows.Forms.FlowLayoutPanel buttonsPanel;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton cancelButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton applyButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton helpButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton commandExposureButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton okButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton lockButton;
	}
}
