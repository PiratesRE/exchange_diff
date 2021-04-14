using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface IByteSource
	{
		bool GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkLength);

		void ReportOutput(int readCount);
	}
}
