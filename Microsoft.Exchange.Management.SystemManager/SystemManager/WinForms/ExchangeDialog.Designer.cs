namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ExchangeDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeForm
	{
		private void InitializeComponent()
		{
			this.helpButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.cancelButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.okButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.buttonsPanel = new global::System.Windows.Forms.TableLayoutPanel();
			this.lockButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.buttonsPanel.SuspendLayout();
			base.SuspendLayout();
			this.helpButton.AutoSize = true;
			this.helpButton.FlatStyle = global::System.Windows.Forms.FlatStyle.System;
			this.helpButton.FocusedAlwaysOnClick = false;
			this.helpButton.Location = new global::System.Drawing.Point(425, 8);
			this.helpButton.Margin = new global::System.Windows.Forms.Padding(0, 0, 8, 0);
			this.helpButton.Name = "helpButton";
			this.helpButton.Size = new global::System.Drawing.Size(75, 23);
			this.helpButton.TabIndex = 2;
			this.cancelButton.AutoSize = true;
			this.cancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = global::System.Windows.Forms.FlatStyle.System;
			this.cancelButton.FocusedAlwaysOnClick = false;
			this.cancelButton.Location = new global::System.Drawing.Point(342, 8);
			this.cancelButton.Margin = new global::System.Windows.Forms.Padding(0, 0, 8, 0);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new global::System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Click += new global::System.EventHandler(this.CloseOnClick);
			this.okButton.AutoSize = true;
			this.okButton.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.okButton.FlatStyle = global::System.Windows.Forms.FlatStyle.System;
			this.okButton.FocusedAlwaysOnClick = false;
			this.okButton.Location = new global::System.Drawing.Point(259, 8);
			this.okButton.Margin = new global::System.Windows.Forms.Padding(0, 0, 8, 0);
			this.okButton.Name = "okButton";
			this.okButton.Size = new global::System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Click += new global::System.EventHandler(this.CloseOnClick);
			this.buttonsPanel.AutoSize = true;
			this.buttonsPanel.ColumnCount = 5;
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.buttonsPanel.Controls.Add(this.helpButton, 4, 0);
			this.buttonsPanel.Controls.Add(this.lockButton, 0, 0);
			this.buttonsPanel.Controls.Add(this.cancelButton, 3, 0);
			this.buttonsPanel.Controls.Add(this.okButton, 2, 0);
			this.buttonsPanel.Dock = global::System.Windows.Forms.DockStyle.Bottom;
			this.buttonsPanel.Location = new global::System.Drawing.Point(0, 414);
			this.buttonsPanel.Name = "buttonsPanel";
			this.buttonsPanel.Padding = new global::System.Windows.Forms.Padding(12, 8, 8, 8);
			this.buttonsPanel.RowCount = 1;
			this.buttonsPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.buttonsPanel.Size = new global::System.Drawing.Size(516, 39);
			this.buttonsPanel.TabIndex = 1;
			this.lockButton.Anchor = global::System.Windows.Forms.AnchorStyles.Left;
			this.lockButton.AutoSize = true;
			this.lockButton.Location = new global::System.Drawing.Point(12, 8);
			this.lockButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.lockButton.Name = "lockButton";
			this.lockButton.Size = new global::System.Drawing.Size(23, 23);
			this.lockButton.TabIndex = 3;
			this.lockButton.TabStop = false;
			base.AcceptButton = this.okButton;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new global::System.Drawing.Size(516, 453);
			base.Controls.Add(this.buttonsPanel);
			base.Margin = new global::System.Windows.Forms.Padding(4, 3, 4, 3);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ExchangeDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.buttonsPanel.ResumeLayout(false);
			this.buttonsPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton cancelButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton okButton;

		private global::System.Windows.Forms.TableLayoutPanel buttonsPanel;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton lockButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton helpButton;
	}
}
