using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class HomePageSprite : BaseSprite
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

		public HomePageSprite.SpriteId ImageId { get; set; }

		public string SpriteCssClass
		{
			get
			{
				return HomePageSprite.GetCssClass(this.ImageId);
			}
		}

		public bool IsRenderable
		{
			get
			{
				return this.ImageId != HomePageSprite.SpriteId.NONE;
			}
		}

		public static string GetCssClass(HomePageSprite.SpriteId spriteid)
		{
			if (spriteid == HomePageSprite.SpriteId.NONE)
			{
				return string.Empty;
			}
			return HomePageSprite.GetBaseCssClass() + " " + HomePageSprite.ImageStyles[(int)spriteid];
		}

		private static string GetBaseCssClass()
		{
			string text = HomePageSprite.BaseCssClass;
			if (HomePageSprite.HasDCImage && BaseSprite.IsDataCenter)
			{
				text = "DC" + text;
			}
			return text;
		}

		public static readonly string BaseCssClass = "HomePageSprite";

		public static readonly bool HasDCImage = false;

		private static readonly string[] ImageStyles = new string[]
		{
			string.Empty,
			"J-HP",
			"K-HP",
			"H-HP",
			"I-HP",
			"N-HP",
			"O-HP",
			"L-HP",
			"M-HP",
			"B-HP",
			"C-HP",
			"CalendarLog",
			"A-HP",
			"F-HP",
			"G-HP",
			"D-HP",
			"E-HP"
		};

		public enum SpriteId
		{
			NONE,
			RemotePowerShell,
			Rules,
			Password,
			Passwordliveid,
			Voicemail,
			WhatsNewForOrganizations,
			Themes,
			Users32,
			Forward,
			ImportContacts,
			CalendarLog,
			Forefront,
			Oof,
			Outlook,
			ManageOrganization,
			Mobiledevices
		}
	}
}
