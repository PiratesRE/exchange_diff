using System;
using System.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class InterestingEvent
	{
		public static InterestingEvent Create(MapiEvent mapiEvent)
		{
			return new InterestingEvent(mapiEvent)
			{
				WasProcessed = true
			};
		}

		public static InterestingEvent CreateUnprocessed(MapiEvent mapiEvent)
		{
			return new InterestingEvent(mapiEvent);
		}

		private InterestingEvent(MapiEvent mapiEvent)
		{
			this.mapiEvent = mapiEvent;
		}

		public MapiEvent MapiEvent
		{
			get
			{
				return this.mapiEvent;
			}
		}

		public Stopwatch EnqueuedStopwatch
		{
			get
			{
				return this.enqueuedStopwatch;
			}
		}

		public bool WasProcessed { get; private set; }

		private MapiEvent mapiEvent;

		private Stopwatch enqueuedStopwatch = Stopwatch.StartNew();
	}
}
