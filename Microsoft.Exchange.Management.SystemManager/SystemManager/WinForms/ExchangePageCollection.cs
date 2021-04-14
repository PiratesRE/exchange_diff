using System;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangePageCollection : Collection<ExchangePage>
	{
		internal ExchangePageCollection(DataContextFlags owner)
		{
			this.owner = owner;
		}

		protected override void InsertItem(int index, ExchangePage item)
		{
			base.InsertItem(index, item);
			this.ExchangePage_ContextChanged(item, EventArgs.Empty);
			item.ContextChanged += this.ExchangePage_ContextChanged;
		}

		protected override void RemoveItem(int index)
		{
			base.Items[index].ContextChanged -= this.ExchangePage_ContextChanged;
			base.RemoveItem(index);
		}

		private void ExchangePage_ContextChanged(object sender, EventArgs e)
		{
			ExchangePage exchangePage = (ExchangePage)sender;
			if (exchangePage.Context != null)
			{
				exchangePage.Context.Flags = this.owner;
			}
		}

		protected override void SetItem(int index, ExchangePage item)
		{
			base.RemoveAt(index);
			this.InsertItem(index, item);
		}

		private DataContextFlags owner;
	}
}
