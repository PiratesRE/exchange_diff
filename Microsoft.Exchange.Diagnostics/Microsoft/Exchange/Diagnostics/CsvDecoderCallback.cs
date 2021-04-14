using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal delegate object CsvDecoderCallback(byte[] src, int offset, int count);
}
