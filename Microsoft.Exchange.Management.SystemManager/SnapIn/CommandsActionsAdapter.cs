using System;
using System.Collections.Generic;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SnapIn
{
	public class CommandsActionsAdapter : IDisposable
	{
		public CommandsActionsAdapter(IServiceProvider serviceProvider, ActionsPaneItemCollection actions, CommandCollection commands, bool showGroupsAsRegions, IExchangeSnapIn snapIn, ExchangeFormView view) : this(serviceProvider, actions, commands, showGroupsAsRegions, snapIn, view, false)
		{
		}

		public CommandsActionsAdapter(IServiceProvider serviceProvider, ActionsPaneItemCollection actions, CommandCollection commands, bool showGroupsAsRegions, IExchangeSnapIn snapIn, ExchangeFormView view, bool createActionsAtBottom)
		{
			this.snapIn = snapIn;
			this.serviceProvider = serviceProvider;
			this.actions = actions;
			this.commands = commands;
			this.adapters = new List<CommandActionAdapter>(commands.Count);
			this.showGroupsAsRegions = showGroupsAsRegions;
			this.createActionsAtBottom = createActionsAtBottom;
			this.view = view;
			this.commands.CommandAdded += new CommandEventHandler(this.commands_CommandAdded);
			this.commands.CommandRemoved += new CommandEventHandler(this.commands_CommandRemoved);
			ActionsPaneItemCollection actionsPaneItemCollection = new ActionsPaneItemCollection();
			for (int i = 0; i < this.commands.Count; i++)
			{
				CommandActionAdapter commandActionAdapter = this.CreateAdapter(this.commands[i]);
				this.adapters.Add(commandActionAdapter);
				if (commandActionAdapter.Command.Visible)
				{
					actionsPaneItemCollection.Add(commandActionAdapter.ActionItem);
				}
			}
			this.actions.AddRange(actionsPaneItemCollection.ToArray());
		}

		public void Dispose()
		{
			this.actions.Clear();
			for (int i = 0; i < this.adapters.Count; i++)
			{
				this.DisposeAdapter(this.adapters[i]);
			}
			this.commands.CommandAdded -= new CommandEventHandler(this.commands_CommandAdded);
			this.commands.CommandRemoved -= new CommandEventHandler(this.commands_CommandRemoved);
			this.commands = null;
			this.actions = null;
			this.snapIn = null;
			this.view = null;
		}

		private CommandActionAdapter CreateAdapter(Command command)
		{
			command.EnabledChanged += this.DelayUpdates;
			command.VisibleChanged += this.Command_VisibleChanged;
			return new CommandActionAdapter(this.serviceProvider, command, this.showGroupsAsRegions, this.snapIn, this.view);
		}

		private void DisposeAdapter(CommandActionAdapter adapter)
		{
			adapter.Command.EnabledChanged -= this.DelayUpdates;
			adapter.Command.VisibleChanged -= this.Command_VisibleChanged;
			adapter.Dispose();
		}

		private void commands_CommandAdded(object sender, CommandEventArgs e)
		{
			this.DelayUpdates(sender, e);
			this.adapters.Insert(this.commands.IndexOf(e.Command), this.CreateAdapter(e.Command));
			if (e.Command.Visible)
			{
				this.Command_VisibleChanged(e.Command, EventArgs.Empty);
			}
		}

		private void commands_CommandRemoved(object sender, CommandEventArgs e)
		{
			this.DelayUpdates(sender, e);
			foreach (CommandActionAdapter commandActionAdapter in this.adapters)
			{
				if (commandActionAdapter.Command == e.Command)
				{
					if (this.actions.Contains(commandActionAdapter.ActionItem))
					{
						this.actions.Remove(commandActionAdapter.ActionItem);
					}
					this.DisposeAdapter(commandActionAdapter);
					this.adapters.Remove(commandActionAdapter);
					break;
				}
			}
		}

		private void Command_VisibleChanged(object sender, EventArgs e)
		{
			this.DelayUpdates(sender, e);
			Command command = (Command)sender;
			int num = this.commands.IndexOf(command);
			if (command.Visible)
			{
				this.actions.Insert(this.GetActionInsertPosition(num), this.adapters[num].ActionItem);
				return;
			}
			this.actions.Remove(this.adapters[num].ActionItem);
		}

		private int GetActionInsertPosition(int commandIndex)
		{
			int num = this.createActionsAtBottom ? 1 : -1;
			int num2 = commandIndex + num;
			while (num2 >= 0 && num2 < this.commands.Count)
			{
				if (this.commands[num2].Visible && this.actions.Contains(this.adapters[num2].ActionItem))
				{
					int num3 = this.actions.IndexOf(this.adapters[num2].ActionItem);
					if (!this.createActionsAtBottom)
					{
						num3++;
					}
					return num3;
				}
				num2 += num;
			}
			if (!this.createActionsAtBottom)
			{
				return 0;
			}
			return this.actions.Count;
		}

		private void DelayUpdates(object sender, EventArgs e)
		{
			if (this.view != null)
			{
				this.view.DelayUpdates();
			}
		}

		private IExchangeSnapIn snapIn;

		private IServiceProvider serviceProvider;

		private ActionsPaneItemCollection actions;

		private CommandCollection commands;

		private List<CommandActionAdapter> adapters;

		private ExchangeFormView view;

		private bool createActionsAtBottom;

		private bool showGroupsAsRegions;
	}
}
