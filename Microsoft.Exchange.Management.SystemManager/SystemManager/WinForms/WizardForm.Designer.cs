namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class WizardForm : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeForm
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				global::Microsoft.Exchange.Management.SystemManager.WinForms.Theme.UseVisualEffectsChanged -= new global::System.EventHandler(this.Theme_UseVisualEffectsChanged);
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.backgroundPanel = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel();
			this.pictureBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.WizardForm.WizardIcon();
			this.title = new global::Microsoft.Exchange.Management.SystemManager.WinForms.WizardForm.AntiAliasedLabel();
			this.buttons = new global::System.Windows.Forms.FlowLayoutPanel();
			this.cancel = new global::Microsoft.ManagementGUI.WinForms.CommandButton();
			this.finish = new global::Microsoft.ManagementGUI.WinForms.CommandButton();
			this.next = new global::Microsoft.ManagementGUI.WinForms.CommandButton();
			this.back = new global::Microsoft.ManagementGUI.WinForms.CommandButton();
			this.reset = new global::Microsoft.ManagementGUI.WinForms.CommandButton();
			this.help = new global::Microsoft.ManagementGUI.WinForms.CommandButton();
			this.orientation = new global::Microsoft.Exchange.Management.SystemManager.WinForms.WizardOrientationPanel();
			this.wizard = new global::Microsoft.Exchange.Management.SystemManager.WinForms.Wizard();
			this.transparencyMask = new global::Microsoft.ManagementGUI.WinForms.TransparencyMask();
			this.pageTitle = new global::System.Windows.Forms.Label();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox).BeginInit();
			this.buttons.SuspendLayout();
			this.backgroundPanel.SuspendLayout();
			base.SuspendLayout();
			this.backgroundPanel.BackColor = global::System.Drawing.Color.Transparent;
			this.backgroundPanel.ColumnCount = 5;
			this.backgroundPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.backgroundPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.backgroundPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.backgroundPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 6f));
			this.backgroundPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.backgroundPanel.Controls.Add(this.pictureBox, 0, 0);
			this.backgroundPanel.Controls.Add(this.title, 1, 0);
			this.backgroundPanel.Controls.Add(this.orientation, 0, 2);
			this.backgroundPanel.Controls.Add(this.pageTitle, 4, 2);
			this.backgroundPanel.Controls.Add(this.wizard, 4, 3);
			this.backgroundPanel.Controls.Add(this.help, 0, 4);
			this.backgroundPanel.Controls.Add(this.buttons, 4, 4);
			this.backgroundPanel.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.backgroundPanel.Name = "backgroundPanel";
			this.backgroundPanel.RowCount = 5;
			this.backgroundPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.backgroundPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle(global::System.Windows.Forms.SizeType.Absolute, 6f));
			this.backgroundPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.backgroundPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.backgroundPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.backgroundPanel.Padding = new global::System.Windows.Forms.Padding(24, 15, 11, 0);
			this.pictureBox.BackColor = global::System.Drawing.Color.Transparent;
			this.pictureBox.Location = new global::System.Drawing.Point(24, 15);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new global::System.Drawing.Size(64, 64);
			this.pictureBox.SizeMode = global::System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			this.title.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.title.AutoEllipsis = true;
			this.title.BackColor = global::System.Drawing.Color.Transparent;
			this.backgroundPanel.SetColumnSpan(this.title, 4);
			this.title.Font = new global::System.Drawing.Font("Verdana", 18f, global::System.Drawing.FontStyle.Bold, global::System.Drawing.GraphicsUnit.Point, 0);
			this.title.Location = new global::System.Drawing.Point(95, 15);
			this.title.Name = "title";
			this.title.Size = new global::System.Drawing.Size(516, 64);
			this.title.TabIndex = 0;
			this.title.Text = "[title]";
			this.title.TextAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
			this.title.UseMnemonic = false;
			this.buttons.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right);
			this.buttons.AutoSize = true;
			this.buttons.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttons.BackColor = global::System.Drawing.Color.Transparent;
			this.buttons.Controls.Add(this.cancel);
			this.buttons.Controls.Add(this.finish);
			this.buttons.Controls.Add(this.next);
			this.buttons.Controls.Add(this.back);
			this.buttons.Controls.Add(this.reset);
			this.buttons.FlowDirection = global::System.Windows.Forms.FlowDirection.RightToLeft;
			this.buttons.Location = new global::System.Drawing.Point(203, 513);
			this.buttons.Name = "buttons";
			this.buttons.Size = new global::System.Drawing.Size(411, 29);
			this.buttons.TabIndex = 4;
			this.cancel.AutoSize = true;
			this.cancel.BackColor = global::System.Drawing.Color.Transparent;
			this.cancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.cancel.FocusedAlwaysOnClick = false;
			this.cancel.Location = new global::System.Drawing.Point(336, 3);
			this.cancel.Margin = new global::System.Windows.Forms.Padding(12, 3, 0, 3);
			this.cancel.Name = "cancel";
			this.cancel.Size = new global::System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 4;
			this.cancel.Text = "[cancel]";
			this.cancel.UseVisualStyleBackColor = false;
			this.finish.AutoSize = true;
			this.finish.BackColor = global::System.Drawing.Color.Transparent;
			this.finish.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.finish.FocusedAlwaysOnClick = false;
			this.finish.Location = new global::System.Drawing.Point(246, 3);
			this.finish.Name = "finish";
			this.finish.Size = new global::System.Drawing.Size(75, 23);
			this.finish.TabIndex = 3;
			this.finish.Text = "[finish]";
			this.finish.UseVisualStyleBackColor = false;
			this.finish.Visible = false;
			this.next.AutoSize = true;
			this.next.BackColor = global::System.Drawing.Color.Transparent;
			this.next.FocusedAlwaysOnClick = false;
			this.next.Location = new global::System.Drawing.Point(165, 3);
			this.next.Name = "next";
			this.next.Size = new global::System.Drawing.Size(75, 23);
			this.next.TabIndex = 2;
			this.next.Text = "[next]";
			this.next.UseVisualStyleBackColor = false;
			this.next.Visible = false;
			this.back.AutoSize = true;
			this.back.BackColor = global::System.Drawing.Color.Transparent;
			this.back.FocusedAlwaysOnClick = false;
			this.back.Location = new global::System.Drawing.Point(84, 3);
			this.back.Name = "back";
			this.back.Size = new global::System.Drawing.Size(75, 23);
			this.back.TabIndex = 1;
			this.back.Text = "[back]";
			this.back.UseVisualStyleBackColor = false;
			this.back.Visible = false;
			this.reset.AutoSize = true;
			this.reset.BackColor = global::System.Drawing.Color.Transparent;
			this.reset.FocusedAlwaysOnClick = false;
			this.reset.Location = new global::System.Drawing.Point(3, 3);
			this.reset.Name = "reset";
			this.reset.Size = new global::System.Drawing.Size(75, 23);
			this.reset.TabIndex = 0;
			this.reset.Text = "[reset]";
			this.reset.UseVisualStyleBackColor = false;
			this.reset.Visible = false;
			this.help.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left);
			this.help.AutoSize = true;
			this.help.BackColor = global::System.Drawing.Color.Transparent;
			this.help.Location = new global::System.Drawing.Point(24, 516);
			this.help.Margin = new global::System.Windows.Forms.Padding(0, 3, 3, 3);
			this.help.Name = "help";
			this.help.Size = new global::System.Drawing.Size(75, 23);
			this.help.TabIndex = 3;
			this.help.Text = "[help]";
			this.help.UseVisualStyleBackColor = false;
			this.orientation.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left);
			this.orientation.BackColor = global::System.Drawing.Color.Transparent;
			this.backgroundPanel.SetColumnSpan(this.orientation, 3);
			this.orientation.Location = new global::System.Drawing.Point(25, 85);
			this.orientation.Margin = new global::System.Windows.Forms.Padding(0);
			this.orientation.Name = "orientation";
			this.backgroundPanel.SetRowSpan(this.orientation, 2);
			this.orientation.Size = new global::System.Drawing.Size(141, 418);
			this.orientation.TabIndex = 1;
			this.orientation.Wizard = this.wizard;
			this.wizard.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.wizard.BackColor = global::System.Drawing.Color.Transparent;
			this.wizard.Location = new global::System.Drawing.Point(172, 105);
			this.wizard.Margin = new global::System.Windows.Forms.Padding(0);
			this.wizard.Name = "wizard";
			this.wizard.Size = new global::System.Drawing.Size(454, 398);
			this.wizard.TabIndex = 2;
			this.wizard.CurrentPageChanged += new global::System.EventHandler(this.wizard_CurrentPageChanged);
			this.pageTitle.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.pageTitle.AutoEllipsis = true;
			this.pageTitle.BackColor = global::System.Drawing.Color.Transparent;
			this.pageTitle.Font = new global::System.Drawing.Font("Microsoft Sans Serif", 8.25f, global::System.Drawing.FontStyle.Bold, global::System.Drawing.GraphicsUnit.Point, 0);
			this.pageTitle.Location = new global::System.Drawing.Point(172, 89);
			this.pageTitle.Name = "pageTitle";
			this.pageTitle.Size = new global::System.Drawing.Size(456, 16);
			this.pageTitle.TabIndex = 5;
			this.pageTitle.Text = "[pageTitle]";
			this.pageTitle.UseMnemonic = false;
			base.AcceptButton = this.finish;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.cancel;
			base.ClientSize = new global::System.Drawing.Size(637, 556);
			base.ControlBox = false;
			base.Controls.Add(this.backgroundPanel);
			this.DoubleBuffered = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "WizardForm";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox).EndInit();
			this.buttons.ResumeLayout(false);
			this.buttons.PerformLayout();
			this.backgroundPanel.ResumeLayout(false);
			this.backgroundPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.WizardForm.WizardIcon pictureBox;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.WizardForm.AntiAliasedLabel title;

		private global::System.Windows.Forms.FlowLayoutPanel buttons;

		private global::Microsoft.ManagementGUI.WinForms.CommandButton help;

		private global::Microsoft.ManagementGUI.WinForms.CommandButton reset;

		private global::Microsoft.ManagementGUI.WinForms.CommandButton back;

		private global::Microsoft.ManagementGUI.WinForms.CommandButton next;

		private global::Microsoft.ManagementGUI.WinForms.CommandButton finish;

		private global::Microsoft.ManagementGUI.WinForms.CommandButton cancel;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.WizardOrientationPanel orientation;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.Wizard wizard;

		private global::System.Windows.Forms.Label pageTitle;

		private global::Microsoft.ManagementGUI.WinForms.TransparencyMask transparencyMask;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel backgroundPanel;
	}
}
