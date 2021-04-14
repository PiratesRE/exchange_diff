using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface ITargetLocation
	{
		string WorkingLocation { get; }

		string ExportLocation { get; }

		string UnsearchableExportLocation { get; }
	}
}
