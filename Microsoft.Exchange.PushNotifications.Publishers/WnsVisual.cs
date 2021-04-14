using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class WnsVisual
	{
		public WnsVisual(string language = null, string baseUri = null, WnsBranding? branding = null, bool? addImageQuery = null)
		{
			this.Language = language;
			this.BaseUri = baseUri;
			this.Branding = branding;
			this.AddImageQuery = addImageQuery;
		}

		public string Language { get; private set; }

		public string BaseUri { get; private set; }

		public WnsBranding? Branding { get; private set; }

		public bool? AddImageQuery { get; private set; }

		public override string ToString()
		{
			return string.Format("{{lang:{0}; baseUri:{1}; branding:{2}; addImageQuery:{3}}}", new object[]
			{
				this.Language.ToNullableString(),
				this.BaseUri.ToNullableString(),
				this.Branding.ToNullableString<WnsBranding>(),
				this.AddImageQuery.ToNullableString<bool>()
			});
		}

		internal void WriteWnsPayload(WnsPayloadWriter wpw)
		{
			ArgumentValidator.ThrowIfNull("wpw", wpw);
			wpw.WriteElementStart("visual", true);
			wpw.WriteLanguageAttribute("lang", this.Language, true);
			wpw.WriteUriAttribute("baseUri", this.BaseUri, true);
			wpw.WriteAttribute<WnsBranding>("branding", this.Branding, true);
			wpw.WriteAttribute<bool>("addImageQuery", this.AddImageQuery, true);
			wpw.WriteAttributesEnd();
			this.WriteWnsBindings(wpw);
			wpw.WriteElementEnd();
		}

		internal abstract void WriteWnsBindings(WnsPayloadWriter wpw);
	}
}
