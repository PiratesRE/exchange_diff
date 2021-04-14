using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class RemoveCommand : Command
	{
		public RemoveCommand(bool useDeleteImage = true)
		{
			this.Name = "Delete";
			base.ShortCut = "Delete";
			this.SelectionMode = SelectionMode.SupportsMultipleSelection;
			if (useDeleteImage)
			{
				base.ImageId = CommandSprite.SpriteId.ToolBarDelete;
				base.ImageAltText = Strings.DeleteCommandText;
				return;
			}
			base.ImageId = CommandSprite.SpriteId.ToolBarRemove;
			base.ImageAltText = Strings.RemoveCommandText;
		}
	}
}
