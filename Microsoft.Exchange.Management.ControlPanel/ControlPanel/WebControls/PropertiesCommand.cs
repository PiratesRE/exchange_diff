using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class PropertiesCommand : WizardCommand
	{
		public PropertiesCommand() : base(null, CommandSprite.SpriteId.ToolBarProperties)
		{
			this.SelectionMode = SelectionMode.RequiresSingleSelection;
			this.DefaultCommand = true;
			this.Name = "Edit";
			this.SingleInstance = true;
			base.ImageAltText = Strings.PropertiesCommandText;
		}

		[DefaultValue(true)]
		public override bool DefaultCommand
		{
			get
			{
				return base.DefaultCommand;
			}
			set
			{
				base.DefaultCommand = value;
			}
		}
	}
}
