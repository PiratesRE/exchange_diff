namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract partial class SearchDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeForm
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.components.Dispose();
				this.ResultListView = null;
				this.DataTableLoader = null;
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new global::System.ComponentModel.Container();
			this.dialogButtonsPanel = new global::System.Windows.Forms.FlowLayoutPanel();
			this.helpButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.cancelButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.okButton = new global::Microsoft.ManagementGUI.WinForms.ExchangeButton();
			this.statusStrip = new global::System.Windows.Forms.StatusStrip();
			this.selectedCountLabel = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.loadStatusLabel = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.loadProgressBar = new global::Microsoft.ManagementGUI.WinForms.ExchangeToolStripProgressBar();
			this.toolStripContainer = new global::System.Windows.Forms.ToolStripContainer();
			this.listControlPanel = new global::System.Windows.Forms.Panel();
			this.menuStrip = new global::System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.enableColumnFiltferingToolStripMenuItem = new global::Microsoft.Exchange.Management.SystemManager.WinForms.CommandToolStripMenuItem(this.components);
			this.addRemoveColumnsToolStripMenuItem = new global::Microsoft.Exchange.Management.SystemManager.WinForms.CommandToolStripMenuItem(this.components);
			this.scopeToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.modifyRecipientPickerScopeToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.modifyExpectedResultSizeMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip = new global::Microsoft.ManagementGUI.WinForms.TabbableToolStrip();
			this.toolStripLabelForName = new global::System.Windows.Forms.ToolStripLabel();
			this.toolStripTextBoxForName = new global::System.Windows.Forms.ToolStripTextBox();
			this.toolStripButtonFindNow = new global::Microsoft.Exchange.Management.SystemManager.WinForms.CommandToolStripButton();
			this.toolStripButtonClearOrStop = new global::Microsoft.Exchange.Management.SystemManager.WinForms.CommandToolStripButton();
			this.dialogButtonsPanel.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
			this.toolStripContainer.ContentPanel.SuspendLayout();
			this.toolStripContainer.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer.SuspendLayout();
			this.listControlPanel.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			base.SuspendLayout();
			this.dialogButtonsPanel.AutoSize = true;
			this.dialogButtonsPanel.Controls.Add(this.helpButton);
			this.dialogButtonsPanel.Controls.Add(this.cancelButton);
			this.dialogButtonsPanel.Controls.Add(this.okButton);
			this.dialogButtonsPanel.Dock = global::System.Windows.Forms.DockStyle.Bottom;
			this.dialogButtonsPanel.FlowDirection = global::System.Windows.Forms.FlowDirection.RightToLeft;
			this.dialogButtonsPanel.Location = new global::System.Drawing.Point(0, 333);
			this.dialogButtonsPanel.Name = "dialogButtonsPanel";
			this.dialogButtonsPanel.Size = new global::System.Drawing.Size(498, 29);
			this.dialogButtonsPanel.TabIndex = 4;
			this.dialogButtonsPanel.TabStop = true;
			this.helpButton.AutoSize = true;
			this.helpButton.FlatStyle = global::System.Windows.Forms.FlatStyle.System;
			this.helpButton.FocusedAlwaysOnClick = false;
			this.helpButton.Name = "helpButton";
			this.helpButton.Size = new global::System.Drawing.Size(75, 23);
			this.helpButton.TabIndex = 2;
			this.helpButton.Click += delegate(object param0, global::System.EventArgs param1)
			{
				this.OnHelpRequested(new global::System.Windows.Forms.HelpEventArgs(global::System.Drawing.Point.Empty));
			};
			this.cancelButton.AutoSize = true;
			this.cancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FocusedAlwaysOnClick = false;
			this.cancelButton.Location = new global::System.Drawing.Point(420, 3);
			this.cancelButton.MinimumSize = new global::System.Drawing.Size(75, 23);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new global::System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.okButton.AutoSize = true;
			this.okButton.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.FocusedAlwaysOnClick = false;
			this.okButton.Location = new global::System.Drawing.Point(339, 3);
			this.okButton.MinimumSize = new global::System.Drawing.Size(75, 23);
			this.okButton.Name = "okButton";
			this.okButton.Size = new global::System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.statusStrip.Dock = global::System.Windows.Forms.DockStyle.None;
			this.statusStrip.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.selectedCountLabel,
				this.loadStatusLabel,
				this.loadProgressBar
			});
			this.statusStrip.Location = new global::System.Drawing.Point(0, 0);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new global::System.Drawing.Size(498, 22);
			this.statusStrip.TabIndex = 4;
			this.selectedCountLabel.Name = "selectedCountLabel";
			this.loadStatusLabel.Name = "loadStatusLabel";
			this.loadStatusLabel.Spring = true;
			this.loadStatusLabel.TextAlign = global::System.Drawing.ContentAlignment.MiddleRight;
			this.loadProgressBar.Name = "loadProgressBar";
			this.loadProgressBar.Size = new global::System.Drawing.Size(100, 16);
			this.loadProgressBar.Style = global::System.Windows.Forms.ProgressBarStyle.Marquee;
			this.loadProgressBar.Visible = false;
			this.toolStripContainer.TopToolStripPanel.Name = "TopToolStripPanel";
			this.toolStripContainer.LeftToolStripPanel.Name = "LeftToolStripPanel";
			this.toolStripContainer.RightToolStripPanel.Name = "RightToolStripPanel";
			this.toolStripContainer.BottomToolStripPanel.Name = "BottomToolStripPanel";
			this.toolStripContainer.ContentPanel.Name = "ContentPanel";
			this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
			this.toolStripContainer.ContentPanel.AutoScroll = true;
			this.toolStripContainer.ContentPanel.Controls.Add(this.listControlPanel);
			this.toolStripContainer.ContentPanel.Controls.Add(this.dialogButtonsPanel);
			this.toolStripContainer.ContentPanel.RenderMode = global::System.Windows.Forms.ToolStripRenderMode.Professional;
			this.toolStripContainer.ContentPanel.Size = new global::System.Drawing.Size(498, 362);
			this.toolStripContainer.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer.Location = new global::System.Drawing.Point(0, 0);
			this.toolStripContainer.Name = "toolStripContainer";
			this.toolStripContainer.TabIndex = 0;
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip);
			this.toolStripContainer.TopToolStripPanel.RenderMode = global::System.Windows.Forms.ToolStripRenderMode.Professional;
			this.listControlPanel.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.listControlPanel.Location = new global::System.Drawing.Point(0, 0);
			this.listControlPanel.Name = "listControlPanel";
			this.listControlPanel.Size = new global::System.Drawing.Size(498, 362);
			this.listControlPanel.TabIndex = 3;
			this.menuStrip.Dock = global::System.Windows.Forms.DockStyle.None;
			this.menuStrip.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.fileToolStripMenuItem,
				this.viewToolStripMenuItem,
				this.scopeToolStripMenuItem
			});
			this.menuStrip.Location = new global::System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.RenderMode = global::System.Windows.Forms.ToolStripRenderMode.Professional;
			this.menuStrip.Size = new global::System.Drawing.Size(498, 24);
			this.menuStrip.TabIndex = 2;
			this.fileToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.closeToolStripMenuItem
			});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.ShortcutKeys = (global::System.Windows.Forms.Keys)262259;
			this.viewToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.enableColumnFiltferingToolStripMenuItem,
				this.addRemoveColumnsToolStripMenuItem,
				this.modifyExpectedResultSizeMenuItem
			});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.enableColumnFiltferingToolStripMenuItem.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.enableColumnFiltferingToolStripMenuItem.Name = "enableColumnFiltferingToolStripMenuItem";
			this.addRemoveColumnsToolStripMenuItem.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.addRemoveColumnsToolStripMenuItem.Name = "addRemoveColumnsToolStripMenuItem";
			this.scopeToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.modifyRecipientPickerScopeToolStripMenuItem
			});
			this.scopeToolStripMenuItem.Name = "scopeToolStripMenuItem";
			this.modifyRecipientPickerScopeToolStripMenuItem.Name = "modifyRecipientPickerScopeToolStripMenuItem";
			this.modifyExpectedResultSizeMenuItem.Name = "modifyExpectedResultSizeMenuItem";
			this.toolStrip.Dock = global::System.Windows.Forms.DockStyle.None;
			this.toolStrip.GripStyle = global::System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.toolStripLabelForName,
				this.toolStripTextBoxForName,
				this.toolStripButtonFindNow,
				this.toolStripButtonClearOrStop
			});
			this.toolStrip.Location = new global::System.Drawing.Point(0, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = global::System.Windows.Forms.ToolStripRenderMode.Professional;
			this.toolStrip.Size = new global::System.Drawing.Size(498, 25);
			this.toolStrip.Stretch = true;
			this.toolStrip.TabIndex = 1;
			this.toolStripLabelForName.Name = "toolStripLabelForName";
			this.toolStripTextBoxForName.Name = "toolStripTextBoxForName";
			this.toolStripTextBoxForName.Size = new global::System.Drawing.Size(150, 25);
			this.toolStripButtonFindNow.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonFindNow.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.toolStripButtonFindNow.Name = "toolStripButtonFindNow";
			this.toolStripButtonClearOrStop.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonClearOrStop.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.toolStripButtonClearOrStop.Name = "toolStripButtonClearOrStop";
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.cancelButton;
			base.ClientSize = new global::System.Drawing.Size(498, 433);
			base.Controls.Add(this.toolStripContainer);
			this.MinimumSize = new global::System.Drawing.Size(323, 284);
			base.Name = "SearchDialog";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.dialogButtonsPanel.ResumeLayout(false);
			this.dialogButtonsPanel.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.BottomToolStripPanel.PerformLayout();
			this.toolStripContainer.ContentPanel.ResumeLayout(false);
			this.toolStripContainer.ContentPanel.PerformLayout();
			this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.PerformLayout();
			this.toolStripContainer.ResumeLayout(false);
			this.toolStripContainer.PerformLayout();
			this.listControlPanel.ResumeLayout(false);
			this.listControlPanel.PerformLayout();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			base.ResumeLayout(false);
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.FlowLayoutPanel dialogButtonsPanel;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton helpButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton cancelButton;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeButton okButton;

		private global::System.Windows.Forms.ToolStripContainer toolStripContainer;

		private global::System.Windows.Forms.StatusStrip statusStrip;

		private global::System.Windows.Forms.ToolStripStatusLabel selectedCountLabel;

		private global::System.Windows.Forms.ToolStripStatusLabel loadStatusLabel;

		private global::Microsoft.ManagementGUI.WinForms.ExchangeToolStripProgressBar loadProgressBar;

		private global::Microsoft.ManagementGUI.WinForms.TabbableToolStrip toolStrip;

		private global::System.Windows.Forms.ToolStripLabel toolStripLabelForName;

		private global::System.Windows.Forms.ToolStripTextBox toolStripTextBoxForName;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.CommandToolStripButton toolStripButtonFindNow;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.CommandToolStripButton toolStripButtonClearOrStop;

		private global::System.Windows.Forms.MenuStrip menuStrip;

		private global::System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripMenuItem scopeToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripMenuItem modifyRecipientPickerScopeToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripMenuItem modifyExpectedResultSizeMenuItem;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.CommandToolStripMenuItem enableColumnFiltferingToolStripMenuItem;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.CommandToolStripMenuItem addRemoveColumnsToolStripMenuItem;

		private global::System.Windows.Forms.Panel listControlPanel;
	}
}
