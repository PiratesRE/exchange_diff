using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal delegate AsyncReturnType RawDataHandler(byte[] data, int offset, int size);
}
