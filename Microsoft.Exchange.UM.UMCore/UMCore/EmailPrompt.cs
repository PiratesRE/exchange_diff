using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class EmailPrompt : TTSPrompt<EmailNormalizedText>
	{
		public EmailPrompt()
		{
		}

		internal EmailPrompt(string promptName, CultureInfo culture, EmailNormalizedText value) : base(promptName, culture, value)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"email",
				base.Config.PromptName,
				string.Empty,
				base.RawText.Substring(0, Math.Min(128, base.RawText.Length))
			});
		}

		protected override void InternalInitialize()
		{
			base.InternalInitialize();
			base.RawText = ((base.InitVal == null) ? string.Empty : base.InitVal.ToString());
			base.Text = this.AddProsodyWithVolume(base.RawText);
		}
	}
}
