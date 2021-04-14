using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class Prompt
	{
		public Prompt()
		{
		}

		internal Prompt(string promptName, CultureInfo culture)
		{
			PromptSetting promptSetting = new PromptSetting(promptName);
			this.Initialize(promptSetting, culture);
		}

		internal virtual CultureInfo TTSLanguage
		{
			set
			{
			}
		}

		internal string ProsodyRate
		{
			get
			{
				return this.prosodyRate;
			}
		}

		internal bool IsInitialized
		{
			get
			{
				return this.isInitialized;
			}
		}

		internal string PromptName
		{
			get
			{
				return this.config.PromptName;
			}
		}

		protected PromptSetting Config
		{
			get
			{
				return this.config;
			}
			set
			{
				this.config = value;
			}
		}

		protected CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
			}
		}

		internal abstract string ToSsml();

		internal void SetProsodyRate(float rate)
		{
			if (string.Equals(this.config.ProsodyRate, "user", StringComparison.OrdinalIgnoreCase))
			{
				this.prosodyRate = (rate * 100f).ToString(CultureInfo.InvariantCulture) + "%";
				if (rate >= 0f)
				{
					this.prosodyRate = "+" + this.prosodyRate;
				}
			}
		}

		internal void Initialize(PromptSetting config, CultureInfo c)
		{
			if (!this.IsInitialized)
			{
				this.config = config;
				this.culture = c;
				this.prosodyRate = (string.Equals(config.ProsodyRate, "user", StringComparison.OrdinalIgnoreCase) ? "+0%" : config.ProsodyRate);
				this.isInitialized = true;
				this.InternalInitialize();
			}
		}

		protected abstract void InternalInitialize();

		internal const string LogFormat = "Type={0}, Name={1}, File={2}, Value={3}";

		internal const string LogFormatWithExtraInfo = "Type={0}, Name={1}, File={2}, Value={3} Extra={4}";

		private PromptSetting config;

		private CultureInfo culture;

		private string prosodyRate;

		private bool isInitialized;
	}
}
