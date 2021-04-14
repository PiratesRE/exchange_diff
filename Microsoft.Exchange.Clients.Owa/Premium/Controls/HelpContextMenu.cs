using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class HelpContextMenu : ContextMenu
	{
		public HelpContextMenu(UserContext userContext) : base("divHelpContextMenu", userContext, false)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div class=\"alertPopupShading\"></div>");
			output.Write("<div id=\"divHelpMnuTopBorderLeft\" class=\"alertDialogTopBorder\"></div>");
			output.Write("<div id=\"divHelpMnuTopBorderRight\" class=\"alertDialogTopBorder\"></div>");
			base.RenderMenuItem(output, 1454393937, "openHelp", "mnuItmTxtItm");
			if (PerformanceConsole.IsPerformanceConsoleEnabled(base.UserContext))
			{
				base.RenderMenuItem(output, -824025661, "openPerfConsole", "mnuItmTxtItm");
			}
			base.RenderMenuItem(output, 539783673, "openAbout", "mnuItmTxtItm");
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.RenderPrivacyStatement.Enabled)
			{
				base.RenderMenuItem(output, -43540613, "divHelpCMPrcy", "openPrivacyStatement", "mnuItmTxtItm");
				base.RenderMenuItem(output, -2128835577, "divHelpCMCommunity", "openCommunitySite", "mnuItmTxtItm");
			}
		}
	}
}
