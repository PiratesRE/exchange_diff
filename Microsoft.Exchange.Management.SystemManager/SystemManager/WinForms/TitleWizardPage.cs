using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class TitleWizardPage : WizardPage
	{
		public TitleWizardPage()
		{
			this.InitializeComponent();
			this.ShortDescription = "";
		}

		private void InitializeComponent()
		{
			this.shortDescriptionLabel = new AutoHeightLabel();
			this.contentPanel = new Panel();
			((ISupportInitialize)base.BindingSource).BeginInit();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			this.shortDescriptionLabel.Dock = DockStyle.Top;
			this.shortDescriptionLabel.Location = new Point(0, 0);
			this.shortDescriptionLabel.Margin = new Padding(3, 3, 16, 3);
			this.shortDescriptionLabel.Padding = new Padding(0, 0, 16, 0);
			this.shortDescriptionLabel.Name = "shortDescriptionLabel";
			this.shortDescriptionLabel.Size = new Size(456, 17);
			this.shortDescriptionLabel.TabIndex = 1;
			this.shortDescriptionLabel.Text = "[shortDescription]";
			this.contentPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.contentPanel.Dock = DockStyle.Fill;
			this.contentPanel.Location = new Point(0, 17);
			this.contentPanel.Margin = new Padding(0);
			this.contentPanel.Name = "contentPanel";
			this.contentPanel.Size = new Size(456, 385);
			this.contentPanel.TabIndex = 2;
			base.Controls.Add(this.contentPanel);
			base.Controls.Add(this.shortDescriptionLabel);
			base.Name = "TitleWizardPage";
			((ISupportInitialize)base.BindingSource).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override Padding DefaultPadding
		{
			get
			{
				return new Padding(0);
			}
		}

		[DefaultValue("")]
		public string ShortDescription
		{
			get
			{
				return this.shortDescriptionLabel.Text;
			}
			set
			{
				this.shortDescriptionLabel.Text = value;
			}
		}

		public Panel ContentPanel
		{
			get
			{
				return this.contentPanel;
			}
		}

		private AutoHeightLabel shortDescriptionLabel;

		[AccessedThroughProperty("ContentPanel")]
		private Panel contentPanel;
	}
}
