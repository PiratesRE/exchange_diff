using System;

namespace Microsoft.Exchange.Rpc
{
	internal class EmsmdbConstants
	{
		// Note: this type is marked as 'beforefieldinit'.
		static EmsmdbConstants()
		{
			int num = EmsmdbConstants.MaxRopBufferSize / 2;
			EmsmdbConstants.MinFastTransferChainSize = EmsmdbConstants.ExtendedBufferHeaderSize + num;
			EmsmdbConstants.MaxMapiHttpChainedPayloadSize = 268288;
			EmsmdbConstants.MaxMapiHttpChainedOutlookPayloadSize = 104448;
		}

		public static readonly int ExtendedBufferHeaderSize = 8;

		public static readonly int MaxAuxBufferSize = 4096;

		public static readonly int MaxExtendedAuxBufferSize = EmsmdbConstants.MaxAuxBufferSize + EmsmdbConstants.ExtendedBufferHeaderSize;

		public static readonly int MaxRopBufferSize = 32767;

		public static readonly int MaxExtendedRopBufferSize = EmsmdbConstants.MaxRopBufferSize + EmsmdbConstants.ExtendedBufferHeaderSize;

		public static readonly int MaxOutlookChainedExtendedRopBufferSize = EmsmdbConstants.ExtendedBufferHeaderSize + 98304;

		public static readonly int MaxChainedExtendedRopBufferSize = 262144;

		public static readonly int MinCompressionSize = 1025;

		public static readonly int MaxChainBuffers = 96;

		public static readonly int MinChainSize = 8192;

		public static readonly int MinQueryRowsChainSize = EmsmdbConstants.MaxRopBufferSize + EmsmdbConstants.ExtendedBufferHeaderSize;

		public static readonly int MinFastTransferChainSize;

		public static readonly int MaxMapiHttpChainedPayloadSize;

		public static readonly int MaxMapiHttpChainedOutlookPayloadSize;
	}
}
