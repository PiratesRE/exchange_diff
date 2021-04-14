using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public class ExportFile
	{
		public string Name { get; internal set; }

		public string Path { get; internal set; }

		public ulong Size { get; internal set; }

		public string Hash { get; internal set; }
	}
}
