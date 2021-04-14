using System;

namespace Microsoft.Exchange.EseRepl
{
	internal delegate int ReadDatabaseCallback(byte[] buffer, ulong fileReadOffset, int bytesToRead);
}
