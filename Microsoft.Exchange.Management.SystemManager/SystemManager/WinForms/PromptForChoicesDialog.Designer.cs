namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed partial class PromptForChoicesDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeForm
	{
		private void InitializeComponent()
		{
			this.tableLayoutPanel = new global::System.Windows.Forms.TableLayoutPanel();
			this.warningIconPictureBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangePictureBox();
			this.buttonsPanel = new global::System.Windows.Forms.TableLayoutPanel();
			this.noButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.yesToAllButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.yesButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.cancelButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.warningMessageLabel = new global::System.Windows.Forms.Label();
			this.tableLayoutPanel.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.warningIconPictureBox).BeginInit();
			this.buttonsPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 32f));
			this.tableLayoutPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 16f));
			this.tableLayoutPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.warningIconPictureBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.buttonsPanel, 2, 2);
			this.tableLayoutPanel.Controls.Add(this.warningMessageLabel, 2, 0);
			this.tableLayoutPanel.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel.Location = new global::System.Drawing.Point(12, 12);
			this.tableLayoutPanel.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 3;
			this.tableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle(global::System.Windows.Forms.SizeType.Absolute, 8f));
			this.tableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new global::System.Drawing.Size(450, 63);
			this.tableLayoutPanel.TabIndex = 0;
			this.warningIconPictureBox.Location = new global::System.Drawing.Point(0, 0);
			this.warningIconPictureBox.Margin = new global::System.Windows.Forms.Padding(0);
			this.warningIconPictureBox.Name = "warningIconPictureBox";
			this.warningIconPictureBox.Size = new global::System.Drawing.Size(32, 32);
			this.warningIconPictureBox.TabIndex = 1;
			this.warningIconPictureBox.TabStop = false;
			this.buttonsPanel.Anchor = (global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right);
			this.buttonsPanel.AutoSize = true;
			this.buttonsPanel.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonsPanel.ColumnCount = 7;
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 25f));
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 8f));
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 25f));
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 8f));
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 25f));
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 8f));
			this.buttonsPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 25f));
			this.buttonsPanel.Controls.Add(this.noButton, 4, 0);
			this.buttonsPanel.Controls.Add(this.yesToAllButton, 2, 0);
			this.buttonsPanel.Controls.Add(this.yesButton, 0, 0);
			this.buttonsPanel.Controls.Add(this.cancelButton, 6, 0);
			this.buttonsPanel.Location = new global::System.Drawing.Point(74, 40);
			this.buttonsPanel.Margin = new global::System.Windows.Forms.Padding(0);
			this.buttonsPanel.Name = "buttonsPanel";
			this.buttonsPanel.RowCount = 1;
			this.buttonsPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.buttonsPanel.Size = new global::System.Drawing.Size(376, 23);
			this.buttonsPanel.TabIndex = 2;
			this.noButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.noButton.AutoSize = true;
			this.noButton.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.noButton.Location = new global::System.Drawing.Point(192, 0);
			this.noButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.noButton.MinimumSize = new global::System.Drawing.Size(75, 23);
			this.noButton.Name = "noButton";
			this.noButton.Size = new global::System.Drawing.Size(88, 23);
			this.noButton.TabIndex = 2;
			this.noButton.Text = "noButton";
			this.noButton.UseVisualStyleBackColor = true;
			this.yesToAllButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.yesToAllButton.AutoSize = true;
			this.yesToAllButton.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.yesToAllButton.Location = new global::System.Drawing.Point(96, 0);
			this.yesToAllButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.yesToAllButton.MinimumSize = new global::System.Drawing.Size(75, 23);
			this.yesToAllButton.Name = "yesToAllButton";
			this.yesToAllButton.Size = new global::System.Drawing.Size(88, 23);
			this.yesToAllButton.TabIndex = 1;
			this.yesToAllButton.Text = "yesToAllButton";
			this.yesToAllButton.UseVisualStyleBackColor = true;
			this.yesButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.yesButton.AutoSize = true;
			this.yesButton.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.yesButton.DialogResult = global::System.Windows.Forms.DialogResult.Yes;
			this.yesButton.FlatStyle = global::System.Windows.Forms.FlatStyle.System;
			this.yesButton.Location = new global::System.Drawing.Point(0, 0);
			this.yesButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.yesButton.MinimumSize = new global::System.Drawing.Size(75, 23);
			this.yesButton.Name = "yesButton";
			this.yesButton.Size = new global::System.Drawing.Size(88, 23);
			this.yesButton.TabIndex = 0;
			this.yesButton.Text = "yesButton";
			this.yesButton.UseVisualStyleBackColor = true;
			this.cancelButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.cancelButton.AutoSize = true;
			this.cancelButton.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.cancelButton.Location = new global::System.Drawing.Point(288, 0);
			this.cancelButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.cancelButton.MinimumSize = new global::System.Drawing.Size(75, 23);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new global::System.Drawing.Size(88, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.warningMessageLabel.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.warningMessageLabel.AutoSize = true;
			this.warningMessageLabel.Location = new global::System.Drawing.Point(48, 0);
			this.warningMessageLabel.Margin = new global::System.Windows.Forms.Padding(0);
			this.warningMessageLabel.Name = "warningMessageLabel";
			this.warningMessageLabel.Size = new global::System.Drawing.Size(402, 13);
			this.warningMessageLabel.TabIndex = 3;
			this.warningMessageLabel.Text = "warningMessageLabel";
			this.warningMessageLabel.UseMnemonic = false;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(474, 85);
			base.Controls.Add(this.tableLayoutPanel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new global::System.Drawing.Size(480, 32);
			base.Name = "PromptForChoicesDialog";
			base.Padding = new global::System.Windows.Forms.Padding(12);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = global::System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.warningIconPictureBox).EndInit();
			this.buttonsPanel.ResumeLayout(false);
			this.buttonsPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangePictureBox warningIconPictureBox;

		private global::System.Windows.Forms.TableLayoutPanel buttonsPanel;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton yesButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton cancelButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton noButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton yesToAllButton;

		private global::System.Windows.Forms.Label warningMessageLabel;
	}
}
