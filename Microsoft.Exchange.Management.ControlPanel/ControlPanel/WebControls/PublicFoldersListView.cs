using System;
using System.ComponentModel;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("PublicFoldersListView", "Microsoft.Exchange.Management.ControlPanel.Client.OrgSettings.js")]
	public class PublicFoldersListView : ListView
	{
		[DefaultValue(false)]
		public virtual bool ShowAll { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ComponentBinding componentBinding = new ComponentBinding(this, "showAll");
			componentBinding.Name = "ShowAll";
			base.FilterParameters.Add(componentBinding);
		}

		public PublicFoldersListView()
		{
			base.SearchTextBox.SearchButtonImageId = new CommandSprite.SpriteId?(CommandSprite.SpriteId.Start);
		}
	}
}
