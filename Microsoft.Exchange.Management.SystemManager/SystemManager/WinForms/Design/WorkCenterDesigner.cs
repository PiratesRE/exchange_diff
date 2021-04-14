using System;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Microsoft.Exchange.Management.SystemManager.WinForms.Design
{
	public class WorkCenterDesigner : ScrollableControlDesigner
	{
		public WorkCenter WorkCenter
		{
			get
			{
				return this.Control as WorkCenter;
			}
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			base.EnableDesignMode(this.WorkCenter.TopPanel, "TopPanel");
		}
	}
}
