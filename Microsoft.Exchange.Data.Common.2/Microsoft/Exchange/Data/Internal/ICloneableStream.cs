using System;
using System.IO;

namespace Microsoft.Exchange.Data.Internal
{
	internal interface ICloneableStream
	{
		Stream Clone();
	}
}
