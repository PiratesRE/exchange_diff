using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class ZipConstants
	{
		public const uint EndOfCentralDirectorySignature = 101010256U;

		public const int ZipEntrySignature = 67324752;

		public const int ZipEntryDataDescriptorSignature = 134695760;

		public const int ZipDirEntrySignature = 33639248;
	}
}
