using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class TextProcessorGrouping : IGrouping<TextProcessorType, KeyValuePair<string, ExchangeBuild>>, IEnumerable<KeyValuePair<string, ExchangeBuild>>, IEnumerable
	{
		internal TextProcessorGrouping(IGrouping<TextProcessorType, KeyValuePair<string, ExchangeBuild>> textProcessorGrouping, IEqualityComparer<string> textProcessorIdsComparer = null)
		{
			if (textProcessorGrouping == null)
			{
				throw new ArgumentNullException("textProcessorGrouping");
			}
			TextProcessorType key = textProcessorGrouping.Key;
			ExAssert.RetailAssert(TextProcessorType.Function <= key && key <= TextProcessorType.Fingerprint, "The specified textProcessorType '{0}' is out-of-range", new object[]
			{
				key.ToString()
			});
			this.textProcessorType = key;
			this.textProcessors = textProcessorGrouping.ToDictionary((KeyValuePair<string, ExchangeBuild> textProcessor) => textProcessor.Key, (KeyValuePair<string, ExchangeBuild> textProcessor) => textProcessor.Value, textProcessorIdsComparer ?? ClassificationDefinitionConstants.TextProcessorIdComparer);
		}

		public TextProcessorType Key
		{
			get
			{
				return this.textProcessorType;
			}
		}

		IEnumerator<KeyValuePair<string, ExchangeBuild>> IEnumerable<KeyValuePair<string, ExchangeBuild>>.GetEnumerator()
		{
			return this.textProcessors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.textProcessors.GetEnumerator();
		}

		internal bool Contains(string textProcessorId)
		{
			return this.textProcessors.ContainsKey(textProcessorId);
		}

		internal int Count
		{
			get
			{
				return this.textProcessors.Count;
			}
		}

		private readonly TextProcessorType textProcessorType;

		private readonly Dictionary<string, ExchangeBuild> textProcessors;
	}
}
