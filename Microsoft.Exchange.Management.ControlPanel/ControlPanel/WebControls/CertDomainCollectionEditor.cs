using System;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("CertDomainCollectionEditor", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ControlValueProperty("Values")]
	[ToolboxData("<{0}:CertDomainCollectionEditor runat=server></{0}:CertDomainCollectionEditor>")]
	[DefaultProperty("Text")]
	public class CertDomainCollectionEditor : EcpCollectionEditor
	{
		protected override void InitListviewCommandCollection()
		{
			base.InitListviewCommandCollection();
			string[] roles = (!string.IsNullOrEmpty(base.Attributes["SetRoles"])) ? base.Attributes["SetRoles"].ToArrayOfStrings() : null;
			if (!base.ReadOnly)
			{
				this.makeDefaultCommand = new Command(string.Empty, CommandSprite.SpriteId.FVAEnabled);
				this.makeDefaultCommand.Name = "Default";
				this.makeDefaultCommand.ImageAltText = Strings.CertificateDefaultDomain;
				this.makeDefaultCommand.SelectionMode = SelectionMode.RequiresSingleSelection;
				this.makeDefaultCommand.OnClientClick = "$find('" + this.ClientID + "').defaultCommand();";
				this.makeDefaultCommand.Roles = roles;
				this.makeDefaultCommand.Condition = "!$_.IsDefault";
				base.Listview.Commands.Add(this.makeDefaultCommand);
				Command removeCommand = base.RemoveCommand;
				if (removeCommand != null)
				{
					removeCommand.Condition = "!$_.IsDefault";
				}
			}
		}

		private Command makeDefaultCommand;
	}
}
