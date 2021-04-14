using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsToastBinding : WnsBinding
	{
		public WnsToastBinding(WnsToast template, WnsToast? fallback = null, string language = null, string baseUri = null, WnsBranding? branding = null, bool? addImageQuery = null, WnsText[] texts = null, WnsImage[] images = null) : base(language, baseUri, branding, addImageQuery, texts, images)
		{
			this.Template = template;
			this.Fallback = fallback;
		}

		public WnsToast Template { get; private set; }

		public WnsToast? Fallback { get; private set; }

		public override WnsTemplateDescription TemplateDescription
		{
			get
			{
				return WnsTemplateDescription.GetToastDescription(this.Template);
			}
		}

		public override WnsTemplateDescription FallbackTemplateDescription
		{
			get
			{
				if (this.Fallback != null)
				{
					return WnsTemplateDescription.GetToastDescription(this.Fallback.Value);
				}
				return null;
			}
		}
	}
}
