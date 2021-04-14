using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsTileBinding : WnsBinding
	{
		public WnsTileBinding(WnsTile template, WnsTile? fallback = null, string language = null, string baseUri = null, WnsBranding? branding = null, bool? addImageQuery = null, WnsText[] texts = null, WnsImage[] images = null) : base(language, baseUri, branding, addImageQuery, texts, images)
		{
			this.Template = template;
			this.Fallback = fallback;
		}

		public WnsTile Template { get; private set; }

		public WnsTile? Fallback { get; private set; }

		public override WnsTemplateDescription TemplateDescription
		{
			get
			{
				return WnsTemplateDescription.GetTileDescription(this.Template);
			}
		}

		public override WnsTemplateDescription FallbackTemplateDescription
		{
			get
			{
				if (this.Fallback != null)
				{
					return WnsTemplateDescription.GetTileDescription(this.Fallback.Value);
				}
				return null;
			}
		}
	}
}
