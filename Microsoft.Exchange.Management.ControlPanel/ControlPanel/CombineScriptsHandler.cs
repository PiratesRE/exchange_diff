using System;
using System.Web;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class CombineScriptsHandler : Page
	{
		public CombineScriptsHandler()
		{
			if (ToolkitScriptManager.CacheScriptBuckets == null)
			{
				ToolkitScriptManager toolkitScriptManager = new ToolkitScriptManager();
				toolkitScriptManager.SkinID = (this.Theme = "Default");
				this.Controls.Add(toolkitScriptManager);
			}
		}

		public override void ProcessRequest(HttpContext context)
		{
			if (ToolkitScriptManager.CacheScriptBuckets == null)
			{
				base.ProcessRequest(context);
			}
			try
			{
				if (!ToolkitScriptManager.OutputCombineScriptResourcesFile(context))
				{
					throw new BadRequestException(new Exception("'ToolkitScriptManager' could not generate combined script resources file."));
				}
			}
			catch (Exception ex)
			{
				EcpEventLogConstants.Tuple_ScriptRequestFailed.LogPeriodicFailure(EcpEventLogExtensions.GetUserNameToLog(), context.GetRequestUrlForLog(), ex, EcpEventLogExtensions.GetFlightInfoForLog());
				throw new BadRequestException(ex);
			}
		}
	}
}
