using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ProgressDialog : ExchangeDialog
	{
		public ProgressDialog()
		{
			this.InitializeComponent();
			Application.Idle += this.Application_Idle;
			base.OkVisible = false;
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			base.HelpVisible = false;
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			if (this.updateControls)
			{
				this.updateControls = false;
				if (this.UseMarquee)
				{
					this.progressBar.Style = ProgressBarStyle.Marquee;
				}
				else
				{
					this.progressBar.Style = ProgressBarStyle.Continuous;
				}
				this.progressBar.Maximum = this.Maximum;
				this.progressBar.Value = this.Value;
				this.statusLabel.Text = this.StatusText;
			}
		}

		[DefaultValue(0)]
		public int Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (value != this.Value)
				{
					this.value = value;
					this.updateControls = true;
					base.Invalidate();
				}
			}
		}

		[DefaultValue(100)]
		public int Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				if (value != this.Maximum)
				{
					this.maximum = value;
					this.updateControls = true;
					base.Invalidate();
				}
			}
		}

		[DefaultValue(false)]
		public bool UseMarquee
		{
			get
			{
				return this.useMarquee;
			}
			set
			{
				if (value != this.UseMarquee)
				{
					this.useMarquee = value;
					this.updateControls = true;
					base.Invalidate();
				}
			}
		}

		[DefaultValue("")]
		public string StatusText
		{
			get
			{
				return this.statusText;
			}
			set
			{
				value = (value ?? "");
				if (value != this.StatusText)
				{
					this.statusText = value;
					this.updateControls = true;
					base.Invalidate();
				}
			}
		}

		private bool updateControls;

		private int value;

		private int maximum = 100;

		private bool useMarquee;

		private string statusText = "";
	}
}
