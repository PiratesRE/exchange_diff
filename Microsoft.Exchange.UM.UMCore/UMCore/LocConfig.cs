using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.UM.Prompts.Config;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class LocConfig
	{
		private LocConfig()
		{
			this.languages = new Dictionary<CultureInfo, LocConfig.LanguageConfig>(32);
		}

		public static LocConfig Instance
		{
			get
			{
				if (LocConfig.instance == null)
				{
					lock (LocConfig.lockObj)
					{
						if (LocConfig.instance == null)
						{
							LocConfig.instance = new LocConfig();
						}
					}
				}
				return LocConfig.instance;
			}
		}

		public LocConfig.LanguageConfig this[CultureInfo c]
		{
			get
			{
				c = (c ?? CultureInfo.InvariantCulture);
				if (!this.languages.ContainsKey(c))
				{
					lock (LocConfig.lockObj)
					{
						if (!this.languages.ContainsKey(c))
						{
							LocConfig.instance.languages.Add(c, LocConfig.LanguageConfig.Load(c));
						}
					}
				}
				return this.languages[c];
			}
		}

		private static object lockObj = new object();

		private static LocConfig instance;

		private Dictionary<CultureInfo, LocConfig.LanguageConfig> languages;

		internal class LanguageConfig
		{
			public LocConfig.TranscriptionConfig Transcription { get; private set; }

			public LocConfig.GeneralConfig General { get; private set; }

			public LocConfig.MowaSpeechConfig MowaSpeech { get; private set; }

			public static LocConfig.LanguageConfig Load(CultureInfo c)
			{
				return new LocConfig.LanguageConfig
				{
					General = LocConfig.GeneralConfig.Load(c),
					Transcription = LocConfig.TranscriptionConfig.Load(c),
					MowaSpeech = LocConfig.MowaSpeechConfig.Load(c)
				};
			}
		}

		internal class GeneralConfig
		{
			private GeneralConfig()
			{
			}

			public int TTSVolume { get; private set; }

			public bool SmartReadingHours { get; private set; }

			public static LocConfig.GeneralConfig Load(CultureInfo c)
			{
				LocConfig.GeneralConfig generalConfig = new LocConfig.GeneralConfig();
				generalConfig.TTSVolume = SafeConvert.ToInt32(Strings.TtsVolume.ToString(c), 0, 200, 100);
				int num = SafeConvert.ToInt32(Strings.SmartReadingHours.ToString(c), -1, 1, 0);
				generalConfig.SmartReadingHours = (1 == num || (num != 0 && c.Name == "en-US"));
				return generalConfig;
			}
		}

		internal class TranscriptionConfig
		{
			private TranscriptionConfig()
			{
			}

			public double HighConfidence { get; private set; }

			public double LowConfidence { get; private set; }

			public bool CapStartOfNewSentence { get; private set; }

			public TimeSpan SilenceThreshold { get; private set; }

			public int NumSentencesPerParagraph { get; private set; }

			public bool LastSentenceInNewLine { get; private set; }

			public bool FirstSentenceInNewLine { get; private set; }

			public static LocConfig.TranscriptionConfig Load(CultureInfo c)
			{
				return new LocConfig.TranscriptionConfig
				{
					HighConfidence = SafeConvert.ToDouble(Strings.TranscriptionHighConfidence.ToString(c), 0.0, 1.0, 0.75),
					LowConfidence = SafeConvert.ToDouble(Strings.TranscriptionLowConfidence.ToString(c), 0.0, 1.0, 0.3),
					CapStartOfNewSentence = SafeConvert.ToBoolean(Strings.CapStartOfNewSentence.ToString(c), true),
					SilenceThreshold = TimeSpan.FromMilliseconds(SafeConvert.ToDouble(Strings.SilenceThreshold.ToString(c), 100.0, 5000.0, 400.0)),
					LastSentenceInNewLine = SafeConvert.ToBoolean(Strings.LastSentenceInNewLine.ToString(c), true),
					FirstSentenceInNewLine = SafeConvert.ToBoolean(Strings.FirstSentenceInNewLine.ToString(c), true),
					NumSentencesPerParagraph = SafeConvert.ToInt32(Strings.NumSentencesPerParagraph.ToString(c), 1, int.MaxValue, 3)
				};
			}
		}

		internal class MowaSpeechConfig
		{
			private MowaSpeechConfig()
			{
			}

			public double MowaVoiceImmediateThreshold { get; private set; }

			public bool EnableMowaVoice { get; private set; }

			public static LocConfig.MowaSpeechConfig Load(CultureInfo c)
			{
				return new LocConfig.MowaSpeechConfig
				{
					MowaVoiceImmediateThreshold = SafeConvert.ToDouble(Strings.MowaVoiceImmediateThreshold.ToString(c), 0.0, 1.0, 0.4),
					EnableMowaVoice = SafeConvert.ToBoolean(Strings.EnableMowaVoice.ToString(c), false)
				};
			}
		}
	}
}
