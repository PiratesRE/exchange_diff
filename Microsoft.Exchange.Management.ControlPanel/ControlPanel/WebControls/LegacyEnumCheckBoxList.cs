using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ControlValueProperty("Value")]
	[ClientScriptResource("LegacyEnumCheckBoxList", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class LegacyEnumCheckBoxList : CheckBoxList, IScriptControl, INamingContainer
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.Attributes.Add("cellspacing", "0");
			base.Attributes.Add("cellpadding", "0");
			if (this.Page != null)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<LegacyEnumCheckBoxList>(this);
			}
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
			base.Render(writer);
		}

		public int[] CheckBoxValues
		{
			get
			{
				int[] array = new int[this.Items.Count];
				for (int i = 0; i < this.Items.Count; i++)
				{
					array[i] = int.Parse(this.Items[i].Value);
				}
				return array;
			}
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptComponentDescriptor scriptComponentDescriptor = new ScriptComponentDescriptor("LegacyEnumCheckBoxList");
			scriptComponentDescriptor.AddProperty("id", this.ClientID);
			scriptComponentDescriptor.AddProperty("CheckBoxValues", this.CheckBoxValues);
			return new ScriptDescriptor[]
			{
				scriptComponentDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(LegacyEnumCheckBoxList));
		}
	}
}
