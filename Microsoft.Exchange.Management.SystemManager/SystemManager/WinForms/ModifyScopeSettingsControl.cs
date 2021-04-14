using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ModifyScopeSettingsControl : ExchangePropertyPageControl
	{
		public ModifyScopeSettingsControl()
		{
			this.InitializeComponent();
			this.forestRadioButton.Text = Strings.ScopeControlForestRadioButtonText;
			this.domainRadioButton.Text = Strings.ScopeControlDomainRadioButtonText;
			this.domainLabel.Text = Strings.ScopeControlDomainLabelText;
			this.ouPickerLauncherTextBox.ButtonText = Strings.ScopeControlDomainBrowse;
			this.ouPicker = new AutomatedObjectPicker(new OrganizationalUnitConfigurable());
			this.ouPickerLauncherTextBox.Picker = this.ouPicker;
			this.ouPickerLauncherTextBox.ValueMember = "Identity";
			this.ouPickerLauncherTextBox.DisplayMember = "CanonicalName";
			base.BindingSource.DataSource = typeof(ScopeSettings);
			this.CausesValidation = false;
			this.forestRadioButton.DataBindings.Add("Checked", base.BindingSource, "ForestViewEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.domainRadioButton.DataBindings.Add("Checked", base.BindingSource, "DomainViewEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.ouPickerLauncherTextBox.DataBindings.Add("Enabled", base.BindingSource, "DomainViewEnabled", false);
			Binding binding = this.ouPickerLauncherTextBox.DataBindings.Add("SelectedValue", base.BindingSource, "OrganizationalUnit", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.DataSourceNullValue = null;
		}

		public ModifyScopeSettingsControl(ObjectPicker picker) : this()
		{
			this.Text = Strings.ModifyScopeSettingsControlTitle;
			this.Description = Strings.ModifyScopeSettingsControlDescription;
			this.ShouldScopingWithinDefaultDomainScope = picker.ShouldScopingWithinDefaultDomainScope;
		}

		protected override void OnKillActive(CancelEventArgs e)
		{
			base.OnKillActive(e);
			if (!e.Cancel && this.forestRadioButton.Checked)
			{
				DialogResult dialogResult = base.ShowMessage(Strings.ForestWarningText, MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.No)
				{
					e.Cancel = true;
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string Description
		{
			get
			{
				return this.descriptionLabel.Text;
			}
			set
			{
				this.descriptionLabel.Text = value;
			}
		}

		[DefaultValue(false)]
		public bool ShouldScopingWithinDefaultDomainScope
		{
			get
			{
				return this.shouldScopingWithinDefaultDomainScope;
			}
			set
			{
				this.shouldScopingWithinDefaultDomainScope = value;
				this.forestRadioButton.Enabled = !this.ShouldScopingWithinDefaultDomainScope;
				this.ouPicker.ScopeSupportingLevel = (this.ShouldScopingWithinDefaultDomainScope ? ScopeSupportingLevel.WithinDefaultScope : ScopeSupportingLevel.NoScoping);
			}
		}

		protected override Size DefaultMinimumSize
		{
			get
			{
				return new Size(418, 142);
			}
		}

		protected override Padding DefaultPadding
		{
			get
			{
				return new Padding(12, 12, 0, 12);
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
		public new bool CausesValidation
		{
			get
			{
				return base.CausesValidation;
			}
			set
			{
				base.CausesValidation = value;
			}
		}

		private void InitializeComponent()
		{
			this.descriptionLabel = new Label();
			this.forestRadioButton = new AutoHeightRadioButton();
			this.descriptionPanel = new AutoTableLayoutPanel();
			this.ouPanel = new AutoTableLayoutPanel();
			this.domainRadioButton = new AutoHeightRadioButton();
			this.domainLabel = new Label();
			this.ouPickerLauncherTextBox = new PickerLauncherTextBox();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.descriptionPanel.SuspendLayout();
			this.ouPanel.SuspendLayout();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			this.descriptionLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.descriptionLabel.AutoSize = true;
			this.descriptionPanel.SetColumnSpan(this.descriptionLabel, 2);
			this.descriptionLabel.Location = new Point(0, 0);
			this.descriptionLabel.Margin = new Padding(0);
			this.descriptionLabel.Name = "descriptionLabel";
			this.descriptionLabel.Text = "descriptionLabel";
			this.descriptionLabel.Size = new Size(390, 13);
			this.descriptionLabel.TabIndex = 0;
			this.forestRadioButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.forestRadioButton.AutoSize = true;
			this.forestRadioButton.Checked = true;
			this.descriptionPanel.SetColumnSpan(this.forestRadioButton, 2);
			this.forestRadioButton.Location = new Point(3, 16);
			this.forestRadioButton.Margin = new Padding(3, 3, 0, 0);
			this.forestRadioButton.Name = "forestRadioButton";
			this.forestRadioButton.Size = new Size(387, 17);
			this.forestRadioButton.TabIndex = 1;
			this.forestRadioButton.TabStop = true;
			this.forestRadioButton.Text = "forestRadioButton";
			this.descriptionPanel.AutoLayout = true;
			this.descriptionPanel.AutoSize = true;
			this.descriptionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.descriptionPanel.ColumnCount = 3;
			this.descriptionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.descriptionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.descriptionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.descriptionPanel.ContainerType = ContainerType.Control;
			this.descriptionPanel.Controls.Add(this.descriptionLabel, 0, 0);
			this.descriptionPanel.Controls.Add(this.forestRadioButton, 0, 1);
			this.descriptionPanel.Dock = DockStyle.Top;
			this.descriptionPanel.Location = new Point(12, 12);
			this.descriptionPanel.Margin = new Padding(0);
			this.descriptionPanel.Name = "descriptionPanel";
			this.descriptionPanel.RowCount = 2;
			this.descriptionPanel.RowStyles.Add(new RowStyle());
			this.descriptionPanel.RowStyles.Add(new RowStyle());
			this.descriptionPanel.Size = new Size(406, 33);
			this.descriptionPanel.TabIndex = 0;
			this.ouPanel.AutoLayout = false;
			this.ouPanel.AutoSize = true;
			this.ouPanel.ColumnCount = 3;
			this.ouPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.ouPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.ouPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.ouPanel.ContainerType = ContainerType.Control;
			this.ouPanel.Controls.Add(this.domainRadioButton, 0, 0);
			this.ouPanel.Controls.Add(this.domainLabel, 1, 1);
			this.ouPanel.Controls.Add(this.ouPickerLauncherTextBox, 1, 2);
			this.ouPanel.Dock = DockStyle.Top;
			this.ouPanel.Location = new Point(12, 45);
			this.ouPanel.Name = "ouPanel";
			this.ouPanel.RowCount = 3;
			this.ouPanel.RowStyles.Add(new RowStyle());
			this.ouPanel.RowStyles.Add(new RowStyle());
			this.ouPanel.RowStyles.Add(new RowStyle());
			this.ouPanel.Size = new Size(406, 71);
			this.ouPanel.TabIndex = 2;
			this.domainRadioButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.domainRadioButton.AutoSize = true;
			this.ouPanel.SetColumnSpan(this.domainRadioButton, 2);
			this.domainRadioButton.Location = new Point(3, 4);
			this.domainRadioButton.Margin = new Padding(3, 4, 0, 0);
			this.domainRadioButton.Name = "domainRadioButton";
			this.domainRadioButton.Size = new Size(387, 17);
			this.domainRadioButton.TabIndex = 0;
			this.domainRadioButton.Text = "domainRadioButton";
			this.domainLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.domainLabel.AutoSize = true;
			this.domainLabel.Location = new Point(16, 29);
			this.domainLabel.Margin = new Padding(0, 8, 0, 0);
			this.domainLabel.Name = "domainLabel";
			this.domainLabel.Size = new Size(374, 13);
			this.domainLabel.TabIndex = 1;
			this.domainLabel.Text = "domainLabel";
			this.ouPickerLauncherTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.ouPickerLauncherTextBox.AutoSize = true;
			this.ouPickerLauncherTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.ouPickerLauncherTextBox.Location = new Point(19, 48);
			this.ouPickerLauncherTextBox.Margin = new Padding(0, 6, 0, 0);
			this.ouPickerLauncherTextBox.Name = "ouPickerLauncherTextBox";
			this.ouPickerLauncherTextBox.Size = new Size(371, 23);
			this.ouPickerLauncherTextBox.TabIndex = 2;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.ouPanel);
			base.Controls.Add(this.descriptionPanel);
			this.MinimumSize = new Size(418, 142);
			base.Name = "ModifyScopeSettingsControl";
			base.Padding = new Padding(12, 12, 0, 12);
			base.Size = new Size(418, 142);
			((ISupportInitialize)base.BindingSource).EndInit();
			this.descriptionPanel.ResumeLayout(false);
			this.descriptionPanel.PerformLayout();
			this.ouPanel.ResumeLayout(false);
			this.ouPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private Label descriptionLabel;

		private AutoHeightRadioButton forestRadioButton;

		private AutoTableLayoutPanel ouPanel;

		private AutoHeightRadioButton domainRadioButton;

		private Label domainLabel;

		private PickerLauncherTextBox ouPickerLauncherTextBox;

		private AutoTableLayoutPanel descriptionPanel;

		private ObjectPicker ouPicker;

		private bool shouldScopingWithinDefaultDomainScope;
	}
}
