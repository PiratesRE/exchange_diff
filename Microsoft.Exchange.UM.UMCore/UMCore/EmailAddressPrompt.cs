using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class EmailAddressPrompt : TextPrompt
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"emailAddress",
				base.Config.PromptName,
				string.Empty,
				base.Text.ToString()
			});
		}

		internal override string ToSsml()
		{
			int num = base.Text.IndexOf('@');
			if (-1 != num)
			{
				return this.AddProsodyWithVolume(string.Format(CultureInfo.InvariantCulture, "<say-as interpret-as=\"net\" format=\"email\">{0}</say-as>", new object[]
				{
					base.Text
				}));
			}
			return this.AddProsodyWithVolume(string.Format(CultureInfo.InvariantCulture, "<say-as interpret-as=\"letters\">{0}</say-as>", new object[]
			{
				base.Text
			}));
		}

		protected override void SanitizeRawText()
		{
			base.SanitizeRawText();
			base.RawText = Util.SanitizeStringForSayAs(base.RawText);
		}

		private const string SpellFormat = "<say-as interpret-as=\"letters\">{0}</say-as>";

		private const string SmtpFormat = "<say-as interpret-as=\"net\" format=\"email\">{0}</say-as>";
	}
}
