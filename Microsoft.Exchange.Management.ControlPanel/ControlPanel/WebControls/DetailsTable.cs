using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ToolboxData("<{0}:DetailsTable runat=server></{0}:DetailsTable>")]
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class DetailsTable : HtmlTable, IScriptControl
	{
		protected override void OnPreRender(EventArgs e)
		{
			if (Util.IsSafari() || Util.IsFirefox() || Util.IsChrome())
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<DetailsTable>(this);
			}
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (!base.DesignMode && (Util.IsSafari() || Util.IsFirefox() || Util.IsChrome()))
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
			base.Render(writer);
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("DetailsTable", this.ClientID);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(DetailsTable));
		}
	}
}
