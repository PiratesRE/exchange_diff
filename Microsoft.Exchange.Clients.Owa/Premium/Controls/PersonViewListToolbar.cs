using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class PersonViewListToolbar : ViewListToolbar
	{
		public PersonViewListToolbar(bool isMultiLine, bool isPublicFolder, bool isWebpart, ReadingPanePosition readingPanePosition) : base(isMultiLine, readingPanePosition)
		{
			this.isPublicFolder = isPublicFolder;
			this.isWebpart = isWebpart;
		}

		protected override bool ShowCategoryButton
		{
			get
			{
				return false;
			}
		}

		protected override void RenderButtons()
		{
			if (this.isPublicFolder && !this.isWebpart)
			{
				base.RenderButtons(ToolbarButtons.NewContactCombo, new ToolbarButton[]
				{
					ToolbarButtons.SearchInPublicFolder
				});
				return;
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.IncludeImportContactListButton.Enabled)
			{
				base.RenderButtons(ToolbarButtons.NewContactCombo, new ToolbarButton[]
				{
					ToolbarButtons.ImportContactList
				});
				return;
			}
			base.RenderButtons(ToolbarButtons.NewContactCombo, new ToolbarButton[0]);
		}

		protected override void RenderSharingButton()
		{
		}

		private void RenderShareContactMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.OpenSharedContact);
		}

		protected override void RenderNewMenuItems()
		{
			if (base.UserContext.IsFeatureEnabled(Feature.Contacts))
			{
				base.RenderMenuItem(ToolbarButtons.NewContact);
				base.RenderMenuItem(ToolbarButtons.NewContactDistributionList);
			}
			if (!this.isPublicFolder)
			{
				base.RenderMenuItem(ToolbarButtons.NewMessage);
			}
		}

		private readonly bool isPublicFolder;

		private readonly bool isWebpart;
	}
}
