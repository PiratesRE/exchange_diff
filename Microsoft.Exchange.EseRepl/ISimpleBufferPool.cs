using System;

namespace Microsoft.Exchange.EseRepl
{
	internal interface ISimpleBufferPool : IPool<SimpleBuffer>
	{
		int BufferSize { get; }

		SimpleBuffer TryGetObject(int bufSize);
	}
}
