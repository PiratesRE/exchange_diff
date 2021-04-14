using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class WorkPanePage : TabPage
	{
		[DefaultValue(null)]
		public AbstractResultPane ResultPane
		{
			get
			{
				return this.resultPane;
			}
			set
			{
				if (this.ResultPane != null)
				{
					this.ResultPane.TextChanged -= this.ResultPane_TextChanged;
					this.Text = string.Empty;
					base.Controls.Remove(this.resultPane);
				}
				this.resultPane = value;
				if (this.ResultPane != null)
				{
					if (this.ResultPane.Dock != DockStyle.Fill)
					{
						this.ResultPane.Dock = DockStyle.Fill;
					}
					base.Name = this.ResultPane.Name;
					base.Controls.Add(this.ResultPane);
					this.ResultPane.TextChanged += this.ResultPane_TextChanged;
					this.ResultPane_TextChanged(this.ResultPane, EventArgs.Empty);
				}
			}
		}

		private void ResultPane_TextChanged(object sender, EventArgs e)
		{
			this.Text = this.ResultPane.Text;
		}

		private AbstractResultPane resultPane;
	}
}
