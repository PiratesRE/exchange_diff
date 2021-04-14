using System;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class ViewUIService : SnapInUIService
	{
		public ViewUIService(FormView view) : base(view.SnapIn, view.Control)
		{
			this.view = view;
		}

		public override void SetUIDirty()
		{
			this.view.IsModified = true;
		}

		private View view;
	}
}
