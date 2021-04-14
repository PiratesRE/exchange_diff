using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeTextBox : EnhancedTextBox
	{
		public ExchangeTextBox()
		{
			base.DataBindings.CollectionChanged += this.DataBindings_CollectionChanged;
		}

		protected override void OnBindingContextChanged(EventArgs e)
		{
			base.OnBindingContextChanged(e);
			this.DataBindings_CollectionChanged(null, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}

		private void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if (!base.DesignMode)
			{
				Binding binding = (Binding)e.Element;
				if (e.Action == CollectionChangeAction.Add && binding.PropertyName == "Text" && this.constraintProvider == null)
				{
					this.constraintProvider = new TextBoxConstraintProvider(this, this);
				}
			}
		}

		private TextBoxConstraintProvider constraintProvider;
	}
}
