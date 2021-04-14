using System;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class CommandCollection : Collection<Command>
	{
		public Command FindCommandByName(string name)
		{
			foreach (Command command in this)
			{
				if (string.Equals(command.Name, name, StringComparison.InvariantCultureIgnoreCase))
				{
					return command;
				}
			}
			return null;
		}

		internal void MakeReadOnly()
		{
			this.isReadOnly = true;
		}

		protected override void InsertItem(int index, Command item)
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException("The collection is read-only.");
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, Command item)
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException("The collection is read-only.");
			}
			base.SetItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException("The collection is read-only.");
			}
			base.RemoveItem(index);
		}

		protected override void ClearItems()
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException("The collection is read-only.");
			}
			base.ClearItems();
		}

		public bool ContainsVisibleCommands()
		{
			for (int i = 0; i < base.Count; i++)
			{
				if (base[i].Visible)
				{
					return true;
				}
			}
			return false;
		}

		private bool isReadOnly;
	}
}
