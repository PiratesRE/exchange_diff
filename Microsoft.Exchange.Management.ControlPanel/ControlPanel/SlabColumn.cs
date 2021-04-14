using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ParseChildren(true, "Slabs")]
	[DefaultProperty("Slabs")]
	public class SlabColumn : SlabComponent
	{
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<SlabControl> Slabs
		{
			get
			{
				return this.slabs;
			}
		}

		public Unit Width { get; set; }

		internal void Refactor()
		{
			IPrincipal user = this.Context.User;
			double num = 0.0;
			double num2 = 0.0;
			bool flag = false;
			for (int i = this.Slabs.Count - 1; i >= 0; i--)
			{
				SlabControl slabControl = this.Slabs[i];
				bool flag2 = false;
				slabControl.InitializeAsUserControl(this.Page);
				if (!slabControl.AccessibleToUser(user))
				{
					this.Slabs.RemoveAt(i);
					flag2 = true;
					slabControl.Visible = false;
				}
				if (slabControl.Height.Type == UnitType.Percentage)
				{
					num += slabControl.Height.Value;
					if (flag2)
					{
						flag = true;
					}
					else
					{
						num2 += slabControl.Height.Value;
					}
				}
			}
			if (flag && num2 != 0.0)
			{
				double num3 = num / num2;
				foreach (SlabControl slabControl2 in this.Slabs)
				{
					if (slabControl2.Height.Type == UnitType.Percentage)
					{
						slabControl2.Height = new Unit(slabControl2.Height.Value * num3, UnitType.Percentage);
					}
				}
			}
		}

		private List<SlabControl> slabs = new List<SlabControl>();
	}
}
