using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.Services;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CommandToolStripMenuItem : ToolStripMenuItem, IServiceProvider
	{
		public CommandToolStripMenuItem()
		{
			this.servicedComponents = new ServicedContainer(this);
		}

		public CommandToolStripMenuItem(IContainer container) : this()
		{
			container.Add(this);
		}

		public CommandToolStripMenuItem(IContainer container, Command command) : this(container)
		{
			this.Command = command;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.Command = null;
				this.servicedComponents.Dispose();
			}
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
						this.Command.CommandAdded -= new CommandEventHandler(this.Command_CommandAdded);
						this.Command.CommandRemoved -= new CommandEventHandler(this.Command_CommandRemoved);
						this.Command.StyleChanged -= this.Command_StyleChanged;
						if (this.Command.HasCommands)
						{
							for (int i = base.DropDownItems.Count - 1; i >= 0; i--)
							{
								base.DropDownItems[i].Dispose();
							}
						}
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
						this.Command.CommandAdded += new CommandEventHandler(this.Command_CommandAdded);
						this.Command.CommandRemoved += new CommandEventHandler(this.Command_CommandRemoved);
						if (!this.Command.HasCommands)
						{
							return;
						}
						using (IEnumerator enumerator = this.Command.Commands.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								Command sourceCommand = (Command)obj;
								base.DropDownItems.Add(this.CreateMenuItem(sourceCommand));
							}
							return;
						}
					}
					this.DisposeImage();
				}
			}
		}

		protected virtual ToolStripItem CreateMenuItem(Command sourceCommand)
		{
			if (sourceCommand.IsSeparator)
			{
				return new CommandToolStripSeparator(sourceCommand);
			}
			return new CommandToolStripMenuItem(this.servicedComponents, sourceCommand);
		}

		public virtual ToolStripItem FindMenuItem(Command commandToFind)
		{
			foreach (object obj in base.DropDownItems)
			{
				ToolStripItem toolStripItem = (ToolStripItem)obj;
				if (toolStripItem is CommandToolStripMenuItem && (toolStripItem as CommandToolStripMenuItem).Command == commandToFind)
				{
					return toolStripItem;
				}
				if (toolStripItem is CommandToolStripSeparator && (toolStripItem as CommandToolStripSeparator).Command == commandToFind)
				{
					return toolStripItem;
				}
			}
			return null;
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

		private void Command_CommandAdded(object sender, CommandEventArgs e)
		{
			base.DropDownItems.Insert(this.Command.Commands.IndexOf(e.Command), this.CreateMenuItem(e.Command));
		}

		private void Command_CommandRemoved(object sender, CommandEventArgs e)
		{
			ToolStripItem toolStripItem = this.FindMenuItem(e.Command);
			if (toolStripItem != null)
			{
				base.DropDownItems.Remove(toolStripItem);
				this.servicedComponents.Remove(toolStripItem);
				toolStripItem.Dispose();
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

		object IServiceProvider.GetService(Type serviceType)
		{
			return this.GetService(serviceType);
		}

		private Command command;

		private ServicedContainer servicedComponents;
	}
}
