namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class SmtpProxyAddressTemplateDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ProxyAddressDialog
	{
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel();
			this.aliasRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.firstNameLastNameRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.firstNameInitialLastNameRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.firstNameLastNameInitialRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.lastNameFirstNameRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.lastNameInitialFirstNameRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.lastNameFirstNameInitialRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.localPartCheckBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightCheckBox();
			this.domainTableLayoutPanel = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel();
			this.acceptedDomainRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.customDomainRadioButton = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton();
			this.acceptedDomainPickerLauncherTextBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.PickerLauncherTextBox();
			this.customDomainTextBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeTextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.domainTableLayoutPanel.SuspendLayout();
			base.ContentPage.SuspendLayout();
			base.SuspendLayout();
			this.domainTableLayoutPanel.AutoSize = true;
			this.domainTableLayoutPanel.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.domainTableLayoutPanel.ColumnCount = 2;
			this.domainTableLayoutPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 16f));
			this.domainTableLayoutPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.domainTableLayoutPanel.Controls.Add(this.acceptedDomainPickerLauncherTextBox, 1, 1);
			this.domainTableLayoutPanel.Controls.Add(this.acceptedDomainRadioButton, 0, 0);
			this.domainTableLayoutPanel.Controls.Add(this.customDomainRadioButton, 0, 2);
			this.domainTableLayoutPanel.Controls.Add(this.customDomainTextBox, 1, 3);
			this.domainTableLayoutPanel.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.domainTableLayoutPanel.AutoLayout = true;
			this.domainTableLayoutPanel.Margin = new global::System.Windows.Forms.Padding(0);
			this.domainTableLayoutPanel.Name = "domainTableLayoutPanel";
			this.domainTableLayoutPanel.Padding = new global::System.Windows.Forms.Padding(13, 0, 16, 12);
			this.domainTableLayoutPanel.RowCount = 4;
			this.domainTableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.domainTableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.domainTableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.domainTableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.domainTableLayoutPanel.Size = new global::System.Drawing.Size(429, 100);
			this.domainTableLayoutPanel.TabIndex = 9;
			this.acceptedDomainRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.acceptedDomainRadioButton.AutoSize = true;
			this.domainTableLayoutPanel.SetColumnSpan(this.acceptedDomainRadioButton, 2);
			this.acceptedDomainRadioButton.Location = new global::System.Drawing.Point(3, 3);
			this.acceptedDomainRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 0);
			this.acceptedDomainRadioButton.Name = "acceptedDomainRadioButton";
			this.acceptedDomainRadioButton.Size = new global::System.Drawing.Size(352, 17);
			this.acceptedDomainRadioButton.TabIndex = 10;
			this.acceptedDomainRadioButton.Text = "acceptedDomainRadioButton";
			this.acceptedDomainRadioButton.UseVisualStyleBackColor = true;
			this.customDomainRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.customDomainRadioButton.AutoSize = true;
			this.domainTableLayoutPanel.SetColumnSpan(this.customDomainRadioButton, 2);
			this.customDomainRadioButton.Location = new global::System.Drawing.Point(3, 52);
			this.customDomainRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 0);
			this.customDomainRadioButton.Name = "customDomainRadioButton";
			this.customDomainRadioButton.Size = new global::System.Drawing.Size(352, 17);
			this.customDomainRadioButton.TabIndex = 12;
			this.customDomainRadioButton.Text = "customDomainRadioButton";
			this.customDomainRadioButton.UseVisualStyleBackColor = true;
			this.customDomainRadioButton.Checked = true;
			this.acceptedDomainPickerLauncherTextBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.acceptedDomainPickerLauncherTextBox.AutoSize = true;
			this.acceptedDomainPickerLauncherTextBox.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.acceptedDomainPickerLauncherTextBox.CanBrowse = true;
			this.acceptedDomainPickerLauncherTextBox.Location = new global::System.Drawing.Point(19, 26);
			this.acceptedDomainPickerLauncherTextBox.Margin = new global::System.Windows.Forms.Padding(3, 6, 0, 0);
			this.acceptedDomainPickerLauncherTextBox.Name = "acceptedDomainPickerLauncherTextBox";
			this.acceptedDomainPickerLauncherTextBox.Size = new global::System.Drawing.Size(336, 23);
			this.acceptedDomainPickerLauncherTextBox.TabIndex = 11;
			this.customDomainTextBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.customDomainTextBox.Location = new global::System.Drawing.Point(19, 72);
			this.customDomainTextBox.Name = "customDomainTextBox";
			this.customDomainTextBox.Size = new global::System.Drawing.Size(333, 20);
			this.customDomainTextBox.TabIndex = 13;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 16f));
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel1.Controls.Add(this.aliasRadioButton, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.firstNameLastNameRadioButton, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.firstNameInitialLastNameRadioButton, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.firstNameLastNameInitialRadioButton, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.lastNameFirstNameRadioButton, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.lastNameInitialFirstNameRadioButton, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this.lastNameFirstNameInitialRadioButton, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this.localPartCheckBox, 0, 0);
			this.tableLayoutPanel1.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.AutoLayout = true;
			this.tableLayoutPanel1.Location = new global::System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new global::System.Windows.Forms.Padding(13, 12, 16, 0);
			this.tableLayoutPanel1.RowCount = 8;
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new global::System.Drawing.Size(429, 237);
			this.tableLayoutPanel1.TabIndex = 0;
			this.aliasRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.aliasRadioButton.AutoSize = true;
			this.aliasRadioButton.Location = new global::System.Drawing.Point(32, 33);
			this.aliasRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 4, 0, 0);
			this.aliasRadioButton.Name = "aliasRadioButton";
			this.aliasRadioButton.Size = new global::System.Drawing.Size(381, 17);
			this.aliasRadioButton.TabIndex = 2;
			this.aliasRadioButton.TabStop = true;
			this.aliasRadioButton.Text = "aliasRadioButton";
			this.aliasRadioButton.UseVisualStyleBackColor = true;
			this.firstNameLastNameRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.firstNameLastNameRadioButton.AutoSize = true;
			this.firstNameLastNameRadioButton.Location = new global::System.Drawing.Point(32, 54);
			this.firstNameLastNameRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 4, 0, 0);
			this.firstNameLastNameRadioButton.Name = "firstNameLastNameRadioButton";
			this.firstNameLastNameRadioButton.Size = new global::System.Drawing.Size(381, 17);
			this.firstNameLastNameRadioButton.TabIndex = 3;
			this.firstNameLastNameRadioButton.TabStop = true;
			this.firstNameLastNameRadioButton.Text = "firstNameLastNameRadioButton";
			this.firstNameLastNameRadioButton.UseVisualStyleBackColor = true;
			this.firstNameInitialLastNameRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.firstNameInitialLastNameRadioButton.AutoSize = true;
			this.firstNameInitialLastNameRadioButton.Location = new global::System.Drawing.Point(32, 75);
			this.firstNameInitialLastNameRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 4, 0, 0);
			this.firstNameInitialLastNameRadioButton.Name = "firstNameInitialLastNameRadioButton";
			this.firstNameInitialLastNameRadioButton.Size = new global::System.Drawing.Size(381, 17);
			this.firstNameInitialLastNameRadioButton.TabIndex = 4;
			this.firstNameInitialLastNameRadioButton.TabStop = true;
			this.firstNameInitialLastNameRadioButton.Text = "firstNameInitialLastNameRadioButton";
			this.firstNameInitialLastNameRadioButton.UseVisualStyleBackColor = true;
			this.firstNameLastNameInitialRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.firstNameLastNameInitialRadioButton.AutoSize = true;
			this.firstNameLastNameInitialRadioButton.Location = new global::System.Drawing.Point(32, 96);
			this.firstNameLastNameInitialRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 4, 0, 0);
			this.firstNameLastNameInitialRadioButton.Name = "firstNameLastNameInitialRadioButton";
			this.firstNameLastNameInitialRadioButton.Size = new global::System.Drawing.Size(381, 17);
			this.firstNameLastNameInitialRadioButton.TabIndex = 5;
			this.firstNameLastNameInitialRadioButton.TabStop = true;
			this.firstNameLastNameInitialRadioButton.Text = "firstNameLastNameInitialRadioButton";
			this.firstNameLastNameInitialRadioButton.UseVisualStyleBackColor = true;
			this.lastNameFirstNameRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.lastNameFirstNameRadioButton.AutoSize = true;
			this.lastNameFirstNameRadioButton.Location = new global::System.Drawing.Point(32, 117);
			this.lastNameFirstNameRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 4, 0, 0);
			this.lastNameFirstNameRadioButton.Name = "lastNameFirstNameRadioButton";
			this.lastNameFirstNameRadioButton.Size = new global::System.Drawing.Size(381, 17);
			this.lastNameFirstNameRadioButton.TabIndex = 6;
			this.lastNameFirstNameRadioButton.TabStop = true;
			this.lastNameFirstNameRadioButton.Text = "lastNameFirstNameRadioButton";
			this.lastNameFirstNameRadioButton.UseVisualStyleBackColor = true;
			this.lastNameInitialFirstNameRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.lastNameInitialFirstNameRadioButton.AutoSize = true;
			this.lastNameInitialFirstNameRadioButton.Location = new global::System.Drawing.Point(32, 138);
			this.lastNameInitialFirstNameRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 4, 0, 0);
			this.lastNameInitialFirstNameRadioButton.Name = "lastNameInitialFirstNameRadioButton";
			this.lastNameInitialFirstNameRadioButton.Size = new global::System.Drawing.Size(381, 17);
			this.lastNameInitialFirstNameRadioButton.TabIndex = 7;
			this.lastNameInitialFirstNameRadioButton.TabStop = true;
			this.lastNameInitialFirstNameRadioButton.Text = "lastNameInitialFirstNameRadioButton";
			this.lastNameInitialFirstNameRadioButton.UseVisualStyleBackColor = true;
			this.lastNameFirstNameInitialRadioButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.lastNameFirstNameInitialRadioButton.AutoSize = true;
			this.lastNameFirstNameInitialRadioButton.Location = new global::System.Drawing.Point(32, 159);
			this.lastNameFirstNameInitialRadioButton.Margin = new global::System.Windows.Forms.Padding(3, 4, 0, 0);
			this.lastNameFirstNameInitialRadioButton.Name = "lastNameFirstNameInitialRadioButton";
			this.lastNameFirstNameInitialRadioButton.Size = new global::System.Drawing.Size(381, 17);
			this.lastNameFirstNameInitialRadioButton.TabIndex = 8;
			this.lastNameFirstNameInitialRadioButton.TabStop = true;
			this.lastNameFirstNameInitialRadioButton.Text = "lastNameFirstNameInitialRadioButton";
			this.lastNameFirstNameInitialRadioButton.UseVisualStyleBackColor = true;
			this.localPartCheckBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.localPartCheckBox.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.localPartCheckBox, 2);
			this.localPartCheckBox.Location = new global::System.Drawing.Point(16, 12);
			this.localPartCheckBox.Margin = new global::System.Windows.Forms.Padding(3, 0, 0, 0);
			this.localPartCheckBox.Name = "localPartCheckBox";
			this.localPartCheckBox.Size = new global::System.Drawing.Size(397, 17);
			this.localPartCheckBox.TabIndex = 1;
			this.localPartCheckBox.Text = "localPartCheckBox";
			this.localPartCheckBox.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelVisible = true;
			base.ClientSize = new global::System.Drawing.Size(437, 348);
			base.Margin = new global::System.Windows.Forms.Padding(5, 3, 5, 3);
			base.Name = "SmtpProxyAddressTemplateDialog";
			base.OkEnabled = true;
			base.ContentPage.Controls.Add(this.domainTableLayoutPanel);
			base.ContentPage.Controls.Add(this.tableLayoutPanel1);
			this.domainTableLayoutPanel.ResumeLayout(false);
			this.domainTableLayoutPanel.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ContentPage.ResumeLayout(false);
			base.ContentPage.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel tableLayoutPanel1;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton aliasRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton firstNameLastNameRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton firstNameInitialLastNameRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton firstNameLastNameInitialRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton lastNameFirstNameRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton lastNameInitialFirstNameRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton lastNameFirstNameInitialRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightCheckBox localPartCheckBox;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel domainTableLayoutPanel;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton acceptedDomainRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoHeightRadioButton customDomainRadioButton;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.PickerLauncherTextBox acceptedDomainPickerLauncherTextBox;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeTextBox customDomainTextBox;
	}
}
