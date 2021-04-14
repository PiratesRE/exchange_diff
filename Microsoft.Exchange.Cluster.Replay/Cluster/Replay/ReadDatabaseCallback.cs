using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal delegate int ReadDatabaseCallback(byte[] buffer, ulong fileReadOffset, int bytesToRead);
}
