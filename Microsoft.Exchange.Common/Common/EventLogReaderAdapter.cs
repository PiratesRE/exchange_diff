using System;
using System.Diagnostics.Eventing.Reader;

namespace Microsoft.Exchange.Common
{
	internal class EventLogReaderAdapter : IDisposable
	{
		internal EventLogReaderAdapter(EventLogQuery query)
		{
			this.reader = new EventLogReader(query);
		}

		internal virtual EventRecord ReadEvent()
		{
			return this.reader.ReadEvent();
		}

		public void Dispose()
		{
			this.reader.Dispose();
		}

		protected virtual void Dispose(bool disposing)
		{
			this.reader.Dispose();
		}

		protected EventLogReader reader;
	}
}
