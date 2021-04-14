using System;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class SnapInUIService : UIService
	{
		public SnapInUIService(NamespaceSnapInBase snapIn) : base(null)
		{
			this.snapIn = snapIn;
		}

		public SnapInUIService(NamespaceSnapInBase snapIn, Control control) : base(control)
		{
			this.snapIn = snapIn;
		}

		public override void SetUIDirty()
		{
			this.snapIn.IsModified = true;
		}

		protected override DialogResult OnShowDialog(Form form)
		{
			DialogResult result;
			try
			{
				result = this.snapIn.Console.ShowDialog(form);
			}
			catch (InvalidOperationException)
			{
				result = form.ShowDialog(base.GetDialogOwnerWindow());
			}
			return result;
		}

		private NamespaceSnapInBase snapIn;
	}
}
