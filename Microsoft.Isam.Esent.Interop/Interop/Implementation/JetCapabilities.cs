using System;

namespace Microsoft.Isam.Esent.Interop.Implementation
{
	internal sealed class JetCapabilities
	{
		public bool SupportsServer2003Features { get; set; }

		public bool SupportsVistaFeatures { get; set; }

		public bool SupportsWindows7Features { get; set; }

		public bool SupportsWindows8Features { get; set; }

		public bool SupportsWindows81Features { get; set; }

		public bool SupportsUnicodePaths { get; set; }

		public bool SupportsLargeKeys { get; set; }

		public int ColumnsKeyMost { get; set; }
	}
}
