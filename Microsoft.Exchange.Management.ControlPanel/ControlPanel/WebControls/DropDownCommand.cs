using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class DropDownCommand : Command
	{
		[MergableProperty(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[RefreshProperties(RefreshProperties.All)]
		public CommandCollection Commands
		{
			get
			{
				if (this.commands == null)
				{
					this.HasCommand = true;
					this.commands = new CommandCollection();
				}
				return this.commands;
			}
		}

		[Conditional("DEBUG")]
		private void VerifySubCommandIcon()
		{
			if (this.HasCommand && !this.AllowAddSubCommandIcon)
			{
				foreach (Command command in this.commands)
				{
				}
			}
		}

		[DefaultValue(false)]
		public bool AllowAddSubCommandIcon { get; set; }

		[DefaultValue(false)]
		public bool HasCommand { get; set; }

		public string DefaultCommandName { get; set; }

		public bool HideArrow { get; set; }

		public Command GetDefaultCommand()
		{
			if (string.IsNullOrEmpty(this.DefaultCommandName))
			{
				return null;
			}
			foreach (Command command in this.Commands)
			{
				if (command.Name == this.DefaultCommandName)
				{
					return command;
				}
			}
			return null;
		}

		public override bool IsAccessibleToUser(IPrincipal user)
		{
			if (this.HasCommand)
			{
				foreach (Command command in this.Commands)
				{
					if (!(command is SeparatorCommand) && command.IsAccessibleToUser(user))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal void ApplyRolesFilter(IPrincipal user)
		{
			if (!this.rolesFilterApplied)
			{
				if (!this.HasCommand)
				{
					return;
				}
				for (int i = this.Commands.Count - 1; i >= 0; i--)
				{
					Command command = this.Commands[i];
					if (!command.IsAccessibleToUser(user))
					{
						this.Commands.RemoveAt(i);
					}
				}
				for (int j = this.Commands.Count - 1; j >= 0; j--)
				{
					if (this.Commands[j] is SeparatorCommand && (j == this.Commands.Count - 1 || j == 0 || this.Commands[j - 1] is SeparatorCommand))
					{
						this.Commands.RemoveAt(j);
					}
				}
				this.Commands.MakeReadOnly();
				this.rolesFilterApplied = true;
			}
		}

		private CommandCollection commands;

		private bool rolesFilterApplied;
	}
}
