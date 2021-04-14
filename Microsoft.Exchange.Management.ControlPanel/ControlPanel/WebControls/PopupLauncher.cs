using System;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("PopupLauncher", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[TargetControlType(typeof(Control))]
	public class PopupLauncher : ExtenderControlBase
	{
		protected override void OnPreRender(EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.NavigationUrl))
			{
				string path = base.TargetControl.ResolveClientUrl(this.NavigationUrl);
				if (!LoginUtil.CheckUrlAccess(path))
				{
					Util.MakeControlRbacDisabled(base.TargetControl);
					base.Enabled = false;
				}
			}
			base.OnPreRender(e);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddUrlProperty("NavigationUrl", this.NavigationUrl, this);
			descriptor.AddComponentProperty("OwnerControl", this.OwnerControlID, this);
			descriptor.AddProperty("Width", this.Width, 510);
			descriptor.AddProperty("Height", this.Height, 564);
		}

		public string NavigationUrl
		{
			get
			{
				return this.navigationUrl ?? string.Empty;
			}
			set
			{
				this.navigationUrl = value;
			}
		}

		public string OwnerControlID
		{
			get
			{
				return this.ownerControlID ?? string.Empty;
			}
			set
			{
				this.ownerControlID = value;
			}
		}

		[DefaultValue(510)]
		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		[DefaultValue(564)]
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		private string navigationUrl;

		private string ownerControlID;

		private int width = 510;

		private int height = 564;
	}
}
