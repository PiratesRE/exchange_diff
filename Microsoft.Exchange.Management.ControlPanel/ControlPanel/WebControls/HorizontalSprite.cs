using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class HorizontalSprite : BaseSprite
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

		public HorizontalSprite.SpriteId ImageId { get; set; }

		public string SpriteCssClass
		{
			get
			{
				return HorizontalSprite.GetCssClass(this.ImageId);
			}
		}

		public bool IsRenderable
		{
			get
			{
				return this.ImageId != HorizontalSprite.SpriteId.NONE;
			}
		}

		public static string GetCssClass(HorizontalSprite.SpriteId spriteid)
		{
			if (spriteid == HorizontalSprite.SpriteId.NONE)
			{
				return string.Empty;
			}
			return HorizontalSprite.GetBaseCssClass() + " " + HorizontalSprite.ImageStyles[(int)spriteid];
		}

		private static string GetBaseCssClass()
		{
			string text = HorizontalSprite.BaseCssClass;
			if (HorizontalSprite.HasDCImage && BaseSprite.IsDataCenter)
			{
				text = "DC" + text;
			}
			return text;
		}

		public static readonly string BaseCssClass = "HorizontalSprite";

		public static readonly bool HasDCImage = false;

		private static readonly string[] ImageStyles = new string[]
		{
			string.Empty,
			"HBGR",
			"A-HS"
		};

		public enum SpriteId
		{
			NONE,
			HBGR,
			EsoBar
		}
	}
}
