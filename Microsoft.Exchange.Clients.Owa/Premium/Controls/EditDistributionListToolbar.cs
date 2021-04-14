using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class EditDistributionListToolbar : Toolbar
	{
		internal EditDistributionListToolbar(Item item) : base(ToolbarType.View)
		{
			this.item = item;
		}

		protected override void RenderButtons()
		{
			ToolbarButtonFlags flags = ToolbarButtonFlags.None;
			if (this.item != null && !ItemUtility.UserCanEditItem(this.item))
			{
				flags = ToolbarButtonFlags.Disabled;
			}
			ToolbarButtonFlags flags2 = ToolbarButtonFlags.None;
			if (this.item != null && !ItemUtility.UserCanDeleteItem(this.item))
			{
				flags2 = ToolbarButtonFlags.Disabled;
			}
			base.RenderButton(ToolbarButtons.SaveAndClose, flags);
			base.RenderButton(ToolbarButtons.NewMessageToDistributionList);
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderButton(ToolbarButtons.NewMeetingRequestToContact);
			}
			base.RenderButton(ToolbarButtons.Delete, flags2);
			base.RenderButton(ToolbarButtons.Flag, flags);
			base.RenderButton(ToolbarButtons.Categories, flags);
		}

		private Item item;
	}
}
