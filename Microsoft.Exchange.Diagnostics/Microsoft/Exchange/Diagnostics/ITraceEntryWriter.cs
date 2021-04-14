using System;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public interface ITraceEntryWriter
	{
		void Write(TraceEntry entry);
	}
}
