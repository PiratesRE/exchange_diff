using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SilencePrompt : Prompt
	{
		public override string ToString()
		{
			return string.Empty;
		}

		internal override string ToSsml()
		{
			return this.ssml;
		}

		protected override void InternalInitialize()
		{
			if (base.Config.PromptName.EndsWith("ms", StringComparison.InvariantCulture))
			{
				this.numSec = int.Parse(base.Config.PromptName.Substring(0, base.Config.PromptName.Length - 2), CultureInfo.InvariantCulture) / 1000;
			}
			else if (base.Config.PromptName.EndsWith("s", StringComparison.InvariantCulture))
			{
				this.numSec = int.Parse(base.Config.PromptName.Substring(0, base.Config.PromptName.Length - 1), CultureInfo.InvariantCulture);
			}
			else
			{
				this.numSec = 1;
			}
			string text = Path.Combine(Util.WavPathFromCulture(base.Culture), "Silence-1.wav");
			if (!File.Exists(text))
			{
				throw new FileNotFoundException(text);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.numSec; i++)
			{
				stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<audio src=\"{0}\" />", new object[]
				{
					text
				}));
			}
			this.ssml = stringBuilder.ToString();
		}

		private int numSec;

		private string ssml;
	}
}
