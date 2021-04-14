using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("OwaPickerUtil", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:OwaPickerUtil runat=server></{0}:OwaPickerUtil>")]
	public class OwaPickerUtil : OwaScriptsUtil
	{
		public OwaPickerUtil()
		{
			this.ID = "owaUtil";
			base.NameSpace = "OwaPeoplePickerUtil";
			this.CssClass = "OwaUtilDiv";
		}

		public static bool CanUseOwaPicker
		{
			get
			{
				LocalSession localSession = RbacPrincipal.GetCurrent(false) as LocalSession;
				return localSession != null && !EcpUrl.IsEcpStandalone && !localSession.IsCrossSiteMailboxLogon && localSession.IsInRole("LogonUserMailbox+OWA+MailboxFullAccess+!DelegatedAdmin");
			}
		}
	}
}
