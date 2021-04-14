using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AddressPrompt : TextPrompt
	{
		public AddressPrompt()
		{
		}

		public AddressPrompt(string promptName, CultureInfo culture, string value) : base(promptName, culture, value)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"address",
				base.Config.PromptName,
				string.Empty,
				base.Text.ToString()
			});
		}

		internal override string ToSsml()
		{
			return this.AddProsodyWithVolume("<say-as type=\"address\">" + Regex.Replace(base.Text ?? string.Empty, "(?<building>\\d\\d+)/(?<room>\\d\\d\\d\\d+)", "${building} / ${room}") + "</say-as>");
		}

		protected override void SanitizeRawText()
		{
			base.SanitizeRawText();
			base.RawText = Util.SanitizeStringForSayAs(base.RawText);
		}
	}
}
