using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ReadDistributionListToolbar : Toolbar
	{
		internal ReadDistributionListToolbar(bool isInDeleteItems, DistributionList distributionList) : base(ToolbarType.Form)
		{
			this.isInDeleteItems = isInDeleteItems;
			this.distributionList = distributionList;
		}

		protected override void RenderButtons()
		{
			ToolbarButtonFlags flags = ItemUtility.UserCanDeleteItem(this.distributionList) ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled;
			ToolbarButtonFlags flags2 = ItemUtility.UserCanEditItem(this.distributionList) ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled;
			base.RenderButton(ToolbarButtons.NewMessageToDistributionList);
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderButton(ToolbarButtons.NewMeetingRequestToContact);
			}
			base.RenderButton(ToolbarButtons.Delete, flags);
			if (!this.isInDeleteItems)
			{
				base.RenderButton(ToolbarButtons.Flag, flags2);
			}
			base.RenderButton(ToolbarButtons.Categories, flags2);
		}

		private bool isInDeleteItems;

		private DistributionList distributionList;
	}
}
