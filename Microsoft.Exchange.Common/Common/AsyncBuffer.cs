using System;

namespace Microsoft.Exchange.Common
{
	internal struct AsyncBuffer
	{
		internal IAsyncResult AsyncResult;

		internal byte[] Buffer;

		internal long FileOffset;

		internal int ReadLength;

		internal bool Reading;

		internal bool WriteDeferred;
	}
}
