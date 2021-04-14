using System;

namespace Microsoft.Exchange.EseRepl
{
	[Serializable]
	public class CoconetConfig
	{
		public long DictionarySize { get; set; }

		public int SampleRate { get; set; }

		public int LzOption { get; set; }

		internal CoconetConfig(long dictionarySize, int sampleRate, int lzOpt)
		{
			this.DictionarySize = dictionarySize;
			this.SampleRate = sampleRate;
			this.LzOption = lzOpt;
		}

		public CoconetConfig() : this(16777216L, 8, 3)
		{
		}
	}
}
