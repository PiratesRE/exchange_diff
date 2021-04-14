using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class NumericUpDownBulkEditorAdapter : BulkEditorAdapter
	{
		public NumericUpDownBulkEditorAdapter(Control ctrl) : base(ctrl)
		{
		}

		protected override void OnStateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			base.OnStateChanged(sender, e);
			if (base["Value"] == 3)
			{
				base.HostControl.Enabled = false;
			}
		}

		protected override IList<string> InnerGetManagedProperties()
		{
			IList<string> list = base.InnerGetManagedProperties();
			list.Add("Value");
			return list;
		}

		private const string ManagedPropertyName = "Value";
	}
}
