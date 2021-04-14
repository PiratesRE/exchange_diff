using System;
using System.Web;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NavBarDiagnostics : DiagnosticsPage
	{
		protected void RenderNavBarDetails()
		{
			base.Response.Write("<div class='diagBlock'>");
			NavBarDiagnosticsDetails navBarDiagnosticsDetails = new NavBarDiagnosticsDetails();
			HttpContext.Current.Cache["NavBarDiagnostics.Details"] = navBarDiagnosticsDetails;
			base.Write("Use the information below to troubleshoot problems and report issues to technical support.");
			base.Write("NavBar configuration information");
			NavBarClientBase.RenderConfigInformation(base.Response.Output);
			Identity identity = (base.Request.QueryString["flight"] == "geminishellux") ? new Identity("myself", "myself") : new Identity("myorg", "myorg");
			Exception ex = null;
			try
			{
				PowerShellResults<NavBarPack> @object = new NavBar().GetObject(identity);
				base.Write("NavBar data:", @object.HasValue ? @object.Output[0].ToJsonString(null) : null);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			finally
			{
				if (navBarDiagnosticsDetails.Exception != null)
				{
					base.Write("Exception:", navBarDiagnosticsDetails.Exception.ToString());
				}
				if (ex != null)
				{
					base.Write("Additional Exception:", ex.ToString());
				}
				base.Response.Write("</div>");
			}
		}

		internal const string DetailsKey = "NavBarDiagnostics.Details";
	}
}
