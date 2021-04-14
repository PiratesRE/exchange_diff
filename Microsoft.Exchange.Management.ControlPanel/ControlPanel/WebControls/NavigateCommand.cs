using System;
using System.ComponentModel;
using System.Security.Principal;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class NavigateCommand : Command
	{
		public NavigateCommand()
		{
		}

		public NavigateCommand(string text, CommandSprite.SpriteId imageID) : base(text, imageID)
		{
			this.SelectionParameterName = "id";
		}

		[DefaultValue(null)]
		[UrlProperty]
		public virtual string NavigateUrl { get; set; }

		[DefaultValue(false)]
		public virtual bool BypassUrlCheck { get; set; }

		[DefaultValue("_self")]
		public virtual string TargetFrame { get; set; }

		[DefaultValue("id")]
		public virtual string SelectionParameterName { get; set; }

		public override bool IsAccessibleToUser(IPrincipal user)
		{
			return (this.BypassUrlCheck || LoginUtil.CheckUrlAccess(this.NavigateUrl)) && base.IsAccessibleToUser(user);
		}

		protected internal override void PreRender(Control c)
		{
			base.PreRender(c);
			if (!string.IsNullOrEmpty(this.NavigateUrl))
			{
				this.NavigateUrl = c.ResolveClientUrl(this.NavigateUrl);
			}
		}
	}
}
