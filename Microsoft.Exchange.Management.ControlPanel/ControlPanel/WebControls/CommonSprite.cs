using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class CommonSprite : BaseSprite
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

		public CommonSprite.SpriteId ImageId { get; set; }

		public string SpriteCssClass
		{
			get
			{
				return CommonSprite.GetCssClass(this.ImageId);
			}
		}

		public bool IsRenderable
		{
			get
			{
				return this.ImageId != CommonSprite.SpriteId.NONE;
			}
		}

		public static string GetCssClass(CommonSprite.SpriteId spriteid)
		{
			if (spriteid == CommonSprite.SpriteId.NONE)
			{
				return string.Empty;
			}
			return CommonSprite.GetBaseCssClass() + " " + CommonSprite.ImageStyles[(int)spriteid];
		}

		private static string GetBaseCssClass()
		{
			string text = CommonSprite.BaseCssClass;
			if (CommonSprite.HasDCImage && BaseSprite.IsDataCenter)
			{
				text = "DC" + text;
			}
			return text;
		}

		public static readonly string BaseCssClass = "CommonSprite";

		public static readonly bool HasDCImage = false;

		private static readonly string[] ImageStyles = new string[]
		{
			string.Empty,
			"OutlookLogo",
			"fvaArrow",
			"Warning32",
			"A-CO",
			"OfficeLogo",
			"B-CO",
			"Expand",
			"Collapse",
			"TrendingNeutral",
			"Information",
			"Information16",
			"Warning",
			"Warning16",
			"Error",
			"Blank",
			"ArrowExpand",
			"Error16",
			"ArrowDownW",
			"TrendingDown",
			"TrendingUp",
			"ArrowDown",
			"FvaBottom",
			"Plus",
			"Minus",
			"FvaTop",
			"ItmChk",
			"cbItmDisabled > .cbChkCol > .ItmChk",
			"Previous",
			"Next",
			"formletClose",
			"C-CO"
		};

		public enum SpriteId
		{
			NONE,
			OutlookLogo,
			fvaArrow,
			Warning32,
			Aok,
			OfficeLogo,
			SignInArrow,
			Expand,
			Collapse,
			TrendingNeutral,
			Information,
			Information16,
			Warning,
			Warning16,
			Error,
			Blank,
			ArrowExpand,
			Error16,
			ArrowDownW,
			TrendingDown,
			TrendingUp,
			ArrowDown,
			shadowBottom,
			Plus,
			Minus,
			shadowTop,
			ItemChecked,
			ItemDisabledChecked,
			Previous,
			Next,
			MetroDelete_small,
			MoreInfo
		}
	}
}
