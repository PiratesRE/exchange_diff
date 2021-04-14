using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.Prompts.Config;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class OffensiveWordsFilter
	{
		private OffensiveWordsFilter(CultureInfo transcriptionLanguage) : this(transcriptionLanguage, Strings.OffensiveWordsList.ToString(transcriptionLanguage))
		{
		}

		public OffensiveWordsFilter(CultureInfo transcriptionLanguage, string offensiveWordsString)
		{
			this.offensiveWords = new HashSet<string>(StringComparer.Create(transcriptionLanguage, true));
			string[] array = offensiveWordsString.Split(new char[]
			{
				','
			});
			foreach (string item in array)
			{
				this.offensiveWords.Add(item);
			}
		}

		public static bool TryGet(CultureInfo transcriptionLanguage, out OffensiveWordsFilter offensiveWordsFilter)
		{
			return OffensiveWordsFilter.instances.TryGetValue(transcriptionLanguage, out offensiveWordsFilter);
		}

		public static void Init()
		{
			foreach (CultureInfo cultureInfo in Platform.Utilities.SupportedTranscriptionLanguages)
			{
				OffensiveWordsFilter.instances.Add(cultureInfo, new OffensiveWordsFilter(cultureInfo));
			}
		}

		public List<KeyValuePair<string, int>> Filter(List<KeyValuePair<string, int>> rawList)
		{
			List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(rawList.Count);
			foreach (KeyValuePair<string, int> item in rawList)
			{
				if (this.offensiveWords.Contains(item.Key))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "OffensiveWordsFilter::Filter filtering out offensive word '{0}'", new object[]
					{
						item.Key
					});
				}
				else
				{
					list.Add(item);
				}
			}
			return list;
		}

		private const char WordSeparator = ',';

		private static readonly Dictionary<CultureInfo, OffensiveWordsFilter> instances = new Dictionary<CultureInfo, OffensiveWordsFilter>();

		private readonly HashSet<string> offensiveWords;
	}
}
