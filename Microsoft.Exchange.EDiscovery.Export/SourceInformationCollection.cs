using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class SourceInformationCollection
	{
		public SourceInformationCollection(int count)
		{
			this.sources = new List<SourceInformation>(count);
			this.sourcesIndex = new Dictionary<string, int>(count);
		}

		public IEnumerable<SourceInformation> Values
		{
			get
			{
				return this.sources;
			}
		}

		public int Count
		{
			get
			{
				return this.sources.Count;
			}
		}

		public SourceInformation this[string sourceId]
		{
			get
			{
				return this.sources[this.sourcesIndex[sourceId]];
			}
			set
			{
				if (this.sourcesIndex.ContainsKey(sourceId))
				{
					this.sources[this.sourcesIndex[sourceId]] = value;
					return;
				}
				this.sourcesIndex[sourceId] = this.sources.Count;
				this.sources.Add(value);
			}
		}

		public SourceInformation this[int index]
		{
			get
			{
				return this.sources[index];
			}
		}

		public int GetSourceIndex(string sourceId)
		{
			return this.sourcesIndex[sourceId];
		}

		private readonly List<SourceInformation> sources;

		private readonly Dictionary<string, int> sourcesIndex;
	}
}
