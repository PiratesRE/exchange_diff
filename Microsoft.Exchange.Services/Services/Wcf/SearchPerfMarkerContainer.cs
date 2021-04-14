using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SearchPerfMarkerContainer
	{
		public SearchPerfMarkerContainer() : this(Stopwatch.StartNew())
		{
		}

		private SearchPerfMarkerContainer(Stopwatch stopWatch)
		{
			this.stopWatch = stopWatch;
		}

		public void SetPerfMarker(InstantSearchPerfKey key)
		{
			this.perfData.Add(new InstantSearchPerfMarkerType(key, (double)this.stopWatch.ElapsedTicks * 1000.0 / (double)Stopwatch.Frequency));
		}

		internal InstantSearchPerfMarkerType[] GetMarkerSnapshot()
		{
			return this.perfData.ToArray();
		}

		internal List<InstantSearchPerfMarkerType> MarkerCollection
		{
			get
			{
				return this.perfData;
			}
		}

		internal SearchPerfMarkerContainer GetDeepCopy()
		{
			SearchPerfMarkerContainer searchPerfMarkerContainer = new SearchPerfMarkerContainer(this.stopWatch);
			searchPerfMarkerContainer.perfData.AddRange(this.MarkerCollection);
			return searchPerfMarkerContainer;
		}

		private List<InstantSearchPerfMarkerType> perfData = new List<InstantSearchPerfMarkerType>(7);

		private readonly Stopwatch stopWatch;
	}
}
