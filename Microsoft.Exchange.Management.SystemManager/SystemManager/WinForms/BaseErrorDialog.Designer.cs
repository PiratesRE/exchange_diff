namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class BaseErrorDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeForm
	{
		private void InitializeComponent()
		{
			this.iconBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangePictureBox();
			this.buttonsPanel = new global::System.Windows.Forms.TableLayoutPanel();
			this.cancelButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.buttonsDivider = new global::Microsoft.ManagementGUI.WinForms.AutoHeightLabel();
			this.contentPanel = new global::System.Windows.Forms.Panel();
			this.dividerPanel = new global::System.Windows.Forms.Panel();
			this.messagePanel = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoSizePanel();
			this.messageLabel = new global::Microsoft.ManagementGUI.WinForms.AutoHeightLabel();
			((global::System.ComponentModel.ISupportInitialize)this.iconBox).BeginInit();
			this.buttonsPanel.SuspendLayout();
			this.contentPanel.SuspendLayout();
			this.messagePanel.SuspendLayout();
			base.SuspendLayout();
			this.iconBox.Location = new global::System.Drawing.Point(12, 12);
			this.iconBox.Margin = new global::System.Windows.Forms.Padding(0);
			this.iconBox.Name = "iconBox";
			this.iconBox.Size = new global::System.Drawing.Size(32, 32);
			this.iconBox.TabIndex = 0;
			this.iconBox.TabStop = false;
			this.buttonsPanel.AutoSize = true;
			this.buttonsPanel.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonsPanel.ColumnCount = 2;
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.buttonsPanel.Controls.Add(this.cancelButton, 1, 0);
			this.buttonsPanel.Dock = global::System.Windows.Forms.DockStyle.Bottom;
			this.buttonsPanel.Location = new global::System.Drawing.Point(12, 211);
			this.buttonsPanel.Name = "buttonsPanel";
			this.buttonsPanel.RowCount = 1;
			this.buttonsPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.buttonsPanel.Size = new global::System.Drawing.Size(448, 23);
			this.buttonsPanel.TabIndex = 2;
			this.cancelButton.AutoSize = true;
			this.cancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FocusedAlwaysOnClick = false;
			this.cancelButton.Location = new global::System.Drawing.Point(373, 0);
			this.cancelButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new global::System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 0;
			this.buttonsDivider.Dock = global::System.Windows.Forms.DockStyle.Bottom;
			this.buttonsDivider.Location = new global::System.Drawing.Point(12, 195);
			this.buttonsDivider.Name = "buttonsDivider";
			this.buttonsDivider.ShowDivider = true;
			this.buttonsDivider.Size = new global::System.Drawing.Size(448, 16);
			this.buttonsDivider.TabIndex = 1;
			this.contentPanel.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.contentPanel.BackColor = global::System.Drawing.SystemColors.Control;
			this.contentPanel.Controls.Add(this.dividerPanel);
			this.contentPanel.Controls.Add(this.messagePanel);
			this.contentPanel.Location = new global::System.Drawing.Point(56, 12);
			this.contentPanel.Name = "contentPanel";
			this.contentPanel.Size = new global::System.Drawing.Size(404, 180);
			this.contentPanel.TabIndex = 0;
			this.dividerPanel.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.dividerPanel.Location = new global::System.Drawing.Point(0, 0);
			this.dividerPanel.Name = "dividerPanel";
			this.dividerPanel.Size = new global::System.Drawing.Size(404, 12);
			this.dividerPanel.TabIndex = 1;
			this.messagePanel.AutoScroll = true;
			this.messagePanel.AutoSize = true;
			this.messagePanel.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.messagePanel.Controls.Add(this.messageLabel);
			this.messagePanel.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.messagePanel.Location = new global::System.Drawing.Point(0, 0);
			this.messagePanel.MaximumSize = new global::System.Drawing.Size(0, 84);
			this.messagePanel.Name = "messagePanel";
			this.messagePanel.Size = new global::System.Drawing.Size(404, 0);
			this.messagePanel.TabIndex = 0;
			this.messageLabel.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.messageLabel.LinkArea = new global::System.Windows.Forms.LinkArea(0, 0);
			this.messageLabel.Location = new global::System.Drawing.Point(0, 0);
			this.messageLabel.Margin = new global::System.Windows.Forms.Padding(0, 0, 0, 12);
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Size = new global::System.Drawing.Size(404, 0);
			this.messageLabel.TabIndex = 0;
			base.AcceptButton = this.cancelButton;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(472, 246);
			base.Controls.Add(this.contentPanel);
			base.Controls.Add(this.buttonsDivider);
			base.Controls.Add(this.buttonsPanel);
			base.Controls.Add(this.iconBox);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new global::System.Drawing.Size(480, 280);
			base.Name = "BaseErrorDialog";
			base.Padding = new global::System.Windows.Forms.Padding(12);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			((global::System.ComponentModel.ISupportInitialize)this.iconBox).EndInit();
			this.buttonsPanel.ResumeLayout(false);
			this.contentPanel.ResumeLayout(false);
			this.contentPanel.PerformLayout();
			this.messagePanel.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangePictureBox iconBox;

		private global::System.Windows.Forms.TableLayoutPanel buttonsPanel;

		private global::System.Windows.Forms.Panel dividerPanel;

		private global::Microsoft.ManagementGUI.WinForms.AutoHeightLabel messageLabel;

		private global::Microsoft.ManagementGUI.WinForms.AutoHeightLabel buttonsDivider;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton cancelButton;

		private global::System.Windows.Forms.Panel contentPanel;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoSizePanel messagePanel;
	}
}
