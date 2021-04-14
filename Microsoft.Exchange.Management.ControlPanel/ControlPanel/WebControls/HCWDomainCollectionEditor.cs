using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("HCWDomainCollectionEditor", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:HCWDomainCollectionEditor runat=server></{0}:HCWDomainCollectionEditor>")]
	public class HCWDomainCollectionEditor : EcpCollectionEditor
	{
		protected override void InitListviewCommandCollection()
		{
			base.InitListviewCommandCollection();
			this.setAutodiscoverDomainCommand = new Command(string.Empty, CommandSprite.SpriteId.AutodiscoverDomain);
			this.setAutodiscoverDomainCommand.Name = "SetAutodiscoverDomain";
			this.setAutodiscoverDomainCommand.ImageAltText = Strings.SetAutodiscoverDomain;
			this.setAutodiscoverDomainCommand.OnClientClick = "HCWDomainCollectionEditor.SetAutodiscoverDomain";
			this.setAutodiscoverDomainCommand.Condition = "!HCWDomainCollectionEditor.IsAutodiscoverDomain($_)";
			this.setAutodiscoverDomainCommand.SelectionMode = SelectionMode.RequiresSingleSelection;
			base.Listview.Commands.Add(this.setAutodiscoverDomainCommand);
		}

		private Command setAutodiscoverDomainCommand;
	}
}
