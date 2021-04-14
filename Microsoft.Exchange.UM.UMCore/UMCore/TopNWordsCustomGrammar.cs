using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TopNWordsCustomGrammar : CustomGrammarBase
	{
		internal TopNWordsCustomGrammar(CultureInfo transcriptionLanguage, List<KeyValuePair<string, int>> filteredWords) : this(transcriptionLanguage, filteredWords, AppConfig.Instance.Service.TopNGrammarThreshold)
		{
		}

		internal TopNWordsCustomGrammar(CultureInfo transcriptionLanguage, List<KeyValuePair<string, int>> filteredWords, int threshold) : base(transcriptionLanguage)
		{
			this.filteredWords = filteredWords;
			this.threshold = threshold;
		}

		internal override string FileName
		{
			get
			{
				return "ExtTopN.grxml";
			}
		}

		internal override string Rule
		{
			get
			{
				return "ExtTopN";
			}
		}

		protected override List<GrammarItemBase> GetItems()
		{
			int num = 0;
			List<GrammarItemBase> list = new List<GrammarItemBase>(this.filteredWords.Count);
			foreach (KeyValuePair<string, int> keyValuePair in this.filteredWords)
			{
				if (keyValuePair.Value >= this.threshold)
				{
					num += keyValuePair.Value;
				}
			}
			foreach (KeyValuePair<string, int> keyValuePair2 in this.filteredWords)
			{
				if (keyValuePair2.Value >= this.threshold)
				{
					GrammarItem item = new GrammarItem(keyValuePair2.Key, TopNWordsCustomGrammar.topNWordTrueTag, base.TranscriptionLanguage, (float)keyValuePair2.Value / (float)num);
					list.Add(item);
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "TopN grammar adding {0} words that exceeded threshold {1}", new object[]
			{
				list.Count,
				this.threshold
			});
			return list;
		}

		private static readonly string topNWordTrueTag = "out.topNWords=true;";

		private readonly List<KeyValuePair<string, int>> filteredWords;

		private readonly int threshold;
	}
}
