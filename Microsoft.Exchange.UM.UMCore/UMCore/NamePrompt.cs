using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class NamePrompt : TextPrompt
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"name",
				base.Config.PromptName,
				string.Empty,
				base.Text
			});
		}

		internal override string ToSsml()
		{
			return this.AddProsodyWithVolume("<say-as type=\"name\">" + base.Text.Replace('.', ' ') + "</say-as>");
		}

		protected override void SanitizeRawText()
		{
			base.SanitizeRawText();
			base.RawText = Util.SanitizeStringForSayAs(base.RawText);
		}
	}
}
