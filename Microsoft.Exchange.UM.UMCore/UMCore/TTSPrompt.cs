using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class TTSPrompt<T> : VariablePrompt<T>
	{
		public TTSPrompt()
		{
		}

		public TTSPrompt(string promptName, CultureInfo culture, T value) : base(promptName, culture, value)
		{
		}

		internal string RawText
		{
			get
			{
				return this.rawText;
			}
			set
			{
				this.rawText = value;
			}
		}

		internal override CultureInfo TTSLanguage
		{
			set
			{
				if (string.Equals(base.Config.Language, "message", StringComparison.OrdinalIgnoreCase))
				{
					this.ttsLanguage = value;
				}
			}
		}

		protected string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		protected CultureInfo TtsLanguage
		{
			get
			{
				return this.ttsLanguage;
			}
			set
			{
				this.ttsLanguage = value;
			}
		}

		internal override string ToSsml()
		{
			string text = string.Concat(new string[]
			{
				"<prosody rate=\"",
				base.ProsodyRate,
				"\">",
				this.text,
				"</prosody>"
			});
			if (this.ttsLanguage == null)
			{
				return text;
			}
			return string.Concat(new string[]
			{
				"<voice xml:lang=\"",
				this.ttsLanguage.Name,
				"\">",
				text,
				"</voice>"
			});
		}

		protected override void InternalInitialize()
		{
			this.ttsLanguage = null;
			if (!string.Equals(base.Config.Language, "message", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					this.ttsLanguage = CultureInfo.GetCultureInfo(base.Config.Language);
					if (object.Equals(this.ttsLanguage, CultureInfo.InvariantCulture))
					{
						this.ttsLanguage = null;
					}
				}
				catch (ArgumentException)
				{
					this.ttsLanguage = null;
				}
			}
		}

		protected override string AddProsodyWithVolume(string text)
		{
			return Util.AddProsodyWithVolume((this.ttsLanguage != null) ? this.ttsLanguage : base.Culture, text);
		}

		private CultureInfo ttsLanguage;

		private string text;

		private string rawText;
	}
}
