using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	public interface PagedDataObject : IConfigurable
	{
		void ConvertDatesToLocalTime();

		void ConvertDatesToUniversalTime();
	}
}
