using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class NavigationSprite : BaseSprite
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.CssClass = this.CssClass + " " + this.SpriteCssClass;
			this.ImageUrl = BaseSprite.GetSpriteImageSrc(this);
			string value = (this.AlternateText == null) ? "" : this.AlternateText;
			base.Attributes.Add("title", value);
			this.GenerateEmptyAlternateText = true;
		}

		public NavigationSprite.SpriteId ImageId { get; set; }

		public string SpriteCssClass
		{
			get
			{
				return NavigationSprite.GetCssClass(this.ImageId);
			}
		}

		public bool IsRenderable
		{
			get
			{
				return this.ImageId != NavigationSprite.SpriteId.NONE;
			}
		}

		public static string GetCssClass(NavigationSprite.SpriteId spriteid)
		{
			if (spriteid == NavigationSprite.SpriteId.NONE)
			{
				return string.Empty;
			}
			return NavigationSprite.GetBaseCssClass() + " " + NavigationSprite.ImageStyles[(int)spriteid];
		}

		private static string GetBaseCssClass()
		{
			string text = NavigationSprite.BaseCssClass;
			if (NavigationSprite.HasDCImage && BaseSprite.IsDataCenter)
			{
				text = "DC" + text;
			}
			return text;
		}

		public static readonly string BaseCssClass = "NavigationSprite";

		public static readonly bool HasDCImage = false;

		private static readonly string[] ImageStyles = new string[]
		{
			string.Empty,
			"OwaBrand",
			"Office365Icon",
			"EsoBarEdge",
			"ReturnToOWA",
			"NtfImgB",
			"NtfImg",
			"NtfInfo",
			"NtfError",
			"NtfWarning",
			"CollapsePriNav",
			"ExpandPriNav"
		};

		public enum SpriteId
		{
			NONE,
			OwaBrand,
			Office365Icon,
			EsoBarEdge,
			ReturnToOWA,
			NotificationBlue,
			NotificationNormal,
			NotificationInfo,
			NotificationError,
			NotificationWarning,
			CollapsePriNav,
			ExpandPriNav
		}
	}
}
