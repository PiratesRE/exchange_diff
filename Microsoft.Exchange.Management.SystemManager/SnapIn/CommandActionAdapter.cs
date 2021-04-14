using System;
using System.Threading;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SnapIn
{
	public class CommandActionAdapter : IDisposable
	{
		public CommandActionAdapter(IServiceProvider serviceProvider, Command command, bool treatCommandAsGroup, IExchangeSnapIn snapIn, ExchangeFormView view)
		{
			this.serviceProvider = serviceProvider;
			this.command = command;
			if (this.command.Name == "-")
			{
				this.actionItem = new ActionSeparator();
			}
			else
			{
				if (this.command.HasCommands || this.command.Style == 4 || treatCommandAsGroup)
				{
					ActionGroup actionGroup = new ActionGroup();
					actionGroup.RenderAsRegion = true;
					this.actionItem = actionGroup;
					this.groupItemsAdapter = new CommandsActionsAdapter(this.serviceProvider, actionGroup.Items, this.Command.Commands, false, snapIn, view);
				}
				else
				{
					Action action = new Action();
					this.actionItem = action;
					action.Triggered += new Action.ActionEventHandler(this.action_Triggered);
					this.Command.EnabledChanged += this.command_EnabledChanged;
					this.command_EnabledChanged(this.Command, EventArgs.Empty);
					this.Command.CheckedChanged += this.command_CheckedChanged;
					this.command_CheckedChanged(this.Command, EventArgs.Empty);
				}
				this.Command.TextChanged += this.command_TextChanged;
				this.command_TextChanged(this.Command, EventArgs.Empty);
				this.Command.DescriptionChanged += this.command_DescriptionChanged;
				this.command_DescriptionChanged(this.Command, EventArgs.Empty);
				if (snapIn != null)
				{
					(this.ActionItem as ActionsPaneExtendedItem).ImageIndex = snapIn.RegisterIcon(this.Command.Name, this.Command.Icon);
				}
				(this.ActionItem as ActionsPaneExtendedItem).LanguageIndependentName = this.ActionItem.GetHashCode().ToString();
			}
			this.ActionItem.Tag = this;
		}

		public ActionsPaneItem ActionItem
		{
			get
			{
				return this.actionItem;
			}
		}

		public Command Command
		{
			get
			{
				return this.command;
			}
		}

		public void Dispose()
		{
			this.Command.TextChanged -= this.command_TextChanged;
			this.Command.DescriptionChanged -= this.command_DescriptionChanged;
			this.Command.EnabledChanged -= this.command_EnabledChanged;
			this.Command.CheckedChanged -= this.command_CheckedChanged;
			Action action = this.ActionItem as Action;
			if (action != null)
			{
				action.Triggered -= new Action.ActionEventHandler(this.action_Triggered);
				return;
			}
			if (this.groupItemsAdapter != null)
			{
				this.groupItemsAdapter.Dispose();
				this.groupItemsAdapter = null;
			}
		}

		private void command_EnabledChanged(object sender, EventArgs e)
		{
			Action action = this.ActionItem as Action;
			if (this.command.Enabled)
			{
				action.Enabled = true;
				return;
			}
			action.Enabled = false;
		}

		private void command_CheckedChanged(object sender, EventArgs e)
		{
			Action action = this.ActionItem as Action;
			action.Checked = this.command.Checked;
		}

		private void action_Triggered(object sender, ActionEventArgs e)
		{
			try
			{
				ExTraceGlobals.ProgramFlowTracer.TraceFunction<string>(0L, "*--CommandActionAdapter.action_Triggered: Invoking command {0}", this.Command.Name);
				SynchronizationContext synchronizationContext = (SynchronizationContext)this.serviceProvider.GetService(typeof(SynchronizationContext));
				SynchronizationContext.SetSynchronizationContext(synchronizationContext);
				this.Command.Invoke();
			}
			catch (Exception ex)
			{
				if (ExceptionHelper.IsUICriticalException(ex))
				{
					throw;
				}
				IUIService iuiservice = (IUIService)this.serviceProvider.GetService(typeof(IUIService));
				iuiservice.ShowError(ex);
			}
		}

		private void command_TextChanged(object sender, EventArgs e)
		{
			(this.ActionItem as ActionsPaneExtendedItem).DisplayName = this.Command.Text;
		}

		private void command_DescriptionChanged(object sender, EventArgs e)
		{
			(this.ActionItem as ActionsPaneExtendedItem).Description = ExchangeUserControl.RemoveAccelerator(this.Command.Description);
		}

		private IServiceProvider serviceProvider;

		private ActionsPaneItem actionItem;

		private CommandsActionsAdapter groupItemsAdapter;

		private Command command;
	}
}
