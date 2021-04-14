using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsText
	{
		public WnsText(string text, string lang = null)
		{
			this.Text = text;
			this.Language = lang;
		}

		public string Text { get; private set; }

		public string Language { get; private set; }

		public override string ToString()
		{
			return string.Format("{{text:{0}; lang:{1}}}", this.Text.ToNullableString(), this.Language.ToNullableString());
		}

		internal void WriteWnsPayload(int id, WnsPayloadWriter wpw)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("id", id);
			ArgumentValidator.ThrowIfNull("wpw", wpw);
			wpw.WriteElementStart("text", true);
			wpw.WriteAttribute("id", id);
			wpw.WriteLanguageAttribute("lang", this.Language, true);
			wpw.WriteAttributesEnd();
			wpw.WriteContent(this.Text);
			wpw.WriteElementEnd();
		}
	}
}
