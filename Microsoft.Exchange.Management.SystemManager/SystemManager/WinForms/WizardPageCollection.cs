using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class WizardPageCollection : BindingList<WizardPage>
	{
		public WizardPageCollection(WizardPage parentPage)
		{
			if (parentPage == null)
			{
				throw new ArgumentNullException();
			}
			this.parentPage = parentPage;
		}

		public WizardPage ParentPage
		{
			get
			{
				return this.parentPage;
			}
		}

		protected override void InsertItem(int index, WizardPage item)
		{
			if (base.Contains(item))
			{
				if (index >= base.Count)
				{
					index--;
				}
				base.Remove(item);
			}
			item.ParentPage = this.parentPage;
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			WizardPage wizardPage = base[index];
			base.RemoveItem(index);
			wizardPage.ParentPage = null;
		}

		protected override void SetItem(int index, WizardPage item)
		{
			throw new NotSupportedException();
		}

		protected override void ClearItems()
		{
			WizardPage[] array = new WizardPage[base.Count];
			base.CopyTo(array, 0);
			base.ClearItems();
			foreach (WizardPage wizardPage in array)
			{
				wizardPage.ParentPage = null;
			}
		}

		private WizardPage parentPage;
	}
}
