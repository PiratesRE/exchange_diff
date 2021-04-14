using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ConnectionParameters
	{
		public ConnectionParameters(INamedObject id, ILog log, long maxBytesToTransfer = 9223372036854775807L, int timeout = 300000)
		{
			this.Id = id;
			this.Log = log;
			this.MaxBytesToTransfer = maxBytesToTransfer;
			this.Timeout = timeout;
		}

		public INamedObject Id { get; private set; }

		public ILog Log { get; private set; }

		public long MaxBytesToTransfer { get; private set; }

		public int Timeout { get; private set; }
	}
}
