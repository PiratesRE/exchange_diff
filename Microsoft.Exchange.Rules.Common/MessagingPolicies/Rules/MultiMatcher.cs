using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class MultiMatcher : IMatch
	{
		internal MultiMatcher()
		{
			this.matchers = new List<IMatch>();
		}

		internal void Add(IMatch matcher)
		{
			this.matchers.Add(matcher);
		}

		internal void Clear()
		{
			this.matchers.Clear();
		}

		public bool IsMatch(TextScanContext data)
		{
			return this.matchers.Any((IMatch matcher) => matcher.IsMatch(data));
		}

		public bool IsMatch(string text, string textId, RulesEvaluationContext rulesEvaluationContext)
		{
			TextScanContext cachedScanSession = MultiMatcher.GetCachedScanSession(text, textId, rulesEvaluationContext);
			return this.IsMatch(cachedScanSession);
		}

		public bool IsMatch(TextReader reader, string streamId, int maxStreamLength, RulesEvaluationContext rulesEvaluationContext)
		{
			string text;
			if (maxStreamLength > 0)
			{
				char[] array = new char[maxStreamLength];
				int length = reader.Read(array, 0, array.Length);
				text = new string(array, 0, length);
			}
			else
			{
				text = reader.ReadToEnd();
			}
			TextScanContext cachedScanSession = MultiMatcher.GetCachedScanSession(text, streamId, rulesEvaluationContext);
			return this.IsMatch(cachedScanSession);
		}

		private static TextScanContext GetCachedScanSession(string text, string textId, RulesEvaluationContext rulesEvaluationContext)
		{
			TextScanContext textScanContext = null;
			bool flag = rulesEvaluationContext != null && !string.IsNullOrEmpty(textId);
			if (flag)
			{
				rulesEvaluationContext.RegexMatcherCache.TryGetValue(textId, out textScanContext);
			}
			if (textScanContext == null)
			{
				textScanContext = new TextScanContext(text);
				if (flag)
				{
					rulesEvaluationContext.AddTextProcessingContext(textId, textScanContext);
				}
			}
			return textScanContext;
		}

		private List<IMatch> matchers;
	}
}
