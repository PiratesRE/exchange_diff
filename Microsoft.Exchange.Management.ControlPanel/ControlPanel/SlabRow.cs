using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ParseChildren(true, "Content")]
	[DefaultProperty("Content")]
	public class SlabRow : SlabComponent
	{
		public SlabRow()
		{
			this.Content = new List<Control>();
		}

		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<Control> Content { get; private set; }

		internal void Refactor()
		{
			IPrincipal user = this.Context.User;
			for (int i = this.Content.Count - 1; i >= 0; i--)
			{
				SlabControl slabControl = this.Content[i] as SlabControl;
				if (slabControl != null && !slabControl.AccessibleToUser(user))
				{
					this.Content.RemoveAt(i);
					slabControl.Visible = false;
				}
			}
		}
	}
}
