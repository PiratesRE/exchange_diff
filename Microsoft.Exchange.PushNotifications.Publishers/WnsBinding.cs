using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class WnsBinding
	{
		public WnsBinding(string language = null, string baseUri = null, WnsBranding? branding = null, bool? addImageQuery = null, WnsText[] texts = null, WnsImage[] images = null)
		{
			this.Language = language;
			this.BaseUri = baseUri;
			this.Branding = branding;
			this.AddImageQuery = addImageQuery;
			this.Images = images;
			this.Texts = texts;
		}

		public abstract WnsTemplateDescription TemplateDescription { get; }

		public abstract WnsTemplateDescription FallbackTemplateDescription { get; }

		public string Language { get; private set; }

		public string BaseUri { get; private set; }

		public WnsBranding? Branding { get; private set; }

		public bool? AddImageQuery { get; private set; }

		public WnsImage[] Images { get; private set; }

		public WnsText[] Texts { get; private set; }

		public override string ToString()
		{
			return string.Format("{{template:{0}; fallback:{1}; lang:{2}; baseUri:{3}; branding:{4}; addImageQuery:{5}; images:{6}; texts:{7}}}", new object[]
			{
				this.TemplateDescription.Name,
				this.FallbackTemplateDescription.ToNullableString(null),
				this.Language.ToNullableString(),
				this.BaseUri.ToNullableString(),
				this.Branding.ToNullableString<WnsBranding>(),
				this.AddImageQuery.ToNullableString<bool>(),
				this.Images.ToNullableString(null),
				this.Texts.ToNullableString(null)
			});
		}

		internal void WriteWnsPayload(WnsPayloadWriter wpw)
		{
			ArgumentValidator.ThrowIfNull("wpw", wpw);
			wpw.WriteElementStart("binding", true);
			wpw.WriteTemplateAttribute("template", this.TemplateDescription, false);
			wpw.WriteTemplateAttribute("fallback", this.FallbackTemplateDescription, true);
			wpw.WriteLanguageAttribute("lang", this.Language, true);
			wpw.WriteUriAttribute("baseUri", this.BaseUri, true);
			wpw.WriteAttribute<WnsBranding>("branding", this.Branding, true);
			wpw.WriteAttribute<bool>("addImageQuery", this.AddImageQuery, true);
			wpw.WriteAttributesEnd();
			int num = 0;
			while (this.Images != null && num < this.Images.Length)
			{
				this.Images[num].WriteWnsPayload(num + 1, wpw);
				num++;
			}
			int num2 = 0;
			while (this.Texts != null && num2 < this.Texts.Length)
			{
				this.Texts[num2].WriteWnsPayload(num2 + 1, wpw);
				num2++;
			}
			wpw.WriteElementEnd();
		}
	}
}
