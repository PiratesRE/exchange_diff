using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:ScriptManagerProxy runat=\"server\" />")]
	public class ScriptManagerProxy : ScriptManagerProxy
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (ToolkitScriptManager.CacheScriptBuckets == null)
			{
				throw new InvalidOperationException("ToolkitScriptManager must be put in this page at first.");
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (base.Scripts.Count > 0)
			{
				string fullName = typeof(ScriptManagerProxy).Assembly.FullName;
				foreach (ScriptReference scriptReference in base.Scripts)
				{
					if (string.IsNullOrEmpty(scriptReference.Assembly))
					{
						scriptReference.Assembly = fullName;
					}
				}
				ToolkitScriptManager.ExpandAndSort(base.Scripts);
			}
		}
	}
}
