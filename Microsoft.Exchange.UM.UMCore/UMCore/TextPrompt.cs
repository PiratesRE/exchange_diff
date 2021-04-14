using System;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TextPrompt : TTSPrompt<string>
	{
		public TextPrompt()
		{
		}

		internal TextPrompt(string promptName, CultureInfo culture, string value) : base(promptName, culture, value)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"text",
				base.Config.PromptName,
				string.Empty,
				base.RawText.Substring(0, Math.Min(128, base.RawText.Length))
			});
		}

		internal override string ToSsml()
		{
			return this.AddProsodyWithVolume(base.Text);
		}

		protected override void InternalInitialize()
		{
			base.InternalInitialize();
			this.SanitizeRawText();
			base.Text = Util.TextNormalize(SpeechUtils.XmlEncode(base.RawText));
		}

		protected virtual void SanitizeRawText()
		{
			base.RawText = ((base.InitVal == null) ? string.Empty : base.InitVal.TrimEnd(null));
		}
	}
}
