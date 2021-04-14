using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class VoicemailSprite : BaseSprite
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

		public VoicemailSprite.SpriteId ImageId { get; set; }

		public string SpriteCssClass
		{
			get
			{
				return VoicemailSprite.GetCssClass(this.ImageId);
			}
		}

		public bool IsRenderable
		{
			get
			{
				return this.ImageId != VoicemailSprite.SpriteId.NONE;
			}
		}

		public static string GetCssClass(VoicemailSprite.SpriteId spriteid)
		{
			if (spriteid == VoicemailSprite.SpriteId.NONE)
			{
				return string.Empty;
			}
			return VoicemailSprite.GetBaseCssClass() + " " + VoicemailSprite.ImageStyles[(int)spriteid];
		}

		private static string GetBaseCssClass()
		{
			string text = VoicemailSprite.BaseCssClass;
			if (VoicemailSprite.HasDCImage && BaseSprite.IsDataCenter)
			{
				text = "DC" + text;
			}
			return text;
		}

		public static readonly string BaseCssClass = "VoicemailSprite";

		public static readonly bool HasDCImage = false;

		private static readonly string[] ImageStyles = new string[]
		{
			string.Empty,
			"G-VM",
			"D-VM",
			"C-VM",
			"F-VM",
			"A-VM",
			"B-VM",
			"E-VM"
		};

		public enum SpriteId
		{
			NONE,
			VoicemailPlayerPlaceHolder,
			VoiceMailSms32,
			VoiceMailPin32,
			VoiceMailGreeting32,
			TextMessaging32,
			Voicemail32,
			VoiceMailConfirm32
		}
	}
}
