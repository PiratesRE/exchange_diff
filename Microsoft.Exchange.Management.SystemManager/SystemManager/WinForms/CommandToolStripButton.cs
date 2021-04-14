using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CommandToolStripButton : ToolStripButton
	{
		public CommandToolStripButton()
		{
		}

		public CommandToolStripButton(Command command)
		{
			this.Command = command;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Command = null;
			}
			base.Dispose(disposing);
		}

		[DefaultValue(null)]
		public Command Command
		{
			get
			{
				return this.command;
			}
			set
			{
				if (value != this.Command)
				{
					if (this.Command != null)
					{
						this.Command.TextChanged -= this.Command_TextChanged;
						this.Command.DescriptionChanged -= this.Command_DescriptionChanged;
						this.Command.EnabledChanged -= this.Command_EnabledChanged;
						this.Command.VisibleChanged -= this.Command_VisibleChanged;
						this.Command.CheckedChanged -= this.Command_CheckedChanged;
						this.Command.IconChanged -= this.Command_IconChanged;
						this.Command.StyleChanged -= this.Command_StyleChanged;
					}
					this.command = value;
					if (this.Command != null)
					{
						this.Command_TextChanged(this.Command, EventArgs.Empty);
						this.Command_DescriptionChanged(this.Command, EventArgs.Empty);
						this.Command_EnabledChanged(this.Command, EventArgs.Empty);
						this.Command_VisibleChanged(this.Command, EventArgs.Empty);
						this.Command_CheckedChanged(this.Command, EventArgs.Empty);
						this.Command_IconChanged(this.Command, EventArgs.Empty);
						this.Command_StyleChanged(this.Command, EventArgs.Empty);
						this.Command.TextChanged += this.Command_TextChanged;
						this.Command.DescriptionChanged += this.Command_DescriptionChanged;
						this.Command.EnabledChanged += this.Command_EnabledChanged;
						this.Command.VisibleChanged += this.Command_VisibleChanged;
						this.Command.CheckedChanged += this.Command_CheckedChanged;
						this.Command.IconChanged += this.Command_IconChanged;
						this.Command.StyleChanged += this.Command_StyleChanged;
						return;
					}
					this.DisposeImage();
				}
			}
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			this.Command.Invoke();
		}

		private void Command_TextChanged(object sender, EventArgs e)
		{
			this.Text = this.Command.Text;
		}

		private void Command_DescriptionChanged(object sender, EventArgs e)
		{
			base.ToolTipText = this.Command.Description;
		}

		private void Command_EnabledChanged(object sender, EventArgs e)
		{
			this.Enabled = this.Command.Enabled;
		}

		private void Command_VisibleChanged(object sender, EventArgs e)
		{
			base.Visible = this.Command.Visible;
		}

		private void Command_CheckedChanged(object sender, EventArgs e)
		{
			base.Checked = this.Command.Checked;
		}

		private void Command_IconChanged(object sender, EventArgs e)
		{
			this.DisposeImage();
			if (this.Command.Icon != null)
			{
				this.Image = IconLibrary.ToSmallBitmap(this.Command.Icon);
			}
		}

		private void DisposeImage()
		{
			if (this.Image != null)
			{
				this.Image.Dispose();
				this.Image = null;
			}
		}

		private void Command_StyleChanged(object sender, EventArgs e)
		{
			switch (this.Command.Style)
			{
			case 0:
			case 2:
			case 4:
				this.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
				return;
			case 1:
				this.DisplayStyle = ToolStripItemDisplayStyle.Text;
				break;
			case 3:
			case 5:
			case 6:
			case 7:
				break;
			case 8:
				this.DisplayStyle = ToolStripItemDisplayStyle.Image;
				return;
			default:
				return;
			}
		}

		private Command command;
	}
}
